using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public class InitialLoading : MonoBehaviour
	{
		IEnumerator Start()
		{
			// Load and apply options
			UIOptions.Load(null);
			yield return new WaitForSeconds(0.25f);

			// Load UI
			yield return StartCoroutine(BaseChecker.Check());

			// wait for UI.
			yield return StartCoroutine(UIBase.WaitInstance());

			// load menu level
			UIBase.Instance.LoadLevel("menu");
		}
	}
}
