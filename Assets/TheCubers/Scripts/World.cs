using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace TheCubers
{
	public class World : MonoBehaviour
	{
		public static World Instance { get { return instance; } }
		private static World instance = null;
		public static IEnumerator WaitInstance() { if (!Instance) { while (!Instance) { yield return null; } } }

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

		public int TotalUserEnergy;
		private int _userEnergy;
		public int UserEnergy
		{
			get { return _userEnergy; }
			set
			{
				if (value == _userEnergy)
					return;
				_userEnergy = value;
				UIBase.Instance.GetMenu<UIGame>().UserEnergy.text = value.ToString("N0");
			}
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

		public System.Random Random { get; private set; }
		private static bool pauseUser, pauseWait;
		public static bool Paused { get { return pauseUser || pauseWait; } }
		public static bool PauseUser { get { return pauseUser; } set { pauseUser = value; } }

		public bool IsMenu = false;
		public Transform Sun;

		private Pool<Cuber> cubers;
		private Pool<Fourth> fourths;
		private Pool<Energy> energys;

		private int sizeX, sizeZ;
		private int sizeXhalf, sizeZhalf;

		void Awake()
		{
			if (instance)
			{
				Destroy(gameObject);
				Debug.LogError("There is already a instance of this object, HA not anymore!");
			}
			else
				instance = this;
		}

		IEnumerator Start()
		{
			pauseUser = false;
			pauseWait = true;
			enabled = false;
			Debug.Log("World begin start");
			// if no pools wait for them.
			if (!PoolBase.Instance)
			{
				Debug.Log("Waiting for PoolBase instance.");
				while (!PoolBase.Instance)
				{
					yield return null;
				}
			}
			// wait for UI.
			yield return StartCoroutine(UIBase.WaitInstance());

			UserEnergy = TotalUserEnergy;

			Random = new System.Random(Startup.Seed);

			sizeX = (int)transform.localScale.x;
			sizeXhalf = sizeX / 2;
			sizeZ = (int)transform.localScale.z;
			sizeZhalf = sizeZ / 2;
			if ((float)sizeX != transform.localScale.x || (float)sizeZ != transform.localScale.z)
			{
				Debug.LogWarning("World local scale should be a interger.");
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
				if (infected <= 0)
					infected = 1;
			}
			for (int i = 0; i < Startup.Cubers; ++i)
			{
				Vector3 position;
				if (FindGround(new Ray(new Vector3(-sizeXhalf + Random.Next(sizeX), 100f, -sizeZhalf + Random.Next(sizeZ)), Vector3.down), out position))
				{
					cubers.Pull().Init(position, --infected >= 0, 0.5f + (float)Random.NextDouble() * 0.6f);//, Color.black);
				}
			}

			// startup energy
			for (int i = 0; i < EnergyCount; ++i)
			{
				NewEnergy(EnergyMin, EnergyMax);
			}
			enabled = true;
			pauseWait = false;
			Debug.Log("World end start");
		}

		void Update()
		{
			if (World.Paused)
				return;

			UpdateScore();

			// add more energy
			timerEnergy += Time.deltaTime;
			if (timerEnergy > EnergyRate)
			{
				timerEnergy = 0;
				if (energys.Active() < EnergyCount)
					if (!NewEnergy(EnergyMin, EnergyMin))
						NewEnergy(EnergyMin, EnergyMin);
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale);
			Gizmos.DrawWireCube(transform.position + new Vector3(0f, transform.localScale.y * 0.5f, 0f), transform.localScale + new Vector3(2f, 0f, 2f));
		}


		private int lastScore;
		public void UpdateScore()
		{
			if (IsMenu)
				return;

			int fourth = 1000;
			float energy = 10f;
			int life = 5;

			int score = 0;
			int infected = 0;
			int clean = 0;

			score += fourths.Active() * fourth;

			var c = cubers.GetActive();
			int tmp;
			for (int i = 0; i < c.Count; ++i)
			{
				tmp = 0;
				tmp += c[i].Fourths * fourth;
				tmp += 4 * fourth;
				tmp += (int)(c[i].Energy * energy / 5f) * 5;
				tmp += c[i].Life * life;

				score += tmp * (c[i].Infected ? -1 : 1);
				if (c[i].Infected)
					++infected;
				else
					++clean;
			}

			if (score != lastScore)
			{
				lastScore = score;
				UIGame menu = (UIGame)UIBase.Instance.GetMenu("Game");
				menu.Score.text = score.ToString("N0");
			}

			if (clean == 0 || infected == 0)
			{
				UIGameOver menu = UIBase.Instance.GetMenu<UIGameOver>();
				menu.Fill(clean > infected, score);

				UIBase.Instance.Go(menu);
				pauseUser = false;
				pauseWait = true;
			}
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
			//f.Rigidbody.AddForceAtPosition(Vector3.up * 2f, cuber.transform.position, ForceMode.Impulse);

			f.Init();
			//if (cuber.Infected)
			//	f.Init(Color.white);
			//else
			//	f.Init(cuber.Mesh.material.color);
		}

		public bool NewCuber(Vector3 position, bool infected, Color color)
		{
			cubers.Pull().Init(position, infected, 1f);//, color);
			return true;
		}

		/// <summary> new random non special energy </summary>
		public bool NewEnergy(float min, float max)
		{
			return NewEnergy(new Vector3(-sizeXhalf + (float)Random.NextDouble() * sizeX, 0f, -sizeZhalf + (float)Random.NextDouble() * sizeZ), min + (float)Random.NextDouble() * (max - min), false);
		}
		public bool NewEnergy(Vector3 position, float amount, bool special)
		{
			// check if there is already energy around here.
			if (amount < 0.001f || energys.CheckDistance(position, 0.8f))
				return false;

			var e = energys.Pull();
			e.transform.position = position;
			e.Init(amount, special);
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

		public Material GetCuberMat(bool infected, int age)
		{
			Color c2 = CuberGlobal.ColorOld;
			// ToDo : Cache materials.
			Material mat;
			int max = CuberGlobal.InitialLife;
			if (infected)
			{
				mat = new Material(CuberGlobal.InfectedBodyMat);
				c2 = CuberGlobal.ColorOldInfected;
				max *= 2;
			}
			else
				mat = new Material(CuberGlobal.BodyMat);

			float val = 1f - (float)(age) / max;
			if (val > 1f) val = 1f;
			if (val < 0f) val = 0f;

			Color c = mat.color;
			//c.r = Mathf.Clamp(c.r + val, 0f, 1f);
			//c.g = Mathf.Clamp(c.g + val, 0f, 1f);
			//c.b = Mathf.Clamp(c.b + val, 0f, 1f);
			c.r = Mathf.Lerp(c.r, c2.r, val);
			c.g = Mathf.Lerp(c.g, c2.g, val);
			c.b = Mathf.Lerp(c.b, c2.b, val);
			mat.color = c;

			return mat;
		}
	}
}