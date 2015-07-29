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
			public AnimationCurve VisualCurve;
			public float VisualDevider;
			public float VisualMax;
		}
		public float Amount;
		private float lastAmount;
		private bool special;

		public Transform LookAtSun;

		public void Init(float amount)
		{
			Amount = amount;
			lastAmount = Amount;
			special = amount == 10f;

			if (special)
				initEdible(10, false);
			else
				initEdible(1, true);

			updateVisuals();
		}

		protected override void OnPartialConsumed()
		{
			--Amount;
		}

		protected override void OnUpdate()
		{
			if (!special)
			{
				Global g = World.Instance.EnergyGlobal;
				Amount += g.Grow * Time.deltaTime;
				if (Amount > g.Max)
					Amount = g.Max;
			}

			if (Amount != lastAmount)
			{
				lastAmount = Amount;
				countEdible(1);
				updateVisuals();
			}
		}

		private void updateVisuals()
		{
			Global g = World.Instance.EnergyGlobal;
			float v = g.VisualCurve.Evaluate(Amount / g.VisualDevider) * g.VisualMax;
			transform.localScale = new Vector3(v, v, v);

			if (LookAtSun && World.Instance.Sun)
				LookAtSun.LookAt(World.Instance.Sun);
		}
	}
}