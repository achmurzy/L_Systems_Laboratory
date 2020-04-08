using UnityEngine;
using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class Parametric_Turtle : MonoBehaviour 
{
	public Parametric_L_System p_l_sys;
	protected Dictionary <int, List<SystemModule>> additionsList;

    public struct TransformHolder 
    {
        public Vector3 position; 
        public Quaternion rotation; 
        
        public void Store(Transform t)
        {
            position.Set(t.localPosition.x, t.localPosition.y, t.localPosition.z);
            rotation.Set(t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w);
        }
    }

    public int DepthCounter = 0;
    public Stack<ApexModule> apexStack;
    public Stack<TransformHolder> branchStack;
    private Stack<Joint> jointStack;
    
    public List<GameObject> objList;

    protected Vector3 initialPosition;
    protected Quaternion saveOrientation;

	public const int PRODUCTION_ONE = 1, PRODUCTION_TWO = 2, PRODUCTION_THREE = 3, PRODUCTION_FOUR = 4, PRODUCTION_FIVE = 5;
	public const char DRAW = 'F', OBJECT = 'O', JOINT_OPEN = '{', JOINT_CLOSE = '}', BRANCH_OPEN = '[', BRANCH_CLOSE = ']', 
                        MESH = 'M', PHYSICS = 'P', ROTATE = '+', EMPTY = '-';
    

    void Awake()
    {
		branchStack = new Stack <TransformHolder>();
		jointStack = new Stack<Joint>();
        apexStack = new Stack<ApexModule>();
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
        VectorLine.SetCamera3D(FindObjectOfType<Camera>());
	}

	void Update()
	{

	}

	public void TurtleAnalysis(float age)
	{
        List<SystemModule> word = p_l_sys.returnList;

        p_l_sys.BaseNode.transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(initialPosition.x,
                                              initialPosition.y,
                                              initialPosition.z);

        saveOrientation = new Quaternion(transform.rotation.x,
                                         transform.rotation.y,
                                         transform.rotation.z,
                                         transform.rotation.w);

        jointStack.Push(p_l_sys.BaseNode.GetComponent<Joint>());

        for (int i = 0; i < word.Count; i++)
        {
            SystemModule mod = word[i];
            mod.Age += age;
            mod.Age = Mathf.Clamp(mod.Age, 0, mod.TerminalAge);
            mod.UpdateModule(this);

            if (p_l_sys.Productions.ContainsKey(mod.Symbol))
            {
                bool apex_ready = true;
                if (mod is ApexModule)
                {
                    if ((mod as ApexModule).mature)
                    {
                        apex_ready = false;
                    }

                }
                //May need to move this to inside modules so that they can develop independently?
                if (mod.Age >= mod.TerminalAge && mod.TerminalAge >= 0 && apex_ready)
                {
                    
                    List<SystemModule> newList = new List<SystemModule>();
                    for (int j = 0; j < p_l_sys.Productions[mod.Symbol].Count; j++)
                    {
                        SystemModule m = p_l_sys.Productions[mod.Symbol][j].CopyModule();
                        m.InstantiateModule(this);
                        newList.Add(m);
                    }

                    if(mod is ApexModule)
                    {
                        ApexModule am = mod as ApexModule;
                        am.Apex.ProximalModules = newList;
                        am.mature = true;    
                    }
                    else if(mod is PhysicsMoverModule)
                    {
                        PhysicsMoverModule pmm = mod as PhysicsMoverModule;
                        pmm.ProximalModules = newList;
                        pmm.PhysicsModule.GetComponent<ModuleBehaviour>().ProximalModules = newList;
                    }
                    additionsList.Add(i, newList);
                }
            }
        }
        apexStack.Clear();

        if (additionsList.Count != 0)
        {
            p_l_sys.newString = true;
            int i = -1;

            foreach (KeyValuePair<int, List<SystemModule>> kvp in additionsList)
            {
                if (i == -1) //First iteration
                {
                    i = kvp.Value.Count;
                    //p_l_sys.returnList.RemoveAt(kvp.Key);
                    p_l_sys.returnList.InsertRange(kvp.Key + 1, kvp.Value);
                }
                else //every other
                {
                    //p_l_sys.returnList.RemoveAt(i + kvp.Key);
                    p_l_sys.returnList.InsertRange(i + kvp.Key + 1, kvp.Value);
                    i += kvp.Value.Count;
                }
            }
            additionsList.Clear();
        }
        transform.rotation = new Quaternion(saveOrientation.x,
                                            saveOrientation.y,
                                            saveOrientation.z,
                                            saveOrientation.w);
    }
}
