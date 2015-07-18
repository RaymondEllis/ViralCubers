using UnityEngine;

namespace TheCubers
{
	/// <summary>
	/// Removes whole gameobject if finds more than Count of objects with Tag.
	/// </summary>
	class DummyRemoval : MonoBehaviour
	{
		public string Tag = "MainCamera";
		public int Count = 0;
		void Start()
		{
			if (GameObject.FindGameObjectsWithTag(Tag).Length > Count)
				Destroy(gameObject);
		}
	}
}
