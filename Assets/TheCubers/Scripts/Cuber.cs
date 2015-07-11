using UnityEngine;
using System.Collections;

public class Cuber : MonoBehaviour
{
	public MeshRenderer BodyMesh;
	public Material BodyMat;
	public Material InfectedBodyMat;

	public bool Infected;
	public float Energy;
	public int Life;
	public int Fourths;



	void Start()
	{

	}

	/// <summary>
	/// Sets vars and enables
	/// </summary>
	public void Init(bool infected)
	{
		Infected = infected;
		if (Infected)
		{
			BodyMesh.material = InfectedBodyMat;
			Energy = 1f;
			Life = 200;
		}
		else
		{
			BodyMesh.material = BodyMat;
			Energy = 0.5f;
			Life = 100;
		}
		Fourths = 0;
		enabled = true;
	}

	void Update()
	{

	}
}
