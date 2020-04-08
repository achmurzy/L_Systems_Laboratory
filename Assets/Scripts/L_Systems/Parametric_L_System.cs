using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parametric_L_System : MonoBehaviour 
{
	public const float MAX_MATURITY = 10f;
	public List<SystemModule> returnList { get; private set; }
    public SystemModule Axiom;
	public Dictionary <char, List<SystemModule>> Productions { get; set; }
	public int ProductionCount { get { return Productions.Count; } }

	public float Age { get; set; }
	public float Maturity = MAX_MATURITY;

    public GameObject BaseNode { get { return transform.GetChild (1).gameObject; } }
	public bool inGrowth = false;
    public Parametric_Turtle Turtle;

    private ModuleHolder storedModules;
    public bool DeriveModules = false;

    void Awake()
	{
        Turtle = GetComponentInChildren<Parametric_Turtle>();

		//We need to be able to preserve the returnList between storages as a pre-fab
		returnList = new List<SystemModule> ();

		Productions = new Dictionary<char, List<SystemModule>> ();
		Maturity = Mathf.Clamp(Maturity, 0f, MAX_MATURITY);
	}

	// Use this for initialization
	public void Start () 
	{
        returnList.Add(new BranchModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL, true));
        returnList.Add(Axiom);
        returnList.Add(new BranchModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL, false));
        if (DeriveModules)
		{
			ImportDerivation();
			Turtle.TurtleAnalysis(0f);	
			TogglePhysics();
			Age = MAX_MATURITY;
		}
		else
		{
			ImportProductions();
			Age = 0f;
		}
	}
	
	// Update is called once per frame
	protected void Update () 
	{
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(!inGrowth)
            {
            	inGrowth = true;
			}
			else if (Age < Maturity)
			{
				Turtle.TurtleAnalysis(Time.deltaTime);
            	Age += (Time.deltaTime);
			}
        }
		else if(Age > 0f &&  Input.GetKey(KeyCode.LeftControl))
        {
			Turtle.TurtleAnalysis(-Time.deltaTime);
            Age -= Time.deltaTime;
        }
		else if(inGrowth)
        {
			inGrowth = false;
        }
	}

    //Returns the underived apical meristem on the leader axis
    public ApexModule GetLeaderApex()
    {
        foreach (SystemModule sm in returnList)
        {
            if (sm is ApexModule)
            {
                ApexModule am = sm as ApexModule;
                if (!am.mature && am.MainAxis)
                {
                    return am;
                }
            }
        }
        return null;
    }

    //Returns nearest main axis apex
    public ApexModule GetProximalMainAxisApex(ApexModule am)
    {
        ApexModule apex_candidate = am;
        while (!apex_candidate.MainAxis)
        {
            apex_candidate = GetProximalMainAxisApex(am.Parent);
        }
        return apex_candidate;
    }

    public void GrowthControl()
	{
		if (!inGrowth)
		{
			inGrowth = true;
			StartCoroutine ("GrowthRoutine");
		}
		else
		{
			inGrowth = false;
			StopCoroutine ("GrowthRoutine");
		}
	}

	IEnumerator GrowthRoutine()
	{
		while(true)
		{
			//yield return new WaitForSeconds(growthSpeed);
			yield return new WaitForFixedUpdate();
			if (Age < Maturity)
			{
				Turtle.TurtleAnalysis(Time.deltaTime);
				Age += Time.deltaTime;
			}
			else
				StopCoroutine("GrowthRoutine");
		}
	}

	public void BuildDefaultSystem()
	{
		ObjectModule om = new ObjectModule('O', 0, 1, GrowthList.LINEAR, ObjectBuilder.OBJECT_PATH+ObjectList.SIMPLE_LEAF);
		om.scale = Vector3.one * 0.1f;
		om.rotation = new Vector3(90, 0, 0);

        ObjectModule om_right = new ObjectModule('O', 0, 1, GrowthList.LINEAR, ObjectBuilder.OBJECT_PATH + ObjectList.SIMPLE_LEAF);
		om_right.scale = Vector3.one * 0.1f;
		om_right.rotation = new Vector3(90, 180, 0);
        om_right.jointed = true;

        JointModule jm = new JointModule('{', 0, 1, GrowthList.LINEAR, JointBuilder.JOINT_PATH + JointModule.HINGE_JOINT);
		jm.jointSpringSpring = 10f;
		jm.jointSpringDamper = 99f;

		jm.jointLimitMin = -10;
		jm.jointLimitMax = 10;

        ObjectModule om_right_z = new ObjectModule('O', 0, 1, GrowthList.LOGISTIC, ObjectBuilder.OBJECT_PATH + ObjectList.SIMPLE_LEAF);
        om_right_z.scale = Vector3.one * 0.1f;
        om_right_z.rotation = new Vector3(90, 90, 0);

        JointModule jmc = new JointModule('{', 0, 1, GrowthList.LINEAR, JointBuilder.JOINT_PATH + JointModule.CHARACTER_JOINT);
        jmc.mass = 1;
        
        jmc.jointSpringSpring = 10f;
		jmc.jointSpringDamper = 99f;

		jmc.jointLimitMin = -10;
		jmc.jointLimitMax = 10;

		jmc.twistLimit1 = -10;
		jmc.twistLimit2 = 10;

		jmc.twistSpringSpring = 10f;
		jmc.twistSpringDamper = 99f;

        LineModule ln = new LineModule('F', 0, 1, GrowthList.EXPONENTIAL);
		ln.LineWidth = 0.1f;
		ln.LineLength = 0.1f;
        ln.jointed = true;

        ObjectModule om_z = new ObjectModule('O', 0, 1, GrowthList.LINEAR, ObjectBuilder.OBJECT_PATH + ObjectList.SIMPLE_LEAF);
        om_z.scale = Vector3.one * 0.1f;
        om_z.rotation = new Vector3(90, -90, 0);

		List<SystemModule> lm = new List<SystemModule>();
        lm.Add(jmc.CopyModule());
		lm.Add(ln.CopyModule());
        lm.Add(new SystemModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL));
		lm.Add(jm.CopyModule());
		lm.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(1, 0, 0), 45f));
		lm.Add(om_right.CopyModule());
        lm.Add(new SystemModule('}', 0, 1, GrowthList.NON_DEVELOPMENTAL));
        lm.Add(new SystemModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL));
        lm.Add(new SystemModule('1', 0, 1, GrowthList.NON_DEVELOPMENTAL));
        lm.Add(new SystemModule('}', 0, 1, GrowthList.NON_DEVELOPMENTAL));

		Productions.Add('1', lm);
	}

	public bool newString = true;
	private string derivationString = "";
	public string DerivationString()
	{
		if(newString)
		{
			derivationString = "";
			foreach (SystemModule m in returnList)
				derivationString += m.Symbol;
			newString = false;
		}

		return derivationString;
	}

	public void TogglePhysics ()
	{
		Joint[] JointStack = GetComponentsInChildren<Joint> ();
		for (int i = JointStack.Length - 1; i >= 0; i--) 
		{
			Rigidbody thisBody = JointStack [i].GetComponent<Rigidbody> ();
			//thisBody.angularVelocity = Vector3.zero;
			//thisBody.velocity = Vector3.zero;
			thisBody.isKinematic = !thisBody.isKinematic;
			thisBody.angularVelocity = Vector3.zero;
			thisBody.velocity = Vector3.zero;
			if(JointStack[i] is HingeJoint)
			{
				Debug.Log((JointStack[i] as HingeJoint).angle);
				Debug.Log((JointStack[i] as HingeJoint).velocity);
			}
		}
	}

	public void ImportDerivation ()
	{
		string getit = ModuleHolder.SAVE_PATH + "/" + this.gameObject.name + "/" + this.gameObject.name + ModuleHolder.PATH_TAIL;
		storedModules = Resources.Load(getit) as ModuleHolder;
		if(storedModules != null)
		{
			storedModules.LoadModules(this);	//Dont age the system, just build all the modulez
		}
		else
			Debug.Log("No holder found");
	}

	public void ImportProductions ()
	{
		string getit = ModuleHolder.SAVE_PATH + "/" + this.gameObject.name + "/" + this.gameObject.name + ModuleHolder.PATH_TAIL;
		storedModules = Resources.Load(getit) as ModuleHolder;
        Debug.Log(getit);
		if(storedModules != null)
		{
			storedModules.LoadProductions(this);
		}
		else
			Debug.Log("No holder found");
	}

	public void ImportDerivationAndProductions ()
	{
		string getit = ModuleHolder.SAVE_PATH + "/" + this.gameObject.name + "/" + this.gameObject.name + ModuleHolder.PATH_TAIL;
		storedModules = Resources.Load(getit) as ModuleHolder;
		if(storedModules != null)
		{
			storedModules.LoadProductions(this);
			storedModules.LoadModules(this);	//Dont age the system, just build all the modulez
			Turtle.TurtleAnalysis(0f);	
			Maturity = MAX_MATURITY;
		}
		else
			Debug.Log("No holder found");
	}

	public string PrintSystem ()
	{
		string axiom = "\n" + "Axiom: ";
		foreach (SystemModule m in returnList)
			axiom += m.Symbol + " ";

		string output = "\n" + "Productions:\n";
		foreach (KeyValuePair<char, List<SystemModule>> production in Productions)
		{
			string RHS = "";
			foreach(SystemModule m in production.Value)
			{
				Debug.Log(m);
				RHS += m.Symbol + " ";
			}
			output += production.Key.ToString () + " --> " + RHS + "\n";
		}
		Debug.Log(axiom + output);
		return axiom + output;
	}

    public string PrintDerivation()
    {
        string axiom = "";
        foreach (SystemModule m in returnList)
            axiom += m.Symbol + " ";
        return axiom;
    }
}