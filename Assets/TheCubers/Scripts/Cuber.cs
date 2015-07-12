using UnityEngine;
using System.Collections;

namespace TheCubers
{
	[RequireComponent(typeof(Animator))]
	public class Cuber : MonoBehaviour
	{
		private static World World;

		public SkinnedMeshRenderer Mesh;
		public Material BodyMat;
		public Material InfectedBodyMat;
		public Animator animator;

		public bool Infected;
		public float Energy;
		public float EnergyConsistentDrain;
		public float EnergyHopDrain;
		public int Life;
		public int Fourths;

		public float Wait;
		private float timer;


		void Awake()
		{
			if (!World)
				World = GameObject.FindObjectOfType<World>();
			if (!World)
				Debug.LogError("Unable to find world!");

			if (!animator)
				animator = GetComponent<Animator>();
		}

		void Start()
		{

		}

		public void Init(bool infected)
		{
			Infected = infected;
			if (Infected)
			{
				Mesh.material = InfectedBodyMat;
				Energy = 1f;
				Life = 200;
			}
			else
			{
				Mesh.material = BodyMat;
				Energy = 0.5f;
				Life = 100;
			}
			Fourths = 0;

			timer = (float)World.Random.NextDouble() * Wait;
			transform.rotation = Quaternion.Euler(0, World.Random.Next(360), 0);
			animator.Play("Idle", 0, (float)World.Random.NextDouble());
		}

		void Update()
		{
			Energy -= EnergyConsistentDrain * Time.deltaTime;
			if (Energy <= 0f)
			{
				World.Kill(this);
				return;
			}

			// only do movment updates every Wait.
			timer += Time.deltaTime;
			if (timer > Wait)
				timer = 0f;
			else
				return;

			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
				return;

			// Find a target to go for.
			Vector3 target = Vector3.zero;
			Fourth fTarget = null;
			Energy eTarget = null;
			float distance = float.MaxValue;
			float tmp;
			if (Fourths < 4)
			{
				var fourths = World.GetFourthsInView(transform.position);
				for (int i = 0; i < fourths.Count; ++i)
				{
					tmp = Vector3.Distance(transform.position, fourths[i].transform.position);
					if (tmp < distance)
					{
						distance = tmp;
						fTarget = fourths[i];
						target = fourths[i].transform.position;
					}
				}
			}
			// only try for energy if we did not find a fourth.
			if (!fTarget)
			{
				float weight = 0f;
				var energy = World.GetEnergyInView(transform.position);
				for (int i = 0; i < energy.Count; ++i)
				{
					tmp = Vector3.Distance(transform.position, energy[i].transform.position);
					if ((float)energy[i].Amount * 10f / tmp > weight)
					{
						distance = tmp;
						eTarget = energy[i];
						weight = (float)energy[i].Amount * 10f / tmp;
						target = energy[i].transform.position;
					}
				}
			}

			// no target return
			if (distance == float.MaxValue)
				return;

			target.y = transform.position.y;
			transform.LookAt(target);
			//target.Normalize();
			//transform.rotation = Quaternion.LookRotation(target, Vector3.up);

			// if we are close, eat it.
			if (distance < 2f)
			{
				if (fTarget)
				{
					Fourths += 1;
					fTarget.gameObject.SetActive(false);
				}
				else
				{
					Energy += eTarget.Amount;
					eTarget.gameObject.SetActive(false);
				}
				animator.SetTrigger("Eat");

			} // else hop to it.
			else if (Energy > EnergyHopDrain)
			{
				Energy -= EnergyHopDrain;
				animator.SetTrigger("Hop");
			}

		}
	}
}