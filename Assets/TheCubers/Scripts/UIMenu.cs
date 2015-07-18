using UnityEngine;
using System.Collections;

namespace TheCubers
{
	[RequireComponent(typeof(RectTransform))]
	/// <summary>
	/// Note: sense we are disabling the whole gameobject, we update from UIBase.
	/// </summary>
	public class UIMenu : MonoBehaviour
	{
		enum State { Opened, Closed }
		private State state;
		private bool needUpdate;

		public float speed = 1f;
		private float position;

		private Vector2 screenLeft;
		private Vector2 screenRight;
		private Vector2 rest;

		private new RectTransform transform;

		public void Init()
		{
			transform = GetComponent<RectTransform>();

			gameObject.SetActive(false);

			rest = transform.anchoredPosition;
			int w = Screen.width;
			int h = Screen.height;
			screenLeft = new Vector2(-w, rest.y);
			screenRight = new Vector2(w, rest.y);
			transform.anchoredPosition = screenLeft;
			state = State.Closed;
			needUpdate = false;
		}


		public void UpdateUI()
		{
			if (!needUpdate)
				return;


			position += speed * Time.deltaTime;
			if (state == State.Opened)
				transform.anchoredPosition = Vector3.Slerp(screenLeft, rest, position);
			else
				transform.anchoredPosition = Vector3.Slerp(rest, screenRight, position);

			if (position >= 1f)
			{
				if (state == State.Closed)
					gameObject.SetActive(false);
				needUpdate = false;
			}
		}

		public void Set(bool open)
		{
			if (open)
				Open();
			else
				Close();
		}

		public void Open()
		{
			if (state == State.Opened)
				return;
			gameObject.SetActive(true);
			state = State.Opened;
			needUpdate = true;
			position = 0f;
		}

		public void Close()
		{
			if (state == State.Closed)
				return;
			//gameObject.SetActive(false);
			state = State.Closed;
			needUpdate = true;
			position = 0f;
		}

	}
}
