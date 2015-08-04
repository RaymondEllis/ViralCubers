using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UIRegion : UIMenu
	{
		public Transform Layout;
		public GameObject Button;

		//public string[] Levels;
		public Sprite[] Levels;
		private Button[] buttons;

		protected override void OnInit()
		{
			load();
		}

#if UNITY_EDITOR || DEBUG
		protected override void OnOpenStart()
		{
			buttons = null;
			for (int i = 0; i < Layout.childCount; ++i)
				Destroy(Layout.GetChild(i).gameObject);

			load();
		}
#endif

		private void load()
		{
			var infos = UILevelInfo.Info.Get();
			buttons = new Button[infos.Length];

			for (int i = 0; i < infos.Length; ++i)
			{
				var info = infos[i];

				// instantiate the button
				GameObject obj = Instantiate(Button);
				obj.transform.SetParent(Layout);
				obj.transform.localScale = Vector3.one;

				// find and set sprite
				for (int j = 0; j < Levels.Length; ++j)
				{
					if (Levels[j].name == info.Level)
					{
						var sp = obj.GetComponent<Image>();
						sp.sprite = Levels[j];
						break;
					}
				}

				// set button text
				obj.GetComponentsInChildren<Text>(true)[0].text = info.Title;

				// button on click
				var btn = obj.GetComponent<Button>();
				buttons[i] = btn;
				btn.onClick.AddListener(() =>
				{
					var menu = UIBase.Instance.GetMenu<UILevelInfo>();
					menu.GoMe(info.Level);
				});
			}
		}
	}
}
