using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public abstract class Edible : MonoBehaviour
	{
		int count;
		int reserved;
		int consumed;


		protected void initEdible(int count)
		{
			this.count = count;
			reserved = 0;
			consumed = 0;
		}

		protected void countEdible(int count)
		{
			this.count = count;
		}

		public bool CanReserve { get { return reserved + consumed < count; } }

		public bool Reserve()
		{
			if (!CanReserve)
				return false;
			++reserved;
			return true;
		}

		public void Consume()
		{
			--reserved;
			++consumed;
			if (consumed >= count)
				OnConsumed();
			else
				OnPartialConsumed();
		}

		protected virtual void OnPartialConsumed() { }

		protected virtual void OnConsumed()
		{
			gameObject.SetActive(false);
		}

		void Update()
		{
			if (World.Paused || (consumed >= count && count > 0))
				return;

			OnUpdate();
		}

		protected abstract void OnUpdate();

	}
}