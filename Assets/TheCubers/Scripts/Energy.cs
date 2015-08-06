using UnityEngine;
using System.Collections;

namespace TheCubers
{
	[SelectionBase]
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

		public void Init(float amount, bool special)
		{
			Amount = amount;
			lastAmount = Amount;
			this.special = special;

			if (special)
				initEdible(10, false);
			else
				initEdible(1, true);

			updateVisuals();
		}

		public float TakePortion()
		{
			if (Amount > 1f && PortionsLeft > 1)
			{
				Amount -= 1f;
				return 1f;
			}
			else
			{
				float result = Amount;
				Amount = 0f;
				return result;
			}
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
				updateVisuals();
			}
		}

		protected override void OnConsumed()
		{
			if (special)
				++World.Instance.UserEnergy;
			base.OnConsumed();
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