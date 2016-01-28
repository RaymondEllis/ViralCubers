using UnityEngine;
using System;

namespace TheCubers
{
	/// <summary>
	/// Singleton class to create the game pools, stays alive across scene loads.
	/// </summary>
	class PoolBase : MonoBehaviour
	{
		public static PoolBase Instance { get { return instance; } }
		private static PoolBase instance = null;

		public Cuber PrefabCuber = null;
		public Fourth PrefabFouth = null;
		public Energy PrefabEnergy = null;

		public int PreAllocate = 100;
		public int HardMaximum = 100000;
		public PoolResizeMode ResizeMode = PoolResizeMode.Double;
		public int AdditiveValue = 0;

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
				Debug.LogWarning("Missing a prefab!");
			}

			cubers = new Pool<Cuber>(transform, PrefabCuber, PreAllocate, HardMaximum, ResizeMode, AdditiveValue);
			fourths = new Pool<Fourth>(transform, PrefabFouth, PreAllocate, HardMaximum, ResizeMode, AdditiveValue);
			energys = new Pool<Energy>(transform, PrefabEnergy, PreAllocate, HardMaximum, ResizeMode, AdditiveValue);
			createUnloadDummy();
		}

		void OnLevelWasLoaded()
		{
			//Debug.Log("OnLevelWasLoaded");
			createUnloadDummy();
		}

		public void OnDumyDestoryed()
		{
			Debug.Log("Emptying the pools.");
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

	/// <summary>
	/// Dummy object to tell PoolBase when scene is being destroyed.
	/// </summary>
	public class PoolDestroyHelper : MonoBehaviour
	{
		void OnDestroy()
		{
			if (PoolBase.Instance)
				PoolBase.Instance.OnDumyDestoryed();
		}
	}
}