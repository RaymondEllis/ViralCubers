﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace TheCubers
{
	public class UIProfiles : UIMenu
	{
		public Text Title;
		public InputField NewInput;
		public Button Cancel;

		public RectTransform ProfileParent;
		public GameObject ProfileStarter;

		private List<Profile> profiles;
		private List<GameObject> profilesUI;

		private int _current;
		public bool HasCurrent { get { return profiles != null && _current >= 0 && _current < profiles.Count; } }
		public string CurrentName
		{
			get
			{
				if (HasCurrent)
					return profiles[_current].Name;
				else
					return "N/A";
			}
		}
		public Profile Current { get { if (!HasCurrent) return null; return profiles[_current]; } }

		protected override void OnInit()
		{
			ProfileStarter.SetActive(false);

			if (!load())
				profiles = new List<Profile>();


			profilesUI = new List<GameObject>();
			if (profiles.Count > 0)
			{
				addProfileUI(ProfileStarter, 0);
				for (int i = 1; i < profiles.Count; ++i)
				{
					GameObject obj = Instantiate(ProfileStarter);
					addProfileUI(obj, i);
				}
			}
		}

		protected override void OnOpenStart()
		{
			// loading is done OnInit

			// no cancel button if there is no current profile.
			Cancel.gameObject.SetActive(HasCurrent);

			if (HasCurrent)
				Title.text = "Select a profile";
			else
				Title.text = "Create a profile";

			NewInput.text = "";

			if (profiles.Count > 0)
			{
				if (HasCurrent)
					FirstSelected = profilesUI[_current].GetComponentInChildren<Button>();
				else
					FirstSelected = profilesUI[0].GetComponentInChildren<Button>();
			}
			else
				NewInput.Select();
		}

		protected override void OnCloseStart()
		{
			if (HasCurrent)
				profiles[_current].LastUsed = DateTime.Now.ToUniversalTime();
			SaveProfiles();
		}

		/// <summary> returns false if there is no profiles </summary>
		private bool load()
		{
			profiles = MyFiles.LoadProfiles();

			if (profiles == null || profiles.Count == 0)
				return false;

			DateTime last = new DateTime(0);
			for (int i = 0; i < profiles.Count; ++i)
			{
				if (profiles[i].LastUsed > last)
				{
					last = profiles[i].LastUsed;
					_current = i;
				}
			}
			return true;
		}

		public void SaveProfiles()
		{
			MyFiles.SaveProfiles(profiles);
		}

		private void addProfileUI(GameObject obj, int index)
		{
			var btn = obj.GetComponentsInChildren<Button>(true);
			if (btn.Length != 2 || btn[1].name != "Button Delete")
				Debug.LogError("Buttons messed up, see this method.");

			btn[0].onClick.RemoveAllListeners();
			btn[1].onClick.RemoveAllListeners();

			// set name on button text
			var txt = btn[0].GetComponentsInChildren<Text>(true);
			if (!txt[0])
				Debug.LogError("Profile button should have a child Text!");
			txt[0].text = profiles[index].Name;
			obj.name = "Profile " + profiles[index].Name;

			// on profile clicked
			btn[0].onClick.AddListener(() =>
			{
				_current = index;
				UIBase.Instance.Go("Start");
			});

			// on delete clicked
			btn[1].onClick.AddListener(() =>
			{
				if (_current == index)
				{
					_current = -1;
					Cancel.gameObject.SetActive(HasCurrent);
				}

				profiles.RemoveAt(index);
				profilesUI.RemoveAt(index);

				if (ReferenceEquals(ProfileStarter, obj))
					obj.SetActive(false);
				else
					Destroy(obj);
			});

			obj.transform.SetParent(ProfileParent);
			obj.transform.localScale = Vector3.one;
			obj.SetActive(true);
			profilesUI.Add(obj);
		}

		public void NewProfile(InputField name) { NewProfile(name.text); }
		public void NewProfile(string name)
		{
			int index = profiles.Count;
			profiles.Add(new Profile() { Name = name, LastUsed = DateTime.Now, Completed = 1 });

			GameObject obj;
			if (profiles.Count == 0)
				obj = ProfileStarter;
			else
				obj = Instantiate(ProfileStarter);

			addProfileUI(obj, index);

			_current = index;
			UIBase.Instance.Go("Start");
		}
	}
}
