using UnityEngine;

namespace TheCubers
{
	public class UIBase : MonoBehaviour
	{
		void Start()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}
