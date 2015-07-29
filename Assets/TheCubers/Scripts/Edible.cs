using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public abstract class Edible : MonoBehaviour
	{
		int count;
		int wanted;
		bool canWant;
		int consumed;


		protected void initEdible(int count, bool canWant)
		{
			this.count = count;
			this.canWant = canWant;
			wanted = 1;
			consumed = 0;
		}

		protected void countEdible(int count)
		{
			this.count = count;
		}

		public int Wanted { get { return wanted; } }
		public void Want()
		{
			if (canWant)
				++wanted;
		}

		public void Consume()
		{
			--wanted;
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