using UnityEngine;
using UnityEngine.UI;

namespace TheCubers
{
	public class UIGame : UIMenu
	{
		public Text Score;

		public Text LifeValue;
		public Text EnergyValue;

		private int deadLife = 0, deadEnergy = 0;


		protected override void OnOpenStart()
		{
			deadLife = 0;
			deadEnergy = 0;

			updateText();
		}

		public override bool GoBack()
		{
			UIBase.Instance.Pause(true);
			return true;
		}

		public void CountDeath(float energy, int life)
		{
			if (life <= 0 || life < energy)
				++deadLife;
			else
				++deadEnergy;

			updateText();
		}

		private void updateText()
		{

			LifeValue.text = deadLife.ToString("N0");
			EnergyValue.text = deadEnergy.ToString("N0");
		}
	}
}
