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
			UIBase.Instance.GetMenu<UILeaderboards>().AddScore(score);

			CubersWin.gameObject.SetActive(cubersWin);
			VirusWin.gameObject.SetActive(!cubersWin);

			Score.text = score.ToString("N0");
		}
	}
}
