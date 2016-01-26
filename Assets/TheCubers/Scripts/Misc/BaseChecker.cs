using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

class BaseChecker : MonoBehaviour
{
	private static string testObj = "UI";
	private static bool done = false;
	void Awake()
	{
		if (done)
			Destroy(this);
	}

	IEnumerator Start()
	{
		yield return StartCoroutine(Check());
		Destroy(this);
	}

	public static IEnumerator Check()
	{
		if (GameObject.Find(testObj) == null)
		{
			Debug.Log("Adding base level");
			var async = SceneManager.LoadSceneAsync("base", LoadSceneMode.Additive);
			yield return async;
		}
		done = true;
	}
}
