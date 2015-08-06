using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace TheCubers
{
	public class ButtonLevel : MonoBehaviour
	{
		public Button Button;
		public Image Image;
		public Text Text;
		public Image Lock;

		public string text { get { return Text.text; } set { Text.text = value; } }

		public void Setinteractable(bool value)
		{
			Button.interactable = value;
			Text.enabled = value;
			Lock.enabled = !value;
		}
	}
}
