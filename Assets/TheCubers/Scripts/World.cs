using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace TheCubers
{
	public class World : MonoBehaviour
	{
		public static World Instance { get { return instance; } }
		public static World instance = null;

		public startupvar Startup = new startupvar();
		[System.Serializable]
		public struct startupvar
		{
			public int Seed;
			[Range(2, 1000)]
			public int Cubers;
			[Range(0, 100)]
			public int PrecentInfected;
		}

		[Header("Energy spawn")]
		[Range(0.01f, 1f)]
		public float EnergyRate;
		[Range(0, 1000)]
		public int EnergyCount;
		[Range(0.01f, 4f)]
		public float EnergyMin;
		[Range(0.01f, 4f)]
		public float EnergyMax;
		private float timerEnergy;

		public Cuber.Global CuberGlobal = new Cuber.Global();
		public Energy.Global EnergyGlobal = new Energy.Global();

		private Pool<Cuber> cubers;
		private Pool<Fourth> fourths;
		private Pool<Energy> energys;

		public System.Random Random { get; private set; }

		private int sizeX, sizeZ;
		private int sizeXhalf, sizeZhalf;

		void Awake()
		{
			if (instance)
			{
				DestroyImmediate(gameObject);
				Debug.LogWarning("Two worlds, HA not anymore!");
			}
			else
				instance = this;
		}

		IEnumerator Start()
		{
			enabled = false;
			Debug.Log("World Start");
			// if no pools wait for them.
			if (!PoolBase.Instance)
			{
				Debug.Log("Waiting for PoolBase instance.");
				while (!PoolBase.Instance)
				{
					yield return null;
				}
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

			// get pools
			PoolBase pools = PoolBase.Instance;
			cubers = pools.Cubers;
			fourths = pools.Fourths;
			energys = pools.Energys;


			// spawn cubers
			int infected = 0;
			if (Startup.PrecentInfected > 0)
			{
				infected = (int)((float)Startup.Cubers * ((float)Startup.PrecentInfected / 100f));
				if (infected < 0)
					infected = 1;
			}
			for (int i = 0; i < Startup.Cubers; ++i)
			{
				Vector3 position;
				if (FindGround(new Ray(new Vector3(-sizeXhalf + Random.Next(sizeX), 100f, -sizeZhalf + Random.Next(sizeZ)), Vector3.down), out position))
				{
					cubers.Pull().Init(position, --infected >= 0, Color.black);
				}
			}

			// startup energy
			for (int i = 0; i < EnergyCount; ++i)
			{
				NewEnergy(EnergyMin, EnergyMax);
			}
			enabled = true;
		}

		void Update()
		{

			// add more energy
			timerEnergy += Time.deltaTime;
			if (timerEnergy > EnergyRate)
			{
				timerEnergy = 0;
				if (energys.Active() < EnergyCount)
					if (!NewEnergy(EnergyMin, EnergyMax))
						NewEnergy(EnergyMin, EnergyMax);
			}
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
			cubers.Pull().Init(position, infected, color);
			return true;
		}

		/// <summary> new random energy </summary>
		public bool NewEnergy(float min, float max)
		{
			return NewEnergy(new Vector3(-sizeXhalf + (float)Random.NextDouble() * sizeX, 0f, -sizeZhalf + (float)Random.NextDouble() * sizeZ), min - (float)Random.NextDouble() * (max - min));
		}
		public bool NewEnergy(Vector3 position, float amount)
		{
			// check if there is already energy around here.
			if (amount < 0.001f || energys.CheckDistance(position, 0.8f))
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
			if (Physics.Raycast(ray, out info, 1000f, groundLayerMask))
			{
				point = info.point;
				return true;
			}
			point = Vector3.zero;
			Debug.LogWarning("Unable to find ground!");
			return false;
		}
	}
}