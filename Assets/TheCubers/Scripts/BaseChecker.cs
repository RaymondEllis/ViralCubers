using UnityEngine;
using System.Collections;

class BaseChecker : MonoBehaviour
{
	private static bool done = false;
	void Awake()
	{
		if (done)
			Destroy(this);
	}

	IEnumerator Start()
	{
		Debug.Log("Adding base level");
		var async = Application.LoadLevelAdditiveAsync("base");
		yield return async;
		done = true;
		Destroy(this);
	}
}
