using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public class InitialLoading : MonoBehaviour
	{
		public float Wait = 0.5f;

		void Awake()
		{
			// Load and apply options
			UIOptions.Load(null);
		}

		IEnumerator Start()
		{
			// Load UI
			yield return StartCoroutine(BaseChecker.Check());

			// wait for UI.
			yield return StartCoroutine(UIBase.WaitInstance());

			yield return new WaitForSeconds(Wait);

			// load menu level
			UIBase.Instance.LoadLevel("menu");
		}
	}
}
