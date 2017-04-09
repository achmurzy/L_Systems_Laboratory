using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class Szilard_Turtle : MonoBehaviour 
{
	public Szilard_L_System szilardSystem;

    public Stack<TransformHolder> branchStack;
    public List<VectorLine> lineList;
   
	Dictionary <int, List<SzilardModule>> additionsList;

	private Vector3 initialPosition;
	private Quaternion saveOrientation;
	
	// Use this for initialization
	void Start () 
	{
		//We have to start turtle analysis at the same place each time we stir
		initialPosition = transform.localPosition;

		szilardSystem = this.GetComponentInParent<Szilard_L_System> ();
		
		branchStack = new Stack <TransformHolder>();
		lineList = new List<VectorLine> ();
		additionsList = new Dictionary<int, List<SzilardModule>>();
	}
	
	void Update()
	{
		
	}
	
	public void TurtleAnalysis(List<SzilardModule> word, float age)
	{
		destroyLines ();
		stir (word, age);
	}
	
	void stir(List<SzilardModule> word, float age)
	{
		
		//Each new stir we restart at the local origin.
		transform.localPosition = initialPosition;

		saveOrientation = new Quaternion(transform.rotation.x,
		                                 transform.rotation.y,
		                                 transform.rotation.z,
		                                 transform.rotation.w);

		foreach(SzilardModule mod in word)
		{
			if(szilardSystem.productions.ContainsKey(mod.symbol))
			{
				if(mod.age > mod.terminalAge && mod.terminalAge > 0)
				{
					szilardSystem.newString = true;
					List<SzilardModule> newList = new List<SzilardModule>();
					
					for(int i = 0; i < szilardSystem.productions[mod.symbol].Count; i++)
					{
						SzilardModule m = new SzilardModule(szilardSystem.productions [mod.symbol][i]);
						newList.Add(m);
					}
					additionsList.Add(word.IndexOf(mod), newList);
				}
			}

			switch(mod.symbol)
			{
				case 'F':
				{
					VectorLine line;
					
					float length = szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age);
					Vector3 heading = length * transform.up;
					
					line = VectorLine.SetLine3D(Color.green, transform.localPosition, 
					                            transform.localPosition + heading);
					line.drawTransform = szilardSystem.transform;
					line.SetWidth(szilardSystem.BranchWidth);
					lineList.Add(line);						
					
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
				case '&':
				{
					transform.Rotate(new Vector3(szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0, 0));
				}
					break;
				case '^':
				{
					transform.Rotate(new Vector3(-szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0, 0));
				}
					break;
				case '+':
				{
					transform.Rotate(new Vector3(0, szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0));
				}	
					break;
				case '-':
				{ 
					transform.Rotate(new Vector3(0, -szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0));
				}	
					break;
				case '%':
				{
					transform.Rotate(new Vector3(0, 0, szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age)));
				}	
					break;
				case '*':
				{
					transform.Rotate(new Vector3(0, 0, -szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age)));
				}	
					break;
				case '!':
				{				
					szilardSystem.BranchWidth = (szilardSystem.growthFunction(mod.symbol, mod.growthParameter, mod.age));	
				}
					break;
				default:
					break;
			}
			if(mod.age < mod.terminalAge)
				mod.age += age;
		}
		if(additionsList.Count != 0)
		{
			addNewProductions();
		}
		transform.rotation = new Quaternion(saveOrientation.x, 
		                                    saveOrientation.y,
		                                    saveOrientation.z,
		                                    saveOrientation.w);
	}
	
	void addNewProductions()
	{
		int i = -1;
		
		foreach(KeyValuePair<int, List<SzilardModule>> kvp in additionsList)
		{
			if(i == -1)
			{
				i = kvp.Value.Count - 1;
				szilardSystem.returnList.RemoveAt(kvp.Key);
				szilardSystem.returnList.InsertRange(kvp.Key, kvp.Value);
			}
			else
			{
				szilardSystem.returnList.RemoveAt(i+kvp.Key);
				szilardSystem.returnList.InsertRange(i+kvp.Key, kvp.Value);
				i += kvp.Value.Count - 1;
			}
		}
		additionsList.Clear ();
	}
	
	//Used for animation. We cannot possibly transform the VectorLines or the models correctly
	//so we destroy them all and let the turtle create new, correctly oriented ones.
	public new void destroyLines()
	{
		VectorLine.Destroy (lineList);
	}
	
	public void destroySystem()
	{
		VectorLine.Destroy(lineList);
		GameObject.Destroy(transform.parent.gameObject);
		GameObject.Destroy(gameObject);
	}
	
	public void HideLines()
	{
		/*foreach(GameObject go in plantParts)
			go.SetActive(!go.activeSelf);*/
		foreach(VectorLine vi in lineList)
			vi.active = !vi.active;
		
	}
}
