using UnityEngine;
using System.Collections;
using Vectrosity;

public class GLTestingScript : MonoBehaviour {

	private float drawLength = 10.0f;
	// Use this for initialization
	void Start () 
	{
		GL.Color (Color.green);
	}
	
	// Update is called once per frame
	void Update () 
	{
		VectorLine.SetLine3D (Color.green, transform.position, transform.position + (Vector3.up * drawLength));
	}

	void OnPostRender()
	{

	}
}
