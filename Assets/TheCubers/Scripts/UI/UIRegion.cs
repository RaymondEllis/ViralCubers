using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UIRegion : UIMenu
	{
		public Transform Layout;
		public GameObject Button;

		public Sprite[] Levels;
		private ButtonLevel[] buttons;

		protected override void OnInit()
		{
			load();
		}

		public override bool GoBack()
		{
			UIBase.Instance.Go("Start");
			return true;
		}

		protected override void OnOpenStart()
		{
#if UNITY_EDITOR || DEBUG
			buttons = null;
			for (int i = 0; i < Layout.childCount; ++i)
				Destroy(Layout.GetChild(i).gameObject);

			LastSelected = null;
			load();
#endif

			var pro = UIBase.Instance.GetMenu<UIProfiles>().Current;
			if (pro == null)
			{
				UIBase.Instance.Go<UIProfiles>();
				return;
			}

			bool interact;
			for (int i = 0; i < buttons.Length; ++i)
			{
				interact = i <= pro.Completed + 1;
				buttons[i].Setinteractable(interact);
				if (interact)
					FirstSelected = buttons[i].Button;
			}
		}

		public int GetLevelIndex(string level)
		{
			var infos = MyFiles.LoadLevelInfos();
			for (int i = 0; i < infos.Length; ++i)
				if (infos[i].Level == level)
					return i;
			Debug.LogError("Unable to find index for level: " + level);
			return -1;
		}

		private void load()
		{
			var infos = MyFiles.LoadLevelInfos();
			buttons = new ButtonLevel[infos.Length];

			for (int i = 0; i < infos.Length; ++i)
			{
				var info = infos[i];

				// instantiate the button
				GameObject obj = Instantiate(Button);
				obj.transform.SetParent(Layout);
				obj.transform.localScale = Vector3.one;

				var btn = obj.GetComponent<ButtonLevel>();
				buttons[i] = btn;

				// find and set sprite
				for (int j = 0; j < Levels.Length; ++j)
				{
					if (Levels[j].name == info.Level)
					{
						btn.Image.sprite = Levels[j];
						break;
					}
				}

				// set button text
				btn.text = info.Title;

				// button on click
				btn.Button.onClick.AddListener(() =>
				{
					var menu = UIBase.Instance.GetMenu<UILevelInfo>();
					menu.GoMe(info.Level);
				});
			}
		}
	}
}
