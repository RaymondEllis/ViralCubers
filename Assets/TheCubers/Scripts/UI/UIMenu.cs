﻿using UnityEngine;
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
		public bool IgnoreFirstSelected = false;
		public float speed;
		public AnimationCurve SlideCurve;
		private float position;

		private Vector2 rest;

		[HideInInspector]
		public new RectTransform transform;

		private Selectable[] items;

		public void Init()
		{
			if (!FirstSelected && !IgnoreFirstSelected)
				Debug.LogError("Menu " + name + " missing FirstSelected!");

			transform = GetComponent<RectTransform>();

			gameObject.SetActive(false);

			rest = Vector2.zero;
			state = State.Closed;
			needUpdate = false;

			items = GetComponentsInChildren<Selectable>(true);
			OnInit();
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

		protected virtual void OnOpenStart() { }
		protected virtual void OnCloseStart() { }

		public void Open()
		{
			if (state == State.Opened)
				return;
			gameObject.SetActive(true);
			interactableChildren(true);
			state = State.Opened;
			needUpdate = true;
			position = 0f;
			if (!IgnoreFirstSelected)
				FirstSelected.Select();
			OnOpenStart();
		}

		public void Close()
		{
			if (state == State.Closed)
				return;
			interactableChildren(false);
			state = State.Closed;
			needUpdate = true;
			position = 0f;
			OnCloseStart();
		}

		private void interactableChildren(bool interactable)
		{
			for (int i = 0; i < items.Length; ++i)
				if (items[i])
					items[i].interactable = interactable;
		}
	}
}