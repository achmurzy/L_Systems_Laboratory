using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class Turtle2D : MonoBehaviour {
	//To emulate the spatial variables we have for the c++ version of the turtle we attach the turtle
	//script to an empty game object. From this we receive a transform, meaning a world position and rotation.
	//We do however still need to specify intial orientation and position
	
	private Stack branchStack;
	public List<VectorLine> lineList;

	//2D coordinate, rotation float in z position
	private float rotation;
	public Transform UIparent;
	public Vector2 offset;
	private Vector3 initialPos;
	
	// Use this for initialization
	void Start () 
	{	
		initialPos = this.transform.position;
		branchStack = new Stack ();
		lineList = new List<VectorLine> ();
		rotation = Mathf.PI / 2;
	}
	
	// Update is called once per frame
	void Update () 
	{}
	
	public bool destroyLines()
	{
		if(lineList.Count != 0)
		{
			VectorLine.Destroy (lineList);
			lineList.Clear();
			return true;
		}
		else
		{
			VectorLine.canvas.GetComponent<CanvasScaler>().enabled = false;
			transform.position = initialPos;
			//stir (guilsys.returnList);
			VectorLine.canvas.GetComponent<CanvasScaler>().enabled = true;
			return false;
		}
	}
	
	public void stir(string symbols)
	{	
		float theta = 10; //= guilsys.angle;
		float edge = 1; //= guilsys.edgeLength;
		
		for(int i = 0; i < symbols.Length; i++)
		{
			switch(symbols[i])
			{
			case '%':
				rotation += theta;
				break;
			case '*':
				rotation -= theta;
				break;
			case '[':
			{
				transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, rotation);
				branchStack.Push(transform.localPosition);
			}
				break;				
			case ']':					
			{ 
				transform.localPosition = (Vector3)branchStack.Pop();
				rotation = transform.localPosition.z;
				transform.localPosition = new Vector3(transform.localPosition.x, 
				                                      transform.localPosition.y, 0);
			}
				break;							
			case 'F':
			{
				VectorLine line;
				
				Vector2 pos = new Vector2(transform.localPosition.x, transform.localPosition.y);
				pos +=offset;
				Vector2 newPos = new Vector2(pos.x + (edge * Mathf.Cos(rotation)),
				                             pos.y + (edge * Mathf.Sin(rotation)));
				line = VectorLine.SetLine(Color.green, pos, newPos);
				newPos -= offset;
				line.rectTransform.anchorMax = new Vector2(1, 0);
				line.rectTransform.anchorMin = new Vector2(1, 0);
				lineList.Add(line);						
				transform.localPosition = new Vector3(newPos.x, newPos.y, 0);
			}
				break;
			}
		}
	}
	
	public void HideLines()
	{
		foreach(VectorLine vi in lineList)
			vi.active = !vi.active;
	}
}