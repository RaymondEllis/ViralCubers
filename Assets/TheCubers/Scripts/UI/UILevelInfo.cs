using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TheCubers
{
	public class UILevelInfo : UIMenu
	{
		public Text Title;
		public Text Text;
		public Text GoalText;

		// the last known level.
		private string last;
		public string Last
		{
			get
			{
				if (last == null || last.Length == 0)
				{
					last = SceneManager.GetActiveScene().name;
					Debug.Log("Empty last level, got level from Applcation.loadedLevelName :" + last);
				}
				return last;
			}
		}

		public void Play()
		{
			UIBase.Instance.LoadLevel(Last);
		}

		public void GoMe(string level)
		{
			last = level;

			var info = MyFiles.LoadLevelInfo(Last);

			Title.text = info.Title;
			Text.text = info.Description;
			GoalText.text = ScoreInfo.GoalText(Last);

			UIBase.Instance.Go(this);
		}
	}
}
