using UnityEngine;
using UnityEngine.UI;
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

		public Selectable FirstSelected;
		public bool IgnoreSelected = false;
		[System.NonSerialized]
		public Selectable LastSelected = null;
		public float speed;
		public AnimationCurve SlideCurve;
		private float position;

		private Vector2 rest;

		[HideInInspector]
		public new RectTransform transform;

		private Selectable[] items;

		public void Init()
		{
			transform = GetComponent<RectTransform>();

			gameObject.SetActive(false);

			rest = Vector2.zero;
			state = State.Closed;
			needUpdate = false;

			OnInit();

			if (!IgnoreSelected)
			{
				if (!FirstSelected)
					Debug.LogError("Menu " + name + " missing FirstSelected!");

				items = GetComponentsInChildren<Selectable>(true);
			}
		}

		protected virtual void OnInit() { }

		public void UpdateUI()
		{
			if (!needUpdate)
				return;

			var screenLeft = new Vector2(-Screen.width, rest.y);
			var screenRight = new Vector2(Screen.width, rest.y);

			position += speed * Time.deltaTime;
			if (state == State.Opened)
				transform.anchoredPosition = Vector3.Lerp(screenLeft, rest, SlideCurve.Evaluate(position));
			else
				transform.anchoredPosition = Vector3.Lerp(rest, screenRight, SlideCurve.Evaluate(position));

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

		protected virtual void OnDoSelect()
		{
			if (LastSelected)
				LastSelected.Select();
			else
				FirstSelected.Select();
		}
		protected virtual void OnOpenStart() { }
		protected virtual void OnCloseStart() { }

		public void Open()
		{
			if (state == State.Opened)
				return;
			gameObject.SetActive(true);
			if (!IgnoreSelected)
				interactableChildren(true);
			state = State.Opened;
			needUpdate = true;
			position = 0f;
			OnOpenStart();
			if (!IgnoreSelected)
				OnDoSelect();
		}

		public void Close()
		{
			if (state == State.Closed)
				return;
			if (!IgnoreSelected)
			{
				findLastSelected();
				interactableChildren(false);
			}
			state = State.Closed;
			needUpdate = true;
			position = 0f;
			OnCloseStart();
		}

		private void findLastSelected()
		{
			var obj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (!obj)
				return;

			var sel = obj.GetComponent<Selectable>();
			LastSelected = sel;
		}

		private void interactableChildren(bool interactable)
		{
			for (int i = 0; i < items.Length; ++i)
				if (items[i])
					items[i].interactable = interactable;
		}
	}
}
