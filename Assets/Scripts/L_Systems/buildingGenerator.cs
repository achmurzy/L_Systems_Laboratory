using UnityEngine;
using System.Collections;

public class buildingGenerator : MonoBehaviour 
{
	private float offsetBoundary;
	private Rect innerRect;
	private Rect outerRect;

	// Use this for initialization
	void Start () 
	{
		offsetBoundary = 10.0f;
		innerRect = new Rect (transform.position.x, transform.position.y, 
		                      	transform.localScale.x, transform.localScale.y);

		outerRect = new Rect (innerRect);
		outerRect.x -= offsetBoundary;	outerRect.y -= offsetBoundary;	
		outerRect.width += 2 * offsetBoundary; outerRect.height += 2 * offsetBoundary;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool checkBoundaryLayer(Vector2 xz)
	{
		if(outerRect.Contains(xz))
		{
			return true;
		}
		return false;
	}
											//Returns false if the plant is generated in the boundary zone
	public bool checkBuildingXZ(Vector2 xz) //or is not located in the inner zone. 
	{
		if(innerRect.Contains(xz))
		{
			return true;
		}
		return false;
	}

	public float thatHeightThough()
	{
		return transform.position.y;
	}
}
