using UnityEngine;
using System.Collections;

public class L_System : MonoBehaviour {
	
	public float angle;
	public float edgeLength;
	public string buildList, returnList;
	public int generations;
	public ArrayList productions;
	public bool stop = false;
	public Color drawColor;
	public float lineWidth = 1;
	
	public static char[] symbolList = {'F', '1', '2', '3', '4', '5'};	//It should be remembered that under
																		//this production application system
	// Use this for initialization										//There must exist a defined 'F' rule
	void Start () 
	{}
	
	// Update is called once per frame
	void Update () 
	{
		//lookAtCamera();
	}
	
	public void propogateSymbols(int generations)
	{
		if(generations > 0)									
		{													
			buildList = "";						
			
			for(int i = 0; i < returnList.Length; i++)
			{
				int k = 0;
				bool productionFound = false;
				for(int j = 0; j < symbolList.Length; j++)
				{
					if(returnList[i] == symbolList[j])
					{
						productionFound = true;	
						k = j;					//Save the production index
						j = symbolList.Length;	//and break the loop
					}											
				}
				
				if(productionFound)
				{
					buildList = buildList.Substring(0) + productions[k];
					//buildList.Insert(buildList.Length - 1, (string)productions[k]);
				}
				else
					buildList = buildList.Substring(0) + returnList[i];
				//buildList.Insert(buildList.Length - 1, (string)returnList[i]);
			}
			
			returnList = buildList;							//Always make the base list
			propogateSymbols(generations - 1);						//into the newly generated
		}													//list after each recursion
		else
		{
			return;
		}
	}

	public void lookAtCamera()
	{
		transform.LookAt (new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z), 
		                              Vector3.up);
	}
}
