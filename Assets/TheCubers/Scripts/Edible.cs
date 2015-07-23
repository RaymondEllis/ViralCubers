using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public abstract class Edible : MonoBehaviour
	{
		public bool Eaten;

		protected void initEdible()
		{
			Eaten = false;
		}

		void Update()
		{
			if (World.Paused || Eaten)
				return;

			OnUpdate();
		}

		protected abstract void OnUpdate();
	}
}