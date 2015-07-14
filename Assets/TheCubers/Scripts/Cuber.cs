using UnityEngine;
using System.Collections;

namespace TheCubers
{
	[RequireComponent(typeof(Animator))]
	public class Cuber : MonoBehaviour
	{
		private static World world;
		[System.Serializable]
		public class Global
		{
			[Range(0.01f, 10f)]
			public float Wait;
			[Range(1, 500)]
			public float View;

			[Header("Body materials")]
			public Material BodyMat;
			public Material InfectedBodyMat;

			[Header("Energy costs")]
			public float EnergyConsistent;
			public float EnergyHop;
		}

		public SkinnedMeshRenderer Mesh;
		private Animator animator;

		public bool Infected;
		public float Energy;
		public int Life;
		public int Fourths;
		public Color FourthsColor;

		private float timer;

		void Awake()
		{
			if (!world)
				world = GameObject.FindObjectOfType<World>();
			if (!world)
				Debug.LogError("Unable to find world!");

			if (!animator)
				animator = GetComponent<Animator>();
		}

		void Start()
		{

		}

		public void Init(bool infected, Color color)
		{
			Infected = infected;
			if (Infected)
			{
				Mesh.material = world.CuberGlobal.InfectedBodyMat;
				Energy = 1f;
				Life = 200;
			}
			else
			{
				Mesh.material = world.CuberGlobal.BodyMat;
				if (color != Color.black)
					Mesh.material.color = color;
				Energy = 0.5f;
				Life = 100;
			}
			Fourths = 0;
			FourthsColor = Color.black;

			timer = (float)world.Random.NextDouble() * world.CuberGlobal.Wait;
			transform.rotation = Quaternion.Euler(0, world.Random.Next(360), 0);
			animator.Play("Idle", 0, (float)world.Random.NextDouble());
		}

		void Update()
		{
			Energy -= world.CuberGlobal.EnergyConsistent * Time.deltaTime;
			if (Energy <= 0f)
			{
				world.Kill(this);
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
				world.Kill(this);
				return;
			}

			// Hack to fix not being on ground.
			{
				Vector3 position;
				if (World.FindGround(new Ray(transform.position + Vector3.up, Vector3.down), out position))
				{
					if (transform.position.y != position.y)
					{
						transform.position = position;
						Debug.LogWarning("HACK: Fixed not being on ground.");
					}
				}
			}

			// repruduce
			if (Fourths == 4)
			{
				Vector3 position;
				if (World.FindGround(new Ray(transform.position + Vector3.up, Vector3.down), out position))
				{
					Fourths = 0;
					Life -= 4;
					animator.SetTrigger("GiveBirth");
					timer -= 5f;
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
					FourthsColor.r += fTarget.Color.r / 4;
					FourthsColor.g += fTarget.Color.g / 4;
					FourthsColor.b += fTarget.Color.b / 4;
					fTarget.gameObject.SetActive(false);
				}
				else
				{
					Energy += eTarget.Amount;
					eTarget.gameObject.SetActive(false);
				}
				animator.SetTrigger("Eat");

			} // else hop to it.
			else if (Energy > world.CuberGlobal.EnergyHop)
			{
				Energy -= world.CuberGlobal.EnergyHop;
				animator.SetTrigger("Hop");
			}

		}

		public void GiveBirth()
		{
			Vector3 position;
			World.FindGround(new Ray(transform.position + Vector3.up, Vector3.down), out position);
			world.NewCuber(position, Infected, FourthsColor);
		}
	}
}