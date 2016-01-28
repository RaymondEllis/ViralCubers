using UnityEngine;
using System.Collections;

namespace TheCubers
{
	/// <summary>
	/// Because you NEED the controls to move on the virtual controller.
	/// </summary>
	public class Tutorial1Script : MonoBehaviour
	{
		public Transform JoyCam;
		public Transform JoyCamRot;

		public float StickMove;

		private Quaternion joyCamRest;
		private Quaternion joyCamRotRest;

		void Start()
		{
			if (JoyCam)
				joyCamRest = JoyCam.localRotation;
			if (JoyCamRot)
				joyCamRotRest = JoyCamRot.localRotation;
		}

		void Update()
		{
			if (JoyCam)
				JoyCam.localRotation = joyCamRest * Quaternion.Euler(MyInput.Axis(Inp.CameraVertical) * StickMove, -MyInput.Axis(Inp.CameraHorizontal) * StickMove, 0f);
			if (JoyCamRot)
				JoyCamRot.localRotation = joyCamRotRest * Quaternion.Euler(0f, -MyInput.Axis(Inp.CameraRotate) * StickMove, 0f);
		}
	}
}
