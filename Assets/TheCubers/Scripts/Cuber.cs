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

			[Header("Need Weights")]
			public float WeightDistance;
			public float WeightFourth;
			public float WeightEnergy;
			public float WeightCurrentEnergy;
			public float WeightWanted;
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

			findMoveEat(false);
		}

		void OnDrawGizmosSelected()
		{
			findMoveEat(true);
		}

		private void findMoveEat(bool debug)
		{
			Global g = World.Instance.CuberGlobal;
			// Find a target to go for.
			Edible target = null;
			float distance = float.MaxValue;
			float tmpDistance, tmpWeight;
			float weight = 0f;
			if (Fourths < 4)
			{
				var fourths = world.GetFourthsInView(transform.position);
				for (int i = 0; i < fourths.Count; ++i)
				{
					tmpDistance = Vector3.Distance(transform.position, fourths[i].transform.position);
					tmpWeight = ((Fourths + 1) * g.WeightFourth) / (fourths[i].Wanted * g.WeightWanted) / (tmpDistance * g.WeightDistance);
					if (debug)
					{
						Vector3 v = fourths[i].transform.position;
						v.y += 2f + tmpWeight * 0.5f;
						Gizmos.color = Color.blue;
						Gizmos.DrawCube(v, new Vector3(0.2f, tmpWeight, 0.2f));
					}

					if (tmpWeight > weight)
					{
						distance = tmpDistance;
						weight = tmpWeight;
						target = fourths[i];
					}
				}
			}
			// try for energy 
			var energy = world.GetEnergyInView(transform.position);
			for (int i = 0; i < energy.Count; ++i)
			{
				tmpDistance = Vector3.Distance(transform.position, energy[i].transform.position);
				tmpWeight = (energy[i].Amount * g.WeightEnergy) / (Energy * g.WeightCurrentEnergy) / (energy[i].Wanted * g.WeightWanted) / (tmpDistance * g.WeightDistance);
				if (debug)
				{
					Vector3 v = energy[i].transform.position;
					v.y += 2f + tmpWeight * 0.5f;
					Gizmos.color = Color.yellow;
					Gizmos.DrawCube(v, new Vector3(0.2f, tmpWeight, 0.2f));
				}

				if (tmpWeight > weight)
				{
					distance = tmpDistance;
					weight = tmpWeight;
					target = energy[i];
				}
			}

			// no target return
			if (!target || debug)
				return;

			transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));

			target.Want();

			// if we are close, eat it.
			if (distance < 1.5f)
			{
				if (target is Fourth)
				{
					Fourths += 1;
					Color color = ((Fourth)target).Color.gamma;
					FourthsColor.r += color.r / 4f;
					FourthsColor.g += color.g / 4f;
					FourthsColor.b += color.b / 4f;
				}
				else
				{
					Energy += ((Energy)target).Amount;
				}
				targetFood = target;
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