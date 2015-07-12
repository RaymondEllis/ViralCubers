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
			timer += Time.deltaTime;
			if (timer > Wait)
				timer = 0f;
			else
				return;



		}
	}
}