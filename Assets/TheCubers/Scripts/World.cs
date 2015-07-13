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

		public Cuber.Global CuberGlobal = new Cuber.Global();

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


			// spawn cubers
			bool hasInfected = false;
			Cuber cuber = null;
			for (int i = 0; i < Startup.Cubers; ++i)
			{
				cuber = cubers.Pull();
				cuber.transform.position = new Vector3(-sizeXhalf + Random.Next(sizeX), 0f, -sizeZhalf + Random.Next(sizeZ));
				cuber.Init(Random.Next(Startup.Cubers) < Startup.Cubers / Startup.PrecentInfected, Color.black);
				if (cuber.Infected)
					hasInfected = true;
			}
			// we need at least one infected.
			if (!hasInfected)
				cuber.Init(true, Color.black);

			Kill(cubers.Pull());

			// some energy
			for (int i = 0; i < 100; ++i)
			{
				NewEnergy(new Vector3(-sizeXhalf + Random.Next(sizeX), 0f, -sizeZhalf + Random.Next(sizeZ)), (float)Random.NextDouble());
			}
		}

		void Update()
		{

		}

		void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale);
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale + new Vector3(2f, 0f, 2f));
		}



		// kill a cuber and spawn fourths
		public void Kill(Cuber cuber)
		{
			cuber.gameObject.SetActive(false);

			if (!cuber.Infected)
			{
				// bottom
				dropForth(cuber, Quaternion.Euler(0, 0, 0), new Vector3(0f, 0f, 0f));
				dropForth(cuber, Quaternion.Euler(0, 180, 0), new Vector3(0f, 0f, 0f));
			}

			// top
			dropForth(cuber, Quaternion.Euler(180, 0, 0), new Vector3(0f, 1f, 0f));
			dropForth(cuber, Quaternion.Euler(180, 180, 0), new Vector3(0f, 1f, 0f));

			// drop any fourths cuber was carrying.
			if (cuber.Fourths > 0)
				dropForth(cuber, Quaternion.Euler(0, 0, 0), new Vector3(0f, 2f, 0f));
			if (cuber.Fourths > 1)
				dropForth(cuber, Quaternion.Euler(0, 180, 0), new Vector3(0f, 2f, 0f));
			if (cuber.Fourths > 2)
				dropForth(cuber, Quaternion.Euler(180, 0, 0), new Vector3(0f, 3f, 0f));
			if (cuber.Fourths > 3)
				dropForth(cuber, Quaternion.Euler(180, 180, 0), new Vector3(0f, 3f, 0f));

		}
		private void dropForth(Cuber cuber, Quaternion rotation, Vector3 position)
		{
			Fourth f = fourths.Pull();
			f.transform.position = cuber.transform.position + position;
			f.transform.rotation = cuber.transform.rotation * rotation;

			if (cuber.Infected)
				f.Init(Color.white);
			else
				f.Init(cuber.Mesh.material.color);
		}

		public bool NewCuber(Vector3 position, bool infected, Color color)
		{
			var cuber = cubers.Pull();
			cuber.transform.position = position;
			cuber.Init(infected, color);
			return true;
		}

		public bool NewEnergy(Vector3 position, float amount)
		{
			// check if there is already energy around here.
			if (energys.CheckDistance(position, 1f))
				return false;

			var e = energys.Pull();
			e.transform.position = position;
			e.transform.localScale = new Vector3(amount, amount, amount);
			e.Amount = amount;
			return true;
		}

		public List<Fourth> GetFourthsInView(Vector3 position)
		{
			return fourths.GetDistance(position, CuberGlobal.View);
		}
		public List<Energy> GetEnergyInView(Vector3 position)
		{
			return energys.GetDistance(position, CuberGlobal.View);
		}

		private static int groundLayerMask;
		public static bool FindGround(Ray ray, out Vector3 point)
		{
			if (groundLayerMask == 0)
				groundLayerMask = LayerMask.GetMask("ValidGround");

			RaycastHit info;
			if (Physics.Raycast(ray, out info, 100f, groundLayerMask))
			{
				point = info.point;
				return true;
			}
			point = Vector3.zero;
			return false;
		}
	}
}