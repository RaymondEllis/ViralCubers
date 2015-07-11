using UnityEngine;
using System.Collections.Generic;

namespace TheCubers
{
	public enum PoolResizeMode { Fixed, Double }

	public class Pool<T> where T : MonoBehaviour
	{
		private T original;

		private T[] array;
		private PoolResizeMode resizeMode;
		private int resize;

		public Pool(T original, int initialSize) : this(original, initialSize, 0, PoolResizeMode.Double) { }
		public Pool(T original, int initialSize, int resize, PoolResizeMode resizeMode)
		{
			this.original = original;

			array = new T[initialSize];
			fill(0);

			this.resize = resize;
			this.resizeMode = resizeMode;
		}

		public T Get()
		{
			for (int i = 0; i < array.Length; ++i)
			{
				if (!array[i].gameObject.activeSelf)
				{
					array[i].gameObject.SetActive(true);
					return array[i];
				}
			}

			// we need a new item, lets resize the array.
			int startIndex = array.Length;
			switch (resizeMode)
			{
				case PoolResizeMode.Fixed:
					System.Array.Resize<T>(ref array, array.Length + resize);
					break;
				case PoolResizeMode.Double:
					System.Array.Resize<T>(ref array, array.Length * 2);
					break;
			}

			// create the new items.
			fill(startIndex);

			// return our fresh item.
			array[startIndex].gameObject.SetActive(true);
			return array[startIndex];
		}

		private void fill(int start)
		{
			for (int i = start; i < array.Length; ++i)
			{
				array[i] = Object.Instantiate<T>((T)original);
				array[i].name = typeof(T).Name + " " + i;
				array[i].gameObject.SetActive(false);
			}
		}
	}
}
