using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TheCubers
{
	public class UIBase : MonoBehaviour
	{
		public static UIBase Instance { get { return instance; } }
		private static UIBase instance = null;
		public static IEnumerator WaitInstance()
		{
			if (!Instance)
			{
				Debug.Log("Waiting for UIBase instance.");
				while (!Instance)
				{ yield return null; }
			}
		}

		private List<UIMenu> menus;
		private UIMenu back;
		private bool backSize = false;
		private float backPos;
		private Vector2 backFrom;

		private UIMenu active = null;

		void Awake()
		{
			if (instance)
			{
				Destroy(gameObject);
				Debug.LogError("There is already a instance of this object, HA not anymore!");
			}
			else
				instance = this;

			// get all the menus
			menus = new List<UIMenu>();
			Transform child;
			UIMenu menu;
			for (int i = 0; i < transform.childCount; ++i)
			{
				child = transform.GetChild(i);
				if (child)
				{
					menu = child.GetComponent<UIMenu>();
					if (menu)
					{
						menu.Init();
						menus.Add(menu);
						if (menu.name == "Background")
							back = menu;
					}
				}
			}
		}

		void Start()
		{
			DontDestroyOnLoad(gameObject);

			goDefaultMenu(Application.loadedLevelName);
		}

		void Update()
		{
			for (int i = 0; i < menus.Count; ++i)
				menus[i].UpdateUI();

			// update background size
			if (backSize)
			{
				backPos += active.speed * Time.deltaTime;
				back.transform.sizeDelta = Vector2.Lerp(backFrom, active.transform.sizeDelta, active.SlideCurve.Evaluate(backPos));
				if (back.transform.sizeDelta == active.transform.sizeDelta)
					backSize = false;
			}
		}

		/// <summary>
		/// Return menu given the name.
		/// </summary>
		public UIMenu GetMenu(string name)
		{
			for (int i = 0; i < menus.Count; ++i)
				if (menus[i].name == name)
					return menus[i];
			return null;
		}

		public void Go(string name)
		{
			UIMenu menu = GetMenu(name);
			if (menu)
				Go(menu);
			else
				Debug.LogWarning("Unable to find menu: " + name);
		}
		public void Go(UIMenu menu)
		{
			if (active)
				active.Close();
			active = menu;
			active.Open();

			// show background and set size
			back.Set(active.name != "Game");
			if (back.transform.sizeDelta != active.transform.sizeDelta)
			{
				backSize = true;
				backPos = 0f;
				backFrom = back.transform.sizeDelta;
			}
		}

		private void goDefaultMenu(string levelName)
		{
			switch (levelName)
			{
				case "startup":
					break;

				case "menu":
				case "base":
					Go("Start");
					break;

				default:
					Go("Game");
					break;
			}
		}

		public void LoadLevel(string level)
		{
			StartCoroutine(loadLevel(level));
		}
		private IEnumerator loadLevel(string level)
		{
			yield return Application.LoadLevelAsync(level);
			goDefaultMenu(level);
		}

		public void Exit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
