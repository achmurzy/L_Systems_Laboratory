using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ModuleBuilder : MonoBehaviour 
{
	SystemModule currentModule;
	ProductionBuilder productions;

	List<GameObject> PanelList;

	public static Dictionary<int, string> dropdownDictionary;
	public Dropdown symbolDropdown;

	public GameObject DrawPanel;
	public LineBuilder lineBuilder;

	public GameObject JointPanel;
	public JointBuilder jointBuilder;

	public GameObject ObjectPanel;
	public ObjectBuilder objectBuilder;

	public GameObject RotationPanel;
	public RotationBuilder rotationBuilder;

	public GameObject GrowthPanel;
	public GrowthBuilder growthBuilder;

	public const float AGE_MIN = -1f, AGE_MAX = 10f;
	public Slider terminalAge;
	public Button addButton;

	void Awake()
	{
		currentModule = new SystemModule();
		dropdownDictionary = new Dictionary<int, string>();

		FillSymbolDropdown(ref symbolDropdown, ref dropdownDictionary, typeof(Parametric_Turtle));

		PanelList = new List<GameObject>();
		PanelList.Add(ObjectPanel);
		PanelList.Add(DrawPanel);
		PanelList.Add(JointPanel);
		PanelList.Add(RotationPanel);
	}

	// Use this for initialization
	void Start () 
	{
		productions = FindObjectOfType<ProductionBuilder>();

		terminalAge.onValueChanged.AddListener ( delegate {	SetTerminal();	} );
		terminalAge.minValue = AGE_MIN;
		terminalAge.maxValue = AGE_MAX;

		SetSymbol();
		currentModule.Age = 0f;
		currentModule.TerminalAge = AGE_MAX;

		SetTerminal();
		DisablePanels();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	public void SetSymbol ()
	{
		DisablePanels();
		switch(dropdownDictionary[symbolDropdown.value][0])
		{
			case Parametric_Turtle.DRAW:
				currentModule = new LineModule();
				lineBuilder.Init(currentModule as LineModule);
				DrawPanel.gameObject.SetActive(true);
				break;
			case Parametric_Turtle.JOINT_OPEN:
				currentModule = new JointModule();
				jointBuilder.Init(currentModule as JointModule);
				JointPanel.gameObject.SetActive(true);
				break;
			case Parametric_Turtle.OBJECT:
				currentModule = new ObjectModule();
				objectBuilder.Init(currentModule as ObjectModule);
				ObjectPanel.gameObject.SetActive(true);
				break;
			case Parametric_Turtle.ROTATE:
				currentModule = new RotationModule();
				rotationBuilder.Init(currentModule as RotationModule);
				RotationPanel.gameObject.SetActive(true);
				break;
			default:
				currentModule = new SystemModule();
				break;		
		}

		SetTerminal();
		growthBuilder.Init(currentModule);
		currentModule.Symbol = dropdownDictionary[symbolDropdown.value][0];
	}

	public void SetTerminal()
	{
		currentModule.TerminalAge = terminalAge.value;
	}

	public void AddModule ()
	{
		productions.currentProduction.AppendModule(currentModule);
	}

	public static void FillSymbolDropdown (ref Dropdown d, System.Type type)
	{
		FieldInfo[] theseConstants = GetConstants(type);
		List<string> options = new List<string>();

		for(int i = 0; i < theseConstants.Length; i++)
		{
			string symbol = (theseConstants[i].GetRawConstantValue().ToString());
			options.Add(symbol);
		}
		d.ClearOptions();
		d.AddOptions(options);
	}

	public static void FillSymbolDropdown (ref Dropdown d, ref Dictionary<int, string> dict, System.Type type)
	{
		FieldInfo[] theseConstants = GetConstants(type);
		List<string> options = new List<string>();

		for(int i = 0; i < theseConstants.Length; i++)
		{
			string symbol = (theseConstants[i].GetRawConstantValue().ToString());
			options.Add(symbol);
			dict.Add(i, symbol);
		}
		d.ClearOptions();
		d.AddOptions(options);
	}

	private void DisablePanels ()
	{
		foreach(GameObject go in PanelList)
		{
			go.SetActive(false);
		}
	}
		
	public static FieldInfo[] GetConstants(System.Type type)
	{
	    ArrayList constants = new ArrayList();

	    FieldInfo[] fieldInfos = type.GetFields(
	        // Gets all public and static fields

	        BindingFlags.Public | BindingFlags.Static | 
	        // This tells it to get the fields from all base types as well

	        BindingFlags.FlattenHierarchy);

	    // Go through the list and only pick out the constants
	    foreach(FieldInfo fi in fieldInfos)
	        // IsLiteral determines if its value is written at 
	        //   compile time and not changeable
	        // IsInitOnly determine if the field can be set 
	        //   in the body of the constructor
	        // for C# a field which is readonly keyword would have both true 
	        //   but a const field would have only IsLiteral equal to true
	        if(fi.IsLiteral && !fi.IsInitOnly)
	            constants.Add(fi);           

	    // Return an array of FieldInfos
	    return (FieldInfo[])constants.ToArray(typeof(FieldInfo));
	}
}
