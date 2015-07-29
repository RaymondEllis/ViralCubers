using UnityEngine;

namespace TheCubers
{
	public class PlayerControl : MonoBehaviour
	{
		private static World world;
		[Header("Camera")]
		public Camera Camera;

		public Vector3 Offset;
		public Vector3 LookOffset;

		public float MoveSpeed;
		public float PositionSpeed;
		public float RotationSpeed;
		public bool SmoothRotate;

		public Transform Target;
		private Quaternion localRotation = Quaternion.identity;


		[Header("Control")]
		public float Energy;

		void Awake()
		{

			world = Object.FindObjectOfType<World>();

			if (!Camera)
				Camera = UnityEngine.Camera.main;
			if (!Camera)
				Debug.LogWarning("Unable to find camera to take control of!");
		}

		void Start()
		{

		}

		void LateUpdate()
		{
			// ToDo  4: Touch and other input methods.
			updateCamera();

			if (Input.GetMouseButtonDown(0))
			{
				Vector3 position;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (World.FindGround(ray, out position))
				{
					world.NewEnergy(position, Energy);
				}
			}


			if (Input.GetKeyDown(KeyCode.Alpha2))
				Time.timeScale = 2f;
			else if (Input.GetKeyUp(KeyCode.Alpha2))
				Time.timeScale = 1f;
		}

		private void updateCamera()
		{
			// rotate camera
			if (SmoothRotate)
			{
				if (Input.GetKey(KeyCode.Q))
					localRotation *= Quaternion.Euler(0, RotationSpeed * Time.deltaTime, 0);
				if (Input.GetKey(KeyCode.E))
					localRotation *= Quaternion.Euler(0, -RotationSpeed * Time.deltaTime, 0);
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.Q))
					localRotation *= Quaternion.Euler(0, 90, 0);
				if (Input.GetKeyDown(KeyCode.E))
					localRotation *= Quaternion.Euler(0, -90, 0);
			}

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
			Vector3 move = Vector3.zero;
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
				move.z = -1;
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
				move.z = 1;
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
				move.x = -1;
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
				move.x = 1;

			if (move != Vector3.zero)
			{
				transform.position += transform.rotation * move.normalized * MoveSpeed * Time.deltaTime;

				// move unlinks the target
				Target = null;
			}

			// make the our camera follow us
			Vector3 pos = transform.position + transform.rotation * new Vector3(Offset.x, 0f, Offset.z);
			pos.y = Offset.y;
			Vector3 look = transform.position + LookOffset;

			Camera.transform.position = Vector3.Lerp(Camera.transform.position, pos, PositionSpeed * Time.deltaTime);
			Camera.transform.LookAt(look);


		}
	}
}