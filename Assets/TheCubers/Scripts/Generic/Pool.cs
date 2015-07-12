using UnityEngine;
using System.Collections.Generic;

namespace TheCubers
{
	public class Pool<T> where T : MonoBehaviour
	{
		public enum ResizeMode { Fixed, Double }

		private GameObject parent;
		private T original;

		private T[] array;
		private ResizeMode resizeMode;
		private int resize;
		public int Max { get; private set; }
		public int Length { get { return array.Length; } }

		private string typeName = typeof(T).Name;

		public Pool(T original, int initialSize) : this(original, initialSize, int.MaxValue, ResizeMode.Double, 0) { }
		public Pool(T original, int initialSize, int max) : this(original, initialSize, max, ResizeMode.Double, 0) { }
		public Pool(T original, int initialSize, int max, ResizeMode resizeMode, int resize)
		{
			parent = new GameObject("Pool<" + typeName + ">");
			this.original = original;

			array = new T[initialSize];
			fill(0);

			this.Max = max;
			this.resize = resize;
			this.resizeMode = resizeMode;

		}

		/// <summary> Gets number of active objects. </summary>
		public int Active()
		{
			int a = 0;
			for (int i = 0; i < array.Length; ++i)
				if (array[i].gameObject.activeSelf)
					++a;
			return a;
		}

		/// <summary> True if an item is within distance. </summary>
		public bool CheckDistance(Vector3 position, float distance)
		{
			for (int i = 0; i < array.Length; ++i)
				if (array[i].gameObject.activeSelf && Vector3.Distance(array[i].transform.position, position) < distance)
					return true;
			return false;
		}

		/// <summary> Returns items within distance of position. </summary>
		public List<T> GetDistance(Vector3 position, float distance)
		{
			var list = new List<T>();
			for (int i = 0; i < array.Length; ++i)
				if (array[i].gameObject.activeSelf && Vector3.Distance(array[i].transform.position, position) < distance)
					list.Add(array[i]);
			return list;
		}

		/// <summary> Get a unactive item, will actavate and rezise array if nessery. </summary>
		public T Pull()
		{
			for (int i = 0; i < array.Length; ++i)
			{
				if (!array[i].gameObject.activeSelf)
				{
					array[i].gameObject.SetActive(true);
					return array[i];
				}
			}

			// don't allow array to go over max.
			if (array.Length == Max)
			{
				Debug.LogWarning(parent.name + " is full!");
				return null;
			}

			// we need a new item, lets resize the array.
			int startIndex = array.Length;
			switch (resizeMode)
			{
				case ResizeMode.Fixed:
					System.Array.Resize<T>(ref array, System.Math.Min(array.Length + resize, Max));
					break;
				case ResizeMode.Double:
					System.Array.Resize<T>(ref array, System.Math.Min(array.Length * 2, Max));
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
				array[i].transform.SetParent(parent.transform);
				array[i].name = typeName + " " + i;
				array[i].gameObject.SetActive(false);
			}
		}
	}
}
