using UnityEngine;

namespace TheCubers
{
	[SelectionBase]
	[RequireComponent(typeof(Rigidbody))]
	public class Fourth : Edible
	{
		public MeshRenderer Mesh;
		//[System.NonSerialized]
		//public Material Mat;
		//public Color Color;
		public Rigidbody Rigidbody;

		void Awake()
		{
			if (!Rigidbody)
				Rigidbody = GetComponent<Rigidbody>();

			//Mat = Mesh.material;
		}

		public void Init()//(Color color)
		{
			initEdible(1, true);

			//Color = color;
			//Mat.color = Color;
			Rigidbody.AddForceAtPosition(new Vector3(0, 100f, 0), Vector3.up);
		}

		protected override void OnUpdate()
		{
		}
	}
}