using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UIGameOver : UIMenu
	{
		public Text CubersWin;
		public Text VirusWin;
		public Text Score;

		public void Fill(bool cubersWin, int score)
		{
			CubersWin.gameObject.SetActive(cubersWin);
			VirusWin.gameObject.SetActive(!cubersWin);

			Score.text = score.ToString("N0");
		}
	}
}
