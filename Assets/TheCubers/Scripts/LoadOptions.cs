using UnityEngine;

public class LoadOptions : MonoBehaviour
{
	void Start()
	{
		UIOptions.Load(null);
		Destroy(gameObject);
	}
}
