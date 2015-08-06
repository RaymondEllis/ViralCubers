using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UIGameOver : UIMenu
	{
		public Text CubersWin;
		public Text VirusWin;
		public Text Score;
		public Text EndText;

		public override bool GoBack()
		{
			return true;
		}

		public void Fill(bool cubersWin, int score)
		{
			var ui = UIBase.Instance;
			// add score to leaderboard.
			ui.GetMenu<UILeaderboards>().AddScore(score);

			// update current profile if we compleated the goal.
			var last = ui.GetMenu<UILevelInfo>().Last;

			if (ScoreInfo.Test(last, cubersWin, score))
			{
				Profile pro = ui.GetMenu<UIProfiles>().Current;
				int level = ui.GetMenu<UIRegion>().GetLevelIndex(last);
				if (level == pro.Completed + 1)
				{
					pro.Completed = level;
					ui.GetMenu<UIProfiles>().SaveProfiles();
					EndText.text = "Congratulations you unlocked a new level!";
				}
				else
					EndText.text = "Congratulations you finished!";
			}
			else
				EndText.text = ScoreInfo.FailText(last, cubersWin, score);


			// fill the text boxes.
			CubersWin.gameObject.SetActive(cubersWin);
			VirusWin.gameObject.SetActive(!cubersWin);

			Score.text = score.ToString("N0");
		}
	}
}
