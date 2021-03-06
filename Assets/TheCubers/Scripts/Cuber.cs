﻿using UnityEngine;

namespace TheCubers
{
	/// <summary>
	/// All of the Cubers 'AI' is in here.
	/// </summary>
	[SelectionBase]
	[RequireComponent(typeof(Animator))]
	public class Cuber : MonoBehaviour
	{
		/// <summary> Global variables used in every cuber. </summary>
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
			public Color ColorOld;
			public Color ColorOldInfected;

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

		public void Init(Vector3 position, bool infected, float lifeMul)//, Color color)
		{
			lifeMul = Mathf.Clamp01(lifeMul);
			Infected = infected;
			Energy = world.CuberGlobal.InitialEnergy * lifeMul;
			Life = (int)((float)world.CuberGlobal.InitialLife * lifeMul);
			if (Infected)
			{
				//Mesh.material = world.CuberGlobal.InfectedBodyMat;
				Energy *= 2f;
				Life *= 2;
			}
			//else
			//{
			//	Mesh.material = world.CuberGlobal.BodyMat;
			//	if (color != Color.black)
			//		Mesh.material.color = color;
			//}
			Mesh.material = world.GetCuberMat(infected, Life);
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
			Mesh.material = world.GetCuberMat(Infected, Life);

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
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying && enabled)
				findMoveEat(true);
#endif
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
					tmpWeight = ((Fourths + 1) * g.WeightFourth) / chkZ(fourths[i].Wanted * g.WeightWanted) / chkZ(tmpDistance * g.WeightDistance);
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
				tmpWeight = (energy[i].Amount * g.WeightEnergy) / chkZ(Energy * g.WeightCurrentEnergy) / chkZ(energy[i].Wanted * g.WeightWanted) / chkZ(tmpDistance * g.WeightDistance);
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
				targetFood = target;
				animator.SetTrigger("Eat");

			} // else hop to it.
			else if (Energy > world.CuberGlobal.EnergyHop)
			{
				Energy -= world.CuberGlobal.EnergyHop;
				animator.SetTrigger("Hop");
			}

		}

		private float chkZ(float val)
		{
			if (val == 0f)
				return 1f;
			return val;
		}

		public void FinishEating()
		{
			if (targetFood.Consumed)
			{
				targetFood = null;
				return;
			}

			if (targetFood is Fourth)
			{
				Fourths += 1;
				//Color color = ((Fourth)targetFood).Color.gamma;
				//FourthsColor.r += color.r / 4f;
				//FourthsColor.g += color.g / 4f;
				//FourthsColor.b += color.b / 4f;
			}
			else if (targetFood is Energy)
			{

				Energy += ((Energy)targetFood).TakePortion();
			}
			else
				Debug.LogWarning("Unknown edible: " + targetFood.GetType().Name);

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
			UIGame menu = UIBase.Instance.GetMenu<UIGame>();
			menu.CountDeath(Energy, Life);

			world.Kill(this);
		}

		public void GiveBirth()
		{
			if (Fourths < 4)
			{
				Debug.LogError("Tried to give birth with only " + Fourths + " fourths.");
				return;
			}

			Vector3 position;
			World.FindGround(new Ray(transform.position + Vector3.up, Vector3.down), out position);
			world.NewCuber(position, Infected, FourthsColor.linear);

			Fourths = 0;
			FourthsColor = Color.black.gamma;
		}
	}
}