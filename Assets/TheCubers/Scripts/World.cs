using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public Cuber PrefabCuber;
	public GameObject PrefabFouth;
	public GameObject PrefabEnergy;

	public startupvar Startup = new startupvar();
	[System.Serializable]
	public struct startupvar
	{
		public int Seed;
		public int Cubers;
		public int PrecentInfected;
	}



	private Cuber[] cubers;
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
		}



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
