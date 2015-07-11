using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public class Cuber : MonoBehaviour
	{
		public MeshRenderer BodyMesh;
		public Material BodyMat;
		public Material InfectedBodyMat;

		[System.NonSerialized]
		public World World;

		public bool Infected;
		public float Energy;
		public int Life;
		public int Fourths;

		public float Wait;
		private float timer;



		void Start()
		{

		}

		/// <summary>
		/// Sets vars and enables
		/// </summary>
		public void Init(bool infected)
		{
			Infected = infected;
			if (Infected)
			{
				BodyMesh.material = InfectedBodyMat;
				Energy = 1f;
				Life = 200;
			}
			else
			{
				BodyMesh.material = BodyMat;
				Energy = 0.5f;
				Life = 100;
			}
			Fourths = 0;
			enabled = true;
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