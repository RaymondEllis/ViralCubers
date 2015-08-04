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

			var infos = MyFiles.LoadLevelInfo();

			LevelInfo info = new LevelInfo();
			for (int i = 0; i < infos.Length; ++i)
			{
				if (infos[i].Level == Last)
				{
					info = infos[i];
					break;
				}
			}

			Title.text = info.Title;
			Text.text = info.Description;

			UIBase.Instance.Go(this);
		}
	}
}
