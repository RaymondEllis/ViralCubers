using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public class Energy : Edible
	{
		[System.Serializable]
		public struct Global
		{
			public float Grow;
			public float Max;
		}
		public float Amount;
		private float lastAmount;

		public Transform LookAtSun;

		public void Init(float amount)
		{
			initEdible();

			Amount = amount;
			lastAmount = -1f;

			updateVisuals();
		}

		protected override void OnUpdate()
		{
			Global g = World.Instance.EnergyGlobal;
			Amount += g.Grow * Time.deltaTime;
			if (Amount > g.Max)
				Amount = g.Max;

			if (Amount != lastAmount)
			{
				lastAmount = Amount;
				updateVisuals();
			}
		}

		private void updateVisuals()
		{
			transform.localScale = new Vector3(Amount, Amount, Amount);

			if (LookAtSun && World.Instance.Sun)
				LookAtSun.LookAt(World.Instance.Sun);
		}
	}
}