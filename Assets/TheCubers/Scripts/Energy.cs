using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public class Energy : MonoBehaviour
	{
		[System.Serializable]
		public struct Global
		{
			public float Grow;
			public float Max;
		}
		public float Amount;

		void Update()
		{
			if (World.Paused)
				return;

			Global g = World.Instance.EnergyGlobal;
			Amount += g.Grow * Time.deltaTime;
			if (Amount > g.Max)
				Amount = g.Max;
			transform.localScale = new Vector3(Amount, Amount, Amount);
		}
	}
}