using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UILevelInfo : UIMenu
	{
		public Text Title;
		public Text Text;

		// the last known level.
		public string Last { get; private set; }

		public void Play()
		{
			UIBase.Instance.LoadLevel(Last);
		}

		public void GoMe(string level)
		{
			Last = level;

			string name = "";
			string text = "";

			switch (level)
			{
				case "world":
					name = "World";
					text = "Level to test stuffs.";
					break;
				case "level1":
					name = "Level 1";
					text = "A beginer level.";
					break;
			}

			Title.text = name;
			Text.text = text;
			UIBase.Instance.Go(this);
		}
	}
}
