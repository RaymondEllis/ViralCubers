using UnityEngine;
using System.Collections;

namespace TheCubers
{
	[RequireComponent(typeof(Animator))]
	[SelectionBase]
	public class Cuber : MonoBehaviour
	{
		[System.Serializable]
		public class Global
		{
			[Range(0.01f, 10f)]
			public float Wait;
			[Range(1, 500)]
			public float View;

			public int InitialLife;
			public float InitialEnergy;

			[Header("Body materials")]
			public Material BodyMat;
			public Material InfectedBodyMat;

			[Header("Energy costs")]
			public float EnergyConsistent;
			public float EnergyHop;
		}

		private World world;
		private Animator animator;

		public SkinnedMeshRenderer Mesh;
		public bool Infected;
		public float Energy;
		public int Life;
		public int Fourths;
		public Color FourthsColor;

		private float timer;
		private bool dead;
		private Edible targetFood;

		void Awake()
		{
			if (!animator)
				animator = GetComponent<Animator>();
		}

		void OnEnable()
		{
			world = World.Instance;
		}

		public void Init(Vector3 position, bool infected, int livedev, Color color)
		{
			Infected = infected;
			Energy = world.CuberGlobal.InitialEnergy;
			Life = livedev;
			if (Infected)
			{
				Mesh.material = world.CuberGlobal.InfectedBodyMat;
				Energy *= 2f;
				Life *= 2;
			}
			else
			{
				Mesh.material = world.CuberGlobal.BodyMat;
				if (color != Color.black)
					Mesh.material.color = color;
			}
			Fourths = 0;
			FourthsColor = Color.black.gamma;

			timer = (float)world.Random.NextDouble() * world.CuberGlobal.Wait;
			transform.position = position;
			transform.rotation = Quaternion.Euler(0, world.Random.Next(360), 0);
			//ToDo : Animations are not being reset...
			//animator.Play("Idle", 0, (float)world.Random.NextDouble());
			animator.SetTrigger("Idle");
			dead = false;
			targetFood = null;
		}

		void Update()
		{
			if (World.Paused)
				return;

			if (dead)
				return;
			Energy -= world.CuberGlobal.EnergyConsistent * Time.deltaTime;
			if (Energy <= 0f)
			{
				BeginDeath();
				return;
			}

			// only do movment updates every Wait.
			timer += Time.deltaTime;
			if (timer > world.CuberGlobal.Wait)
				timer = 0f;
			else
				return;

			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
				return;

			Life -= 1;
			if (Life <= 0)
			{
				BeginDeath();
				return;
			}

			if (targetFood)
			{
				Debug.LogWarning("Did not finish eating!");
				targetFood = null;
			}

			// Hack to fix not being on ground.
			{
				Vector3 position;
				if (World.FindGround(new Ray(transform.position + Vector3.up, Vector3.down), out position))
				{
					float error = 0.25f;
					if (transform.position.y + error < position.y || transform.position.y - error > position.y)
					{
						float diff = transform.position.y - position.y;
						transform.position = position;
						Debug.LogWarning("HACK: Fixed not being on ground. difference: " + diff);
					}
				}
			}

			// repruduce
			if (Fourths == 4)
			{
				Vector3 position;
				if (World.FindGround(new Ray(transform.position + Vector3.up, Vector3.down), out position))
				{
					Life -= 4;
					animator.SetTrigger("GiveBirth");
					//timer -= 5f;
					return;
				}
			}

			// Find a target to go for.
			Vector3 target = Vector3.zero;
			Fourth fTarget = null;
			Energy eTarget = null;
			float distance = float.MaxValue;
			float tmpDistance, tmpWeight;
			float weight = 0f;
			if (Fourths < 4)
			{
				var fourths = world.GetFourthsInView(transform.position);
				for (int i = 0; i < fourths.Count; ++i)
				{
					if (!fourths[i].CanReserve)
						continue;

					tmpDistance = Vector3.Distance(transform.position, fourths[i].transform.position);
					tmpWeight = 10f * ((float)Fourths + 1f) / tmpDistance;
					if (tmpWeight > weight)
					{
						distance = tmpDistance;
						weight = tmpWeight;
						fTarget = fourths[i];
						target = fourths[i].transform.position;
					}
				}
			}
			// try for energy 
			var energy = world.GetEnergyInView(transform.position);
			for (int i = 0; i < energy.Count; ++i)
			{
				if (!energy[i].CanReserve)
					continue;

				tmpDistance = Vector3.Distance(transform.position, energy[i].transform.position);
				tmpWeight = (float)energy[i].Amount * 10f / Energy / tmpDistance;
				if (tmpWeight > weight)
				{
					distance = tmpDistance;
					weight = tmpWeight;
					fTarget = null;
					eTarget = energy[i];
					target = energy[i].transform.position;
				}
			}

			// no target return
			if (distance == float.MaxValue)
				return;

			target.y = transform.position.y;
			transform.LookAt(target);

			// if we are close, eat it.
			if (distance < 2f)
			{
				if (fTarget)
				{
					Fourths += 1;
					Color color = fTarget.Color.gamma;
					FourthsColor.r += color.r / 4f;
					FourthsColor.g += color.g / 4f;
					FourthsColor.b += color.b / 4f;
					targetFood = fTarget;
				}
				else
				{
					Energy += eTarget.Amount;
					targetFood = eTarget;
				}
				targetFood.Reserve();
				animator.SetTrigger("Eat");

			} // else hop to it.
			else if (Energy > world.CuberGlobal.EnergyHop)
			{
				Energy -= world.CuberGlobal.EnergyHop;
				animator.SetTrigger("Hop");
			}

		}

		public void FinishEating()
		{
			targetFood.Consume();
			targetFood = null;
		}

		public void BeginDeath()
		{
			dead = true;
			animator.SetTrigger("Die");
			animator.ResetTrigger("Idle");
		}
		public void EndDeath()
		{
			UIGame menu = (UIGame)UIBase.Instance.GetMenu("Game");
			menu.CountDeath(Energy, Life);

			world.Kill(this);
		}

		public void GiveBirth()
		{
			Vector3 position;
			World.FindGround(new Ray(transform.position + Vector3.up, Vector3.down), out position);
			world.NewCuber(position, Infected, FourthsColor.linear);

			Fourths = 0;
			FourthsColor = Color.black.gamma;
		}
	}
}