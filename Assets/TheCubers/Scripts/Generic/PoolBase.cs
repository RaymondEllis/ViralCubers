using UnityEngine;
using System;

namespace TheCubers
{
	/// <summary>
	/// Class to create the game pools
	/// </summary>
	class PoolBase : MonoBehaviour
	{
		public static PoolBase Instance { get { return instance; } }
		private static PoolBase instance = null;

		public Cuber PrefabCuber = null;
		public Fourth PrefabFouth = null;
		public Energy PrefabEnergy = null;

		public int InitialCount = 100;

		private Pool<Cuber> cubers;
		private Pool<Fourth> fourths;
		private Pool<Energy> energys;

		public Pool<Cuber> Cubers { get { return cubers; } }
		public Pool<Fourth> Fourths { get { return fourths; } }
		public Pool<Energy> Energys { get { return energys; } }

		void Awake()
		{
			if (instance)
				Debug.LogError("There should be only one pool base!");
			instance = this;

			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
		}

		void Start()
		{
			DontDestroyOnLoad(gameObject);

			if (!PrefabCuber || !PrefabEnergy || !PrefabFouth)
			{
				Debug.LogError("Missing a prefab!");
			}

			cubers = new Pool<Cuber>(transform, PrefabCuber, InitialCount);
			fourths = new Pool<Fourth>(transform, PrefabFouth, InitialCount);
			energys = new Pool<Energy>(transform, PrefabEnergy, InitialCount);
			createUnloadDummy();
		}

		void OnLevelWasLoaded()
		{
			Debug.Log("OnLevelWasLoaded");
			createUnloadDummy();
		}

		void OnDisable()
		{
			Debug.Log("OnDisable");
		}

		public void OnDumyDestoryed()
		{
			Debug.Log("OnDumyDestoryed");
			cubers.DisableAll();
			fourths.DisableAll();
			energys.DisableAll();
		}

		private void createUnloadDummy()
		{
			GameObject dummy = new GameObject("UnloadDummy");
			dummy.AddComponent<PoolDestroyHelper>();
		}
	}

	public class PoolDestroyHelper : MonoBehaviour
	{
		void Start() { }
		void OnDestroy()
		{
			PoolBase.Instance.OnDumyDestoryed();
		}
	}
}