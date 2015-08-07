using UnityEngine;
using System.Collections;

namespace TheCubers
{
	public class PlayerControl : MonoBehaviour
	{
		[Header("Camera")]
		public Camera Camera;

		public Vector3 Offset;
		public Vector3 LookOffset;

		public float MoveSpeed;
		public float PositionSpeed;
		public float RotationSpeed;

		public Transform Target;
		private Quaternion localRotation = Quaternion.identity;


		[Header("Control")]
		public float Energy;

		void Awake()
		{
			if (!Camera)
				Camera = Camera.main;
			if (!Camera)
				Debug.LogWarning("Unable to find camera to take control of!");
		}

		IEnumerator Start()
		{
			yield return StartCoroutine(World.WaitInstance());
		}

		void LateUpdate()
		{
			// ToDo  4: Touch and other input methods.
			updateCamera();

			if (World.Paused)
			{
				Time.timeScale = 1f;
			}
			else
			{
				if (MyInput.GetDown(Inp.Spawn) && World.Instance.UserEnergy > 0)
				{
					Vector3 position;
					// use mouse if pressent and on window.
					if (UIBase.MouseInScreen())
						position = Input.mousePosition;
					else
						position = new Vector3(Screen.width, Screen.height) * 0.5f;

					Ray ray = Camera.main.ScreenPointToRay(position);
					if (World.FindGround(ray, out position))
					{
						if (World.Instance.TestBounds(position))
							if (World.Instance.NewEnergy(position, Energy, true))
								--World.Instance.UserEnergy;
					}
				}

				// speed 2x
				if (MyInput.GetDown(Inp.Speed2x))
				{
					if (Time.timeScale == 1f)
						Time.timeScale = 2f;
					else
						Time.timeScale = 1f;
				}

				if (Input.GetKeyDown(KeyCode.Alpha5))
					Time.timeScale = 0.5f;
				if (Input.GetKeyDown(KeyCode.Alpha4))
					Time.timeScale = 4f;
			}
		}

		void OnDestory()
		{
			Time.timeScale = 1f;
		}

		private void updateCamera()
		{
			if (World.Paused)
				return;

			float deltaTime = Time.deltaTime / Time.timeScale;

			// rotate camera
			float val;
			if (MyInput.Axis(Inp.CameraRotate, out val))
				localRotation *= Quaternion.Euler(0, val * RotationSpeed * deltaTime, 0);


			// update transform based on target
			if (Target)
			{
				transform.position = Target.position;
				transform.rotation = localRotation * Target.rotation;
			}
			else
			{
				transform.rotation = localRotation;
			}

			// move?
			Vector3 move = new Vector3(MyInput.Axis(Inp.CameraVertical), 0f, MyInput.Axis(Inp.CameraHorizontal));

			if (move != Vector3.zero && World.Instance)
			{
				Vector3 newPos = transform.position + transform.rotation * move * MoveSpeed * deltaTime - World.Instance.transform.position;
				Vector3 bounds = World.Instance.transform.localScale * 1.5f;

				if (newPos.x > bounds.x)
					newPos.x = bounds.x;
				else if (newPos.x < -bounds.x)
					newPos.x = -bounds.x;

				if (newPos.z > bounds.z)
					newPos.z = bounds.z;
				else if (newPos.z < -bounds.z)
					newPos.z = -bounds.z;

				transform.position = newPos + World.Instance.transform.position;

				// move unlinks the target
				Target = null;
			}

			// make the our camera follow us
			Vector3 pos = transform.position + transform.rotation * new Vector3(Offset.x, 0f, Offset.z);
			pos.y = Offset.y;
			Vector3 look = transform.position + LookOffset;

			Camera.transform.position = pos; //Vector3.Lerp(Camera.transform.position, pos, PositionSpeed * Time.deltaTime);
			Camera.transform.LookAt(look);


		}
	}
}