using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Szilard_L_System : MonoBehaviour 
{
	public List<SzilardModule> returnList;
	public Dictionary <char, List<SzilardModule>> productions;
	
	//**Geometric parameters**//
	public float currentBranchWidth = 3.0f;	public float BranchWidth { get { return currentBranchWidth; } 
																		set { currentBranchWidth = value; } }
	public float branchAngle = 45.0f;
	public float divergenceAngle = 60.0f;
	
	//**Associated rates**//
	//Experimentally determined exponential rate of growth
	public const float EXPONENTIAL_PARAM = 0.48f;
	
	public float segmentGrowth = 1.0f;
	public float widthGrowth = 1.0f;
	public float axlGrowth = 1.0f;
	
	public float age = 0.0f;
	public float maturity = 6.0f;
	public float growthSpeed = 0.1f;
	public bool inGrowth = false;

	public bool newString = true;
	private string derivationString = "";
	public string DerivationString()
	{
		if(newString)
		{
			derivationString = "";
			foreach (SzilardModule m in returnList)
				derivationString += m.symbol;
			newString = false;
		}

		return derivationString;
	}
	
	void Awake()
	{
		returnList = new List<SzilardModule> ();
		productions = new Dictionary<char, List<SzilardModule>> ();
	}
	
	// Use this for initialization
	public void Start () 
	{

	}
	
	// Update is called once per frame
	public void Update () 
	{
	transform.LookAt(Vector3.zero);
		if(Input.GetKey(KeyCode.LeftShift))
			if (age < maturity)
			{
				this.GetComponentInChildren<Szilard_Turtle>().TurtleAnalysis(returnList, Time.deltaTime);
				age += Time.deltaTime;
			}
	}

	IEnumerator GrowthRoutine()
	{
		while(true)
		{
			//yield return new WaitForSeconds(growthSpeed);
			yield return new WaitForFixedUpdate();
			if (age < maturity)
			{
				this.GetComponentInChildren<Szilard_Turtle>().TurtleAnalysis(returnList, Time.deltaTime);
				age += Time.deltaTime;
			}
			else
				StopCoroutine("GrowthRoutine");
		}
	}

	private void GrowthLogic()
	{

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

	public string PrintSystem()
	{
		string alphabet = "Alphabet: { ";

		string axiom = "\n" + "Axiom: ";
		foreach (SzilardModule m in returnList)
			axiom += m.symbol + " ";
		
		string output = "\n" + "Productions:\n";
		foreach (KeyValuePair<char, List<SzilardModule>> production in productions)
		{
			alphabet += production.Key + " ";

			string RHS = "";
			foreach(SzilardModule m in production.Value)
				RHS += m.symbol + " ";
			output += production.Key.ToString () + "-->" + RHS + "\n";
		}
		alphabet += "}";
		return alphabet + axiom + output;
	}
	
	public virtual float growthFunction(char sym, float gfi, float time)
	{
		return gfi*Mathf.Exp(EXPONENTIAL_PARAM*time);
	}	

}

public class SzilardModule
	{
		public char symbol { get; set; }
		public float age { get; set; }
	    public float terminalAge { get; set; }
	    public float growthParameter { get; set; }

		public SzilardModule(char sym, float a, float term, float gp)
		{
			symbol = sym;
			age = a;
			terminalAge = term;
			growthParameter = gp;
		}

		public SzilardModule(SzilardModule m)
		{
	        symbol = m.symbol;
	        age = m.age;
	        terminalAge = m.terminalAge;
	        growthParameter = m.growthParameter;
		}
	}


