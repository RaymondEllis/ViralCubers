using UnityEngine;
using System.Collections;

namespace TheCubers
{
	[RequireComponent(typeof(Rigidbody))]
	public class Fourth : MonoBehaviour
	{
		public MeshRenderer Mesh;
		[System.NonSerialized]
		public Material Mat;
		public Color Color;
		public Rigidbody Rigidbody;

		void Awake()
		{
			if (!Rigidbody)
				Rigidbody = GetComponent<Rigidbody>();

			Mat = Mesh.material;
		}

		public void Init(Color color)
		{
			Color = color;
			Mat.color = Color;
			Rigidbody.AddForceAtPosition(new Vector3(0, 100f, 0), Vector3.up);
		}
	}
}