using UnityEngine;

namespace TheCubers
{
	public class EnergyPlacer : MonoBehaviour
	{
		private static World world;

		void Awake()
		{
			world = Object.FindObjectOfType<World>();
		}

		void Start()
		{

		}

		void Update()
		{
			// ToDo  4: Touch and other input methods.
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Plane plane = new Plane(Vector3.up, 0f);
				float enter;
				if (plane.Raycast(ray, out enter))
				{
					Vector3 position = ray.GetPoint(enter);
					world.NewEnergy(position, 0.5f);
				}
			}
		}
	}
}