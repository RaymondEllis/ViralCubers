using UnityEngine;
using UnityEngine.UI;
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

		public GameObject Crosshair;

		public static void Select(Selectable obj) { obj.Select(); }

		private List<UIMenu> menus;
		private UIMenu back;
		private bool backSize = false;
		private float backPos;
		private Vector2 backFrom;

		private UIMenu active = null;
		public UIMenu Active { get { return active; } }
		private UIMenu last = null;

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
			// update menus
			for (int i = 0; i < menus.Count; ++i)
				menus[i].UpdateUI();

			// background size transition
			if (backSize)
			{
				backPos += active.speed * Time.deltaTime;
				back.transform.sizeDelta = Vector2.Lerp(backFrom, active.transform.sizeDelta, active.SlideCurve.Evaluate(backPos));
				if (back.transform.sizeDelta == active.transform.sizeDelta)
					backSize = false;
			}

			// select up or down if tab or tab + shift
			if (!(active is UIGame) && Input.GetKeyDown(KeyCode.Tab))
			{
				var e = UnityEngine.EventSystems.EventSystem.current;
				Selectable s = e.currentSelectedGameObject.GetComponent<Selectable>();
				if (s)
				{
					// ToDo 99: wrap around tab select. Selectable.allSelectables looks usefull
					Selectable next;
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
						next = s.FindSelectableOnUp();
					else
						next = s.FindSelectableOnDown();

					if (next)
						next.Select();
				}
			}

			// pause
			if (MyInput.GetDown(Inp.Pause))
			{
				Pause(!World.PauseUser);
			}

			// back
			if (Input.GetButtonDown("Cancel"))
			{
				GoBack();

				//if (active && active is UIRegion)
				//	Go("Start");
				//if (active && active is UIGame)
				//	Pause(true);
				//else if (World.PauseUser)
				//	Pause(false);
				//else
				//	GoBack();
			}

			// show and hide the crosshair
			if (World.Paused || active is UIGame == false)
				Crosshair.SetActive(false);
			else
				Crosshair.SetActive(!MouseInScreen());

		}

		public static bool MouseInScreen()
		{
			return Input.mousePresent && new Rect(0, 0, Screen.width, Screen.height).Contains(Input.mousePosition);
		}

		/// <summary> Return menu given the name </summary>
		public UIMenu GetMenu(string name)
		{
			for (int i = 0; i < menus.Count; ++i)
				if (menus[i].name == name)
					return menus[i];
			Debug.LogWarning("Unable to find menu: " + name);
			return null;
		}
		/// <summary> Return menu given type </summary>
		public T GetMenu<T>() where T : UIMenu
		{
			for (int i = 0; i < menus.Count; ++i)
				if (menus[i] is T)
					return (T)menus[i];
			Debug.LogWarning("Unable to find menu: <" + typeof(T).Name + ">");
			return default(T);
		}

		public void Go(string name)
		{
			Go(GetMenu(name));
		}
		public void Go<T>() where T : UIMenu
		{
			Go(GetMenu<T>());
		}
		public void Go(UIMenu menu)
		{
			if (!menu)
				return;
			if (active)
				active.Close();
			last = active;
			active = menu;
			active.Open();

			// show background and set size
			back.Set(active is UIGame == false);
			if (back.transform.sizeDelta != active.transform.sizeDelta)
			{
				backSize = true;
				backPos = 0f;
				backFrom = back.transform.sizeDelta;
			}
		}

		/// <summary> Goes to last active, or default menu if no last. </summary>
		public void GoBack()
		{
			if (World.PauseUser)
			{
				Pause(false);
				return;
			}

			if (active && active.GoBack())
				return;

			if (last)
				Go(last);
			else
				goDefaultMenu(Application.loadedLevelName);
		}

		private void goDefaultMenu(string levelName)
		{
			switch (levelName)
			{
				case "startup":
					break;

				case "menu":
				case "base":
					{
						UIProfiles menu = GetMenu<UIProfiles>();
						if (menu.HasCurrent)
							Go("Start");
						else
							Go(menu);
					}
					break;

				default:
					Go<UIGame>();
					break;
			}
		}

		public void ReloadLevel()
		{
			LoadLevel(Application.loadedLevelName);
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

		/// <summary> pause or unpause the game </summary>
		public void Pause(bool pause)
		{
			if (!(active is UIGame || World.PauseUser))
				return;

			World.PauseUser = pause;
			if (pause)
				Go("Paused");
			else
				Go<UIGame>();

		}

		public void EndGame()
		{
			World.PauseUser = false;
			World.Instance.IsMenu = true;
			Destroy(FindObjectOfType<PlayerControl>().gameObject);
			Go<UIRegion>();
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
