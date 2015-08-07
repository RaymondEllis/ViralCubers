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
		public int PortionsLeft { get { return count - consumed; } }
		public bool Consumed { get { return PortionsLeft <= 0; } }

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
			if (canWant)
				--wanted;
			++consumed;
			//OnPortionConsumed();
			if (consumed >= count)
				OnConsumed();
		}

		//protected virtual void OnPortionConsumed() { }

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