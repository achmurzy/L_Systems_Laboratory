using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ModuleBuilder : MonoBehaviour 
{
    public UIModule uiModule;
    private bool editModule = false;
    SystemModule currentModule { get { return uiModule.Module; } set { uiModule.Module = value; } }
	ProductionBuilder productions;

	List<GameObject> PanelList;

	public static Dictionary<int, string> dropdownDictionary;
    public static Dictionary<string, int> symbolDictionary;
	public Dropdown symbolDropdown;

	public GameObject DrawPanel;
	private LineBuilder lineBuilder;

	public GameObject JointPanel;
	private JointBuilder jointBuilder;

	public GameObject ObjectPanel;
	private ObjectBuilder objectBuilder;

	public GameObject RotationPanel;
	private RotationBuilder rotationBuilder;

	public GameObject GrowthPanel;
	private GrowthBuilder growthBuilder;

	public const float AGE_MIN = -1f, AGE_MAX = 10f;

    public GameObject TerminalSlider;
	private LabeledValueSlider terminalAge;

	void Awake()
	{
        dropdownDictionary = new Dictionary<int, string>();
        symbolDictionary = new Dictionary<string, int>();
		FillSymbolDropdown(ref symbolDropdown, ref dropdownDictionary, typeof(Parametric_Turtle));
        foreach (KeyValuePair<int, string> kvp in dropdownDictionary)
        { symbolDictionary[kvp.Value] = kvp.Key; }

		PanelList = new List<GameObject>();
		PanelList.Add(ObjectPanel);
		PanelList.Add(DrawPanel);
		PanelList.Add(JointPanel);
		PanelList.Add(RotationPanel);

        lineBuilder = DrawPanel.GetComponent<LineBuilder>();
        objectBuilder = ObjectPanel.GetComponent<ObjectBuilder>();
        jointBuilder = JointPanel.GetComponent<JointBuilder>();
        rotationBuilder = RotationPanel.GetComponent<RotationBuilder>();
        growthBuilder = GrowthPanel.GetComponent<GrowthBuilder>();
	}

	// Use this for initialization
	void Start () 
	{
		productions = FindObjectOfType<ProductionBuilder>();
        terminalAge = TerminalSlider.GetComponent<LabeledValueSlider>();
		terminalAge.Slider.onValueChanged.AddListener ( delegate {	SetTerminal();	} );
		terminalAge.Slider.minValue = AGE_MIN;
		terminalAge.Slider.maxValue = AGE_MAX;

        symbolDropdown.onValueChanged.AddListener(delegate { SetSymbol(); });
        productions.currentProduction.GetComponentInChildren<UIModule>().InstantiateUI();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

    public void FillBuilderUI(UIModule uim)
    {
        if (uim != uiModule)
        {
            Debug.Log("Symbol: " + uim.Module.Symbol);
            Debug.Log("Growth: " + uim.Module.Growth);
            uim.GetComponent<Image>().color = Color.green;
            if (uiModule != null)
                uiModule.GetComponent<Image>().color = Color.white;
            uiModule = uim;
            editModule = true;  //Distinguish between true edit and UI change
            DisablePanels();
            if (productions.currentProduction.RHS.IndexOf(uim) != productions.currentProduction.RHS.Count - 1)
            {
                symbolDropdown.gameObject.SetActive(true);
                switch (currentModule.Symbol)
                {
                    case Parametric_Turtle.DRAW:
                        DrawPanel.gameObject.SetActive(true);
                        lineBuilder.SetUI(currentModule as LineModule);
                        break;
                    case Parametric_Turtle.JOINT_OPEN:
                        JointPanel.gameObject.SetActive(true);
                        jointBuilder.SetUI(currentModule as JointModule);
                        break;
                    case Parametric_Turtle.OBJECT:
                        ObjectPanel.gameObject.SetActive(true);
                        objectBuilder.SetUI(currentModule as ObjectModule);
                        break;
                    case Parametric_Turtle.ROTATE:
                        RotationPanel.gameObject.SetActive(true);
                        rotationBuilder.SetUI(currentModule as RotationModule);
                        break;
                    default:
                        break;
                }
                symbolDropdown.value = symbolDictionary[currentModule.Symbol.ToString()]; //Will trigger SetSymbol
                terminalAge.Slider.value = currentModule.TerminalAge;
                growthBuilder.SetUI(currentModule);
            }
            else
                symbolDropdown.gameObject.SetActive(false);
            editModule = false;
        }
    }

	public void SetSymbol ()
	{
        if (!editModule)
        {
            if (productions.currentProduction != null) //Reflect symbol change on UIModule
            {
                uiModule.GetComponentInChildren<Text>().text =
                     dropdownDictionary[symbolDropdown.value];
            }
            DisablePanels();
            switch (dropdownDictionary[symbolDropdown.value][0])
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
        else
            editModule = false;
	}

	public void SetTerminal()
	{
		currentModule.TerminalAge = terminalAge.Slider.value;
	}

	public void InsertModule ()
	{
        productions.currentProduction.AppendModule(currentModule);
        symbolDropdown.value = symbolDictionary["-"];
	}

    public void RemoveModule()
    {
        productions.currentProduction.RemoveSystemModule();
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
