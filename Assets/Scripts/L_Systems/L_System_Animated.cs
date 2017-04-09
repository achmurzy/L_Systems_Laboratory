using UnityEngine;
using System.Collections;

public class L_System_Animated : L_System {

	public float maxAngle;
	public float maxEdge;

	public float angleAnimateSpeed;
	public float edgeAnimateSpeed;

	public float age;
	public bool inCircle;
	public bool followCamera;


	// Use this for initialization
	void Start () 
	{
		initializeAnimation ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Animate ();
	}

	public void initializeAnimation()
	{
		edgeLength = 0;
		angle = 0;
		age = 0;
		inCircle = true;
		stop = false;
		followCamera = true;
	}

	public virtual void Animate()
	{
		if(!stop)
		{
			if(angle < maxAngle)
			{
				angle += angleAnimateSpeed;
			}
			
			if(edgeLength < maxEdge)
			{
				edgeLength += edgeAnimateSpeed;
			}
			
			if(edgeLength >= maxEdge && angle >= maxAngle)
				stop = true;
		}
		
		if(!inCircle)
			age += Time.deltaTime;

		if(followCamera)
			lookAtCamera ();
	}
}
