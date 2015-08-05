using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UILevelInfo : UIMenu
	{
		public Text Title;
		public Text Text;
		public Text GoalText;

		// the last known level.
		public string Last { get; private set; }

		public void Play()
		{
			UIBase.Instance.LoadLevel(Last);
		}

		public void GoMe(string level)
		{
			Last = level;

			var info = MyFiles.LoadLevelInfo(Last);

			Title.text = info.Title;
			Text.text = info.Description;
			GoalText.text = ScoreInfo.GoalText(Last);

			UIBase.Instance.Go(this);
		}
	}
}
