using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class Parametric_Turtle : MonoBehaviour 
{
	private Parametric_L_System p_l_sys;
	private Dictionary <int, List<SystemModule>> additionsList;

    private struct TransformHolder 
    {
        public Vector3 position; 
        public Quaternion rotation; 
        
        public void Store(Transform t)
        {
            position.Set(t.localPosition.x, t.localPosition.y, t.localPosition.z);
            rotation.Set(t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w);
        }
    }

    private Stack<TransformHolder> branchStack;
    private Stack<Joint> jointStack;
    private List<GameObject> objList;

    private Vector3 initialPosition;
    private Quaternion saveOrientation;

	public const int PRODUCTION_ONE = 1, PRODUCTION_TWO = 2, PRODUCTION_THREE = 3, PRODUCTION_FOUR = 4, PRODUCTION_FIVE = 5;
	public const char DRAW = 'F', OBJECT = 'O', JOINT_OPEN = '{', JOINT_CLOSE = '}', BRANCH_OPEN = '[', BRANCH_CLOSE = ']', ROTATE = '+', EMPTY = '-';
    

    void Awake()
    {
		branchStack = new Stack <TransformHolder>();
		jointStack = new Stack<Joint>();
		objList = new List<GameObject>();
		additionsList = new Dictionary<int, List<SystemModule>>();

		p_l_sys = this.GetComponentInParent<Parametric_L_System> ();
    }

	// Use this for initialization
	void Start () 
	{
		//We have to start turtle analysis at the same place each time we stir
		initialPosition = new Vector3 (transform.localPosition.x, 
		                               transform.localPosition.y, 
		                               transform.localPosition.z);
		jointStack.Push(p_l_sys.BaseNode.GetComponent<Joint>());
        VectorLine.SetCamera3D(FindObjectOfType<Camera>());
	}

	void Update()
	{

	}

	public void TurtleAnalysis(List<SystemModule> word, float Age)
	{
		destroyLines ();
		stir (word, Age);
	}

	void stir(List<SystemModule> word, float Age)
	{
		p_l_sys.BaseNode.transform.localRotation = Quaternion.identity;
		transform.localPosition = new Vector3(initialPosition.x,
		                                      initialPosition.y,
		                                      initialPosition.z);

		saveOrientation = new Quaternion(transform.rotation.x,
		                                 transform.rotation.y,
		                                 transform.rotation.z,
		                                 transform.rotation.w);

		jointStack.Push(p_l_sys.BaseNode.GetComponent<Joint>());

		for(int i = 0; i < word.Count; i++)
		{
			SystemModule mod = word[i];
			if(p_l_sys.Productions.ContainsKey(mod.Symbol))
			{
				if(mod.Age >= mod.TerminalAge && mod.TerminalAge >= 0)
				{
					List<SystemModule> newList = new List<SystemModule>();
					for(int j = 0; j < p_l_sys.Productions [mod.Symbol].Count; j++)
					{
						SystemModule m = p_l_sys.Productions [mod.Symbol][j].CopyModule();
						newList.Add(m);
					}

					additionsList.Add(i, newList);
				}
			}
			switch(mod.Symbol)
			{
				case DRAW:
				{
					LineModule line = mod as LineModule;
					Vector3 heading = line.GrowthFunction(line.LineLength) * transform.up;

                    GameObject lineObject = line.ObjectInstantiate();
                    GameObject parent;
                    if(line.jointed)
                    {
                        Joint thisJoint = jointStack.Peek();	//Assume every branch is jointed
                        if (thisJoint is CharacterJoint)
                        {
                            CharacterJoint cj = thisJoint as CharacterJoint;
                            cj.swingAxis = Vector3.Cross(p_l_sys.transform.up, heading.normalized);
                        }
                        else
                        {
                            thisJoint.axis = Vector3.Cross(p_l_sys.transform.up, heading.normalized);
                        }
                        parent = thisJoint.gameObject;
                        line.ObjectInitialize(parent);
                        lineObject.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        parent = p_l_sys.gameObject;
                        line.ObjectInitialize(parent);
                        lineObject.transform.localPosition = this.transform.localPosition;
                    }

					objList.Add(line.ModuleObject);	

					transform.localPosition += heading;				
				}
					break;
				case JOINT_OPEN:
				{
					JointModule jm = mod as JointModule;
					GameObject obj = jm.ObjectInstantiate();
					obj.transform.parent = p_l_sys.transform;
					obj.transform.localPosition = this.transform.localPosition;
					obj.transform.localRotation = this.transform.localRotation;

					GameObject parentJoint = jointStack.Peek().gameObject;
					/*Debug.Log("TurtleWorld: " + this.transform.position);
					Debug.Log("TurtleLocal: " + this.transform.localPosition);
					Debug.Log("JointWorld: " + obj.transform.position);
					Debug.Log("JointLocal: " + obj.transform.localPosition);
					Debug.Log("ConnectorWorld: " + parentJoint.transform.position);
					Debug.Log("ConnectorLocal: " + parentJoint.transform.localPosition);
					Debug.Log("TurtleDifference: " + (this.transform.localPosition - parentJoint.transform.localPosition));*/
					jm.ObjectInitialize(jointStack.Peek().gameObject);
					jointStack.Push(jm.NodeJoint);
					objList.Add(obj);
				}
					break;
				case JOINT_CLOSE:
				{
					jointStack.Pop();
				}
					break;
				case BRANCH_OPEN:
				{
					TransformHolder pushTrans = new TransformHolder(); 
					pushTrans.Store(transform);
					branchStack.Push(pushTrans);
				}
					break;				
				case BRANCH_CLOSE:					
				{ 
					TransformHolder popTrans = (TransformHolder)branchStack.Pop();
					transform.localPosition = new Vector3(popTrans.position.x, popTrans.position.y, popTrans.position.z);
					transform.localRotation = new Quaternion(popTrans.rotation.x, popTrans.rotation.y, 
													popTrans.rotation.z, popTrans.rotation.w);
				}
					break;	
				case ROTATE:
				{
					RotationModule rm = mod as RotationModule;
					transform.Rotate(rm.RotationAxis, rm.GrowthFunction(rm.RotationScalar));
				}
					break;
				case OBJECT:
				{
					ObjectModule om = mod as ObjectModule;
					GameObject obj = om.ObjectInstantiate();
					obj.transform.position = this.transform.position;
					obj.transform.rotation = this.transform.rotation;
					Joint thisJoint = jointStack.Peek();	
					om.ObjectInitialize(thisJoint.gameObject);
					objList.Add(obj);
				}
					break;
				default:
				break;
			}	

		mod.Age += Age;
        mod.Age = Mathf.Clamp(mod.Age, 0, mod.TerminalAge);
		//word[i] = mod;
		}

		if(additionsList.Count != 0)
		{
			p_l_sys.newString = true;
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

		foreach(KeyValuePair<int, List<SystemModule>> kvp in additionsList)
		{
			if(i == -1)
			{
				i = kvp.Value.Count - 1;
				p_l_sys.returnList.RemoveAt(kvp.Key);
				p_l_sys.returnList.InsertRange(kvp.Key, kvp.Value);
			}
			else
			{
				p_l_sys.returnList.RemoveAt(i+kvp.Key);
				p_l_sys.returnList.InsertRange(i+kvp.Key, kvp.Value);
				i += kvp.Value.Count - 1;
			}
		}
		additionsList.Clear ();
	}

	//Used for animation. We cannot possibly transform the VectorLines or the models correctly
	//so we destroy them all and let the turtle create new, correctly oriented ones.
    //Circumventing this process would greatly increase efficiency
	public void destroyLines()
	{
		//VectorLine.Destroy(lineList);
		foreach(GameObject go in objList)
			GameObject.Destroy(go);
		objList.Clear ();
		jointStack.Clear();
	}

	public void destroySystem ()
	{
		GameObject.Destroy (transform.parent.gameObject);
		GameObject.Destroy (gameObject);
	}
}
