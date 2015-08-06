using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheCubers
{
	public enum Inp
	{
		Spawn,
		Pause,

		CameraVertical,
		CameraHorizontal,
		CameraRotate,
	}

	public static class MyInput
	{
		private class virtualInp
		{
			public enum type { Key, KeyAxis, Axis }
			public type Type;
			public KeyCode Low, High;
			public string Axis;
			public bool Invert;

			public Inp MapTo;

			public virtualInp(Inp mapTo, KeyCode key)
			{
				Type = type.Key;
				MapTo = mapTo;
				High = key;
			}
			public virtualInp(Inp mapTo, KeyCode low, KeyCode high)
			{
				Type = type.KeyAxis;
				MapTo = mapTo;
				High = high;
				Low = low;
			}
			public virtualInp(Inp mapTo, string axis, bool invert = false)
			{
				Type = type.Axis;
				MapTo = mapTo;
				Axis = axis;
				Invert = invert;
			}

			public bool GetDown()
			{
				if (Type != type.Key)
					return false;
				return Input.GetKeyDown(High);
			}

			public float GetAxis()
			{
				switch (Type)
				{
					case type.Key:
						return (Input.GetKey(High) ? 1f : 0f);
					case type.KeyAxis:
						return (Input.GetKey(Low) ? -1f : 0f) + (Input.GetKey(High) ? 1f : 0f);
					case type.Axis:
						if (Invert)
							return -Input.GetAxisRaw(Axis);
						return Input.GetAxisRaw(Axis);
				}
				return 0f;
			}
		}

		private static virtualInp[] virtuals;

		static MyInput()
		{
			virtuals = new virtualInp[] {
				 new virtualInp(Inp.CameraHorizontal, KeyCode.A, KeyCode.D)
				,new virtualInp(Inp.CameraHorizontal, KeyCode.LeftArrow, KeyCode.RightArrow)
				,new virtualInp(Inp.CameraHorizontal, "Horizontal")

				,new virtualInp(Inp.CameraVertical, KeyCode.W, KeyCode.S)
				,new virtualInp(Inp.CameraVertical, KeyCode.UpArrow, KeyCode.DownArrow)
				,new virtualInp(Inp.CameraVertical, "Vertical", true)

				,new virtualInp(Inp.CameraRotate, KeyCode.Q, KeyCode.E)
				,new virtualInp(Inp.CameraRotate, "Horizontal2")

				,new virtualInp(Inp.Spawn, KeyCode.Mouse0)
				,new virtualInp(Inp.Spawn, KeyCode.JoystickButton0)
				,new virtualInp(Inp.Spawn, KeyCode.JoystickButton4)
				,new virtualInp(Inp.Spawn, KeyCode.JoystickButton5)

				,new virtualInp(Inp.Pause, KeyCode.Escape)
				,new virtualInp(Inp.Pause, KeyCode.Pause)
				,new virtualInp(Inp.Pause, KeyCode.JoystickButton7)
			};
		}

		public static bool GetDown(Inp inp)
		{
			for (int i = 0; i < virtuals.Length; ++i)
				if (virtuals[i].MapTo == inp && virtuals[i].GetDown())
					return true;
			return false;
		}

		public static bool Get(Inp inp)
		{
			return Axis(inp) >= 0.5f;
		}

		public static bool Axis(Inp inp, out float value)
		{
			value = Axis(inp);
			return value != 0f;
		}
		public static float Axis(Inp inp)
		{
			float val = 0f;
			for (int i = 0; i < virtuals.Length; ++i)
			{
				if (virtuals[i].MapTo == inp)
					val += virtuals[i].GetAxis();
			}

			if (val > 1f)
				return 1f;
			if (val < -1f)
				return -1f;
			return val;
		}

	}
}
