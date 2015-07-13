using UnityEngine;

namespace TheCubers
{
	public class PlayerControl : MonoBehaviour
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
				Vector3 position;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (World.FindGround(ray, out position))
				{
					world.NewEnergy(position, 0.5f);
				}
			}
		}
	}
}