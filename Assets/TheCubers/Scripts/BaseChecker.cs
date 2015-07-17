using UnityEngine;

class BaseChecker : MonoBehaviour
{
	void Awake()
	{
		BaseChecker[] test = GameObject.FindObjectsOfType<BaseChecker>();
		if (test != null && test.Length > 1)
			DestroyImmediate(gameObject);
	}

	void Start()
	{
		DontDestroyOnLoad(gameObject);
		Debug.Log("Adding base level");
		Application.LoadLevelAdditive("base");
	}
}
