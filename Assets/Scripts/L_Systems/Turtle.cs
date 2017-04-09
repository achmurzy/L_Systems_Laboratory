using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class Turtle : MonoBehaviour 
{	
	protected Stack branchStack;
	protected List<VectorLine> lineList;
	protected List<GameObject> plantParts;
	
	private L_System l_sys{ get; set; }	

	protected Vector3 initialPosition;
	protected Quaternion saveOrientation;
	protected Vector3 lastForward;

	public bool animate = false;

	private UnityEngine.Object currentObject;

	// Use this for initialization
	public void Awake () 
	{
		plantParts = new List<GameObject> ();
		lineList = new List<VectorLine> ();
		initialPosition = new Vector3 (transform.localPosition.x, 
		                               		transform.localPosition.y, 
		                               			transform.localPosition.z);

		l_sys = this.GetComponentInParent<L_System> ();

		branchStack = new Stack ();
	}

	void Start()
	{
		lastForward = l_sys.transform.forward;
		if(l_sys.GetType().BaseType == typeof(L_System_Animated))
			animate = true;
		l_sys.propogateSymbols (l_sys.generations);
		if(!animate)
			stir (l_sys.returnList);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(animate)
		{
			if(!l_sys.stop)
			{					
				destroyLines();
				stir (l_sys.returnList);
			}
		}
	}

	protected virtual void stir(string symbols)
	{
		transform.localPosition = new Vector3(initialPosition.x,
		                                      		initialPosition.y,
		                                      			initialPosition.z);

		if(animate)
		{
			Quaternion undoIt = Quaternion.FromToRotation(lastForward, l_sys.transform.forward);
			undoIt = Quaternion.Inverse (undoIt) * Quaternion.Inverse (undoIt);
			transform.rotation *= undoIt;
			lastForward = l_sys.transform.forward;
		}

		saveOrientation = new Quaternion(transform.rotation.x,
		                                    transform.rotation.y,
		                                    transform.rotation.z,
		                                    transform.rotation.w);

		for(int i = 0; i < symbols.Length; i++)
		{
			switch(symbols[i])
			{
				case 'F':
				{
					VectorLine line;
					Vector3 heading = l_sys.edgeLength * transform.up;
					
					line = VectorLine.SetLine3D(l_sys.drawColor, transform.localPosition, 
						                            transform.localPosition + heading);
					
					line.drawTransform = l_sys.transform;
					line.SetWidth(l_sys.lineWidth);
					line.Draw3DAuto();
					
					lineList.Add(line);						
					transform.localPosition += heading;
				}
					break;
				case 'f':
				{
					Vector3 heading = l_sys.edgeLength * transform.up;
					transform.localPosition += heading;
				}
					break;
				case '[':
				{
					TransformHolder pushTrans = new TransformHolder(); 
					pushTrans.Store(transform);
					branchStack.Push(pushTrans);
				}
					break;				
				case ']':					
				{ 
					TransformHolder popTrans = (TransformHolder)branchStack.Pop();
					transform.localPosition = new Vector3(popTrans.position.x, popTrans.position.y, popTrans.position.z);
					transform.localRotation = new Quaternion(popTrans.rotation.x, popTrans.rotation.y, popTrans.rotation.z, popTrans.rotation.w);
				}
				break;
				case '%':
					transform.Rotate(l_sys.transform.forward, l_sys.angle);
					break;
				case '*':
					transform.Rotate(l_sys.transform.forward, -l_sys.angle);
					break;
				case '&':
					transform.Rotate(new Vector3(l_sys.angle, 0, 0));
					break;
				case '^':
					transform.Rotate(new Vector3(-l_sys.angle, 0, 0));
					break;
				case '+':
					transform.Rotate(new Vector3(0, l_sys.angle, 0));
					break;
				case '-':
					transform.Rotate(new Vector3(0, -l_sys.angle, 0));
					break;							
				case 'O':  //A general case for object placement, assuming a given grammar deals with only a single object 
				{ 			//like, for instance, climbable buildings
					plantParts.Add(GameObject.Instantiate(currentObject, transform.position, transform.rotation) as GameObject);
				}
				break;
			}
		}
		transform.rotation = new Quaternion(saveOrientation.x, 
		                                    saveOrientation.y,
		                                    saveOrientation.z,
		                                    saveOrientation.w);
	}

	public bool destroyLines()
	{
		if(lineList.Count != 0)
		{
			VectorLine.Destroy (lineList);
			lineList.Clear();
			foreach(GameObject g in plantParts)
				Destroy(g);
			plantParts.Clear();
			return true;
		}
		else
		{
			stir (l_sys.returnList);
			return false;
		}
	}
	
	public void cameraControl(Camera cam)
	{
		VectorLine.SetCamera3D (cam);
	}
}

public class TransformHolder 
{
	public Vector3 position; 
	public Quaternion rotation; 
	
	public TransformHolder()
	{
		position = new Vector3();
		rotation = new Quaternion();
	}
	
	public void Store(Transform t)
	{
		position.Set(t.localPosition.x, t.localPosition.y, t.localPosition.z);
		rotation.Set(t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w);
	}
	
	public void Print()
	{
		Debug.Log (position + "\n");
		Debug.Log (rotation + "\n");
	}
}