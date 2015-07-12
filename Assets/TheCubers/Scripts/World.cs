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

		public float CubersView;

		private Pool<Cuber> cubers;
		private Pool<Fourth> fourths;
		private Pool<Energy> energys;

		public System.Random Random { get; private set; }

		private int sizeX, sizeZ;
		private int sizeXhalf, sizeZhalf;

		void Start()
		{
			if (!PrefabCuber || !PrefabEnergy || !PrefabFouth)
			{
				Debug.LogError("Missing a prefab!");
				return;
			}

			Random = new System.Random(Startup.Seed);

			sizeX = (int)transform.localScale.x;
			sizeXhalf = sizeX / 2;
			sizeZ = (int)transform.localScale.z;
			sizeZhalf = sizeZ / 2;
			if ((float)sizeX != transform.localScale.x || (float)sizeZ != transform.localScale.z)
			{
				Debug.LogWarning("World lcoal scale should be int.");
			}

			// create pools
			cubers = new Pool<Cuber>(PrefabCuber, Startup.Cubers);
			fourths = new Pool<Fourth>(PrefabFouth, Startup.Cubers);
			energys = new Pool<Energy>(PrefabEnergy, Startup.Cubers);


			// enable cubers
			bool hasInfected = false;
			Cuber cuber = null;
			for (int i = 0; i < Startup.Cubers; ++i)
			{
				cuber = cubers.Pull();
				cuber.transform.position = new Vector3(-sizeXhalf + Random.Next(sizeX), 0f, -sizeZhalf + Random.Next(sizeZ));
				cuber.Init(Random.Next(Startup.Cubers) < Startup.Cubers / Startup.PrecentInfected);
				if (cuber.Infected)
					hasInfected = true;
			}
			// we need at least one infected.
			if (!hasInfected)
				cuber.Init(true);

			Kill(cubers.Pull());
		}


		// kill a cuber and spawn fourths
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

			if (!cuber.Infected)
			{
				// bottom
				f = fourths.Pull();
				fr = r * Quaternion.Euler(0, 0, 0);
				f.transform.position = p + new Vector3(0f, 0f, 0f);
				f.transform.rotation = fr;
				f.Init(color);

				f = fourths.Pull();
				fr = r * Quaternion.Euler(0, 180, 0);
				f.transform.position = p + new Vector3(0f, 0f, 0f);
				f.transform.rotation = fr;
				f.Init(color);
			}

			// top
			f = fourths.Pull();
			fr = r * Quaternion.Euler(180, 0, 0);
			f.transform.position = p + new Vector3(0f, 1f, 0f);
			f.transform.rotation = fr;
			f.Init(color);

			f = fourths.Pull();
			fr = r * Quaternion.Euler(180, 180, 0);
			f.transform.position = p + new Vector3(0f, 1f, 0f);
			f.transform.rotation = fr;
			f.Init(color);
		}

		public void NewEnergy(Vector3 position, float amount)
		{
			var e = energys.Pull();
			e.transform.position = position;
			e.Amount = amount;
		}

		public List<Fourth> GetFourthsInView(Vector3 position)
		{
			return fourths.GetDistance(position, CubersView);
		}
		public List<Energy> GetEnergyInView(Vector3 position)
		{
			return energys.GetDistance(position, CubersView);
		}

		void Update()
		{

		}

		void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale);
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale + new Vector3(2f, 0f, 2f));
		}
	}
}