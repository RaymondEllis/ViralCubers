using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UIGameOver : UIMenu
	{
		public Text CubersWin;
		public Text VirusWin;
		public Text Score;

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
			Profile pro = ui.GetMenu<UIProfiles>().Current;
			int level = ui.GetMenu<UIRegion>().GetLevelIndex(last);
			if (level == pro.Completed + 1)
			{
				var infos = MyFiles.LoadLevelInfo();
				for (int i = 0; i < infos.Length; ++i)
				{
					var info = infos[i];
					if (info.Level == last)
					{
						bool flag = false;
						switch (info.GoalType)
						{
							case 0: // more
								if (score > info.GoalScore)
									flag = true;
								break;
							case 1: // equal
								if (score == info.GoalScore)
									flag = true;
								break;
							case 2: // less
								if (score < info.GoalScore)
									flag = true;
								break;
						}
						if (flag)
						{
							pro.Completed = level;
							ui.GetMenu<UIProfiles>().SaveProfiles();
						}
						break;
					}
				}
			}


			// fill the text boxes.
			CubersWin.gameObject.SetActive(cubersWin);
			VirusWin.gameObject.SetActive(!cubersWin);

			Score.text = score.ToString("N0");
		}
	}
}
