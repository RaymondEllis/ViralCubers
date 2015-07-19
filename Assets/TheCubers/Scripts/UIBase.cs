using UnityEngine;
using System.Collections.Generic;

namespace TheCubers
{
	public class UIBase : MonoBehaviour
	{
		private List<UIMenu> menus;
		private UIMenu back;

		private UIMenu active = null;

		void Awake()
		{
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
					}
				}
			}
			back = GetMenu("Background");
		}

		void Start()
		{
			DontDestroyOnLoad(gameObject);

			if (Application.loadedLevelName == "menu" || Application.loadedLevelName == "base")
				Go("Start");
			else
				Go("Game");
		}

		void Update()
		{
			for (int i = 0; i < menus.Count; ++i)
				menus[i].UpdateUI();
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

			back.Set(active.name != "Game");
			// resize back to size of menu.
			// ToDo  5: lerpy sizes.
			Vector3 s = active.GetComponent<RectTransform>().sizeDelta;
			back.GetComponent<RectTransform>().sizeDelta = s;
		}

		public void LoadLevel(string level)
		{
			if (level == "menu" || level == "base")
				Go("Start");
			else
				Go("Game");

			Application.LoadLevelAsync(level);
		}

		public void Exit()
		{
			Application.Quit();
		}
	}
}
