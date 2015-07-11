using System.Collections.Generic;
using UnityEngine;

namespace TheCubers
{
	public class World : MonoBehaviour
	{
		public Cuber PrefabCuber;
		public Fourth PrefabFouth;
		public Energy PrefabEnergy;

		public startupvar Startup = new startupvar();
		[System.Serializable]
		public struct startupvar
		{
			public int Seed;
			public int Cubers;
			public int PrecentInfected;
		}


		private Cuber[] cubers;
		private Pool<Fourth> fourths;

		private System.Random rnd;

		private int sizeX, sizeZ;
		private int sizeXhalf, sizeZhalf;

		void Start()
		{
			if (!PrefabCuber || !PrefabEnergy || !PrefabFouth)
			{
				Debug.LogError("Missing a prefab!");
				return;
			}

			rnd = new System.Random(Startup.Seed);

			sizeX = (int)transform.localScale.x;
			sizeXhalf = sizeX / 2;
			sizeZ = (int)transform.localScale.z;
			sizeZhalf = sizeZ / 2;
			if ((float)sizeX != transform.localScale.x || (float)sizeZ != transform.localScale.z)
			{
				Debug.LogWarning("World lcoal scale should be int.");
			}

			// fill cubers pool.
			cubers = new Cuber[Startup.Cubers];
			for (int i = 0; i < cubers.Length; ++i)
			{
				cubers[i] = Object.Instantiate<Cuber>(PrefabCuber);
				cubers[i].name = "Cuber " + i;
				cubers[i].enabled = false;
				cubers[i].World = this;
			}
			// fourths
			fourths = new Pool<Fourth>(PrefabFouth, Startup.Cubers);


			// enable cubers
			bool hasInfected = false;
			for (int i = 0; i < cubers.Length; ++i)
			{
				cubers[i].transform.position = new Vector3(-sizeXhalf + rnd.Next(sizeX), 0f, -sizeZhalf + rnd.Next(sizeZ));
				cubers[i].Init(rnd.Next(cubers.Length) < cubers.Length / Startup.PrecentInfected);
				if (cubers[i].Infected)
					hasInfected = true;
			}
			// we need at least one infected.
			if (!hasInfected)
				cubers[0].Init(true);

			Kill(cubers[1]);
		}


		public void Kill(Cuber cuber)
		{
			cuber.gameObject.SetActive(false);

			Vector3 p = cuber.transform.position;
			Quaternion r = cuber.transform.rotation;

			Color color;
			if (cuber.Infected)
				color = Color.white;
			else
				color = cuber.BodyMat.color;

			Fourth f;
			Quaternion fr;

			// bottom
			f = fourths.Get();
			fr = r * Quaternion.Euler(0, 0, 0);
			f.transform.position = p + new Vector3(0f, 0f, 0f);
			f.transform.rotation = fr;
			f.Init(color);

			f = fourths.Get();
			fr = r * Quaternion.Euler(0, 180, 0);
			f.transform.position = p + new Vector3(0f, 0f, 0f);
			f.transform.rotation = fr;
			f.Init(color);

			// top
			f = fourths.Get();
			fr = r * Quaternion.Euler(180, 0, 0);
			f.transform.position = p + new Vector3(0f, 1f, 0f);
			f.transform.rotation = fr;
			f.Init(color);

			f = fourths.Get();	
			fr = r * Quaternion.Euler(180, 180, 0);
			f.transform.position = p + new Vector3(0f, 1f, 0f);
			f.transform.rotation = fr;
			f.Init(color);
		}

		void Update()
		{

		}

		void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale);
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale + new Vector3(1f, 0f, 1f));
		}
	}
}