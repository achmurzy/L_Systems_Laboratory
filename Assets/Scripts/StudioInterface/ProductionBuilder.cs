using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ProductionBuilder : MonoBehaviour 
{
	Parametric_L_System parametricSystem;
	public string SystemName { get; private set; }
	public const string PREFAB_PATH = "Assets/Resources/Prefabs/L_Systems/Jointed/";

	public GameObject ProductionPanel;
	public List<GameObject> productionSet;
	public const string PRODUCTION_PATH = "Prefabs/UI/Production";
	public const int MAX_PRODUCTIONS = 10;
	public Production currentProduction;

	private InputField Name;
	private Text OutputArea;

	void Awake()
	{
	    productionSet = new List<GameObject>();
        GetComponentInChildren<Production>().SetAsCurrent();
        productionSet.Add(currentProduction.gameObject);
	}

	// Use this for initialization
	void Start () 
	{
        Name = GetComponentInChildren<InputField>();
        Name.onEndEdit.AddListener(delegate { SetName(); });
        SystemName = "Default";
        OutputArea = GameObject.Find("OutputArea").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (parametricSystem != null) 
		{
			if (parametricSystem.inGrowth) 
			{
				OutputArea.text = parametricSystem.DerivationString ();
			}
		}
	}

	public void SetName()
	{
		SystemName = Name.text;
		if(parametricSystem != null)
			parametricSystem.name = SystemName;
	}

	public void AddProduction ()
	{
		if(productionSet.Count < MAX_PRODUCTIONS)
		{
			GameObject prod = GameObject.Instantiate(Resources.Load(PRODUCTION_PATH)) as GameObject;
			prod.transform.SetParent(ProductionPanel.transform, false);
			prod.transform.localPosition = ProductionPanel.GetComponent<RectTransform>().sizeDelta/2;
			productionSet.Add(prod);

			prod.GetComponentInChildren<Production>().SetAsCurrent();
            currentProduction.RHS[0].InstantiateUI();
		}
	}

	public void RemoveProduction()
	{
		//if(productionSet.Count > 0)
		if(currentProduction != null && productionSet.Count > 0)
		{	
			GameObject go = currentProduction.gameObject;
			productionSet.Remove(go);
			GameObject.Destroy(go);
			int index = productionSet.Count - 1;
			if(index > -1)
				currentProduction = productionSet[index].GetComponent<Production>();
		}
	}

	public void ClearProductions()
	{
		while(productionSet.Count > 0)
		{
			RemoveProduction();
		}
	}

	public void InstantiateSystem()
	{
		if(parametricSystem != null)               //Set-up object
			GameObject.Destroy(parametricSystem.gameObject);
		GameObject system = GameObject.Instantiate(Resources.Load("Prefabs/L_Systems/BlankParametric")) as GameObject;
        system.transform.position = Vector3.zero;
        system.transform.Rotate(new Vector3(0, 90, 0));
		parametricSystem = system.GetComponent<Parametric_L_System>();
        parametricSystem.name = SystemName;
        FindObjectOfType<LaboratoryCamera>().SetFocus(parametricSystem.gameObject);

        foreach (GameObject go in productionSet) //Add productions
        {
            Production pp = go.GetComponent<Production>();
            List<SystemModule> moduleList = new List<SystemModule>();
            foreach (UIModule uim in pp.RHS)
            {
                if (uim.Module.Symbol != Parametric_Turtle.EMPTY)
                    moduleList.Add(uim.Module);
            }
            parametricSystem.Productions.Add(pp.LHS, moduleList);
        }
        OutputArea.text = parametricSystem.PrintSystem();
	}

	public void SaveSystem()
	{
        InstantiateSystem();
        if (!AssetDatabase.IsValidFolder("Assets/Resources/" + ModuleHolder.SAVE_PATH + "/" + parametricSystem.name))
        {
            ModuleHolder mh = ScriptableObject.CreateInstance<ModuleHolder>();
            mh.StoreLSystem(parametricSystem);
            PrefabUtility.CreatePrefab(PREFAB_PATH + parametricSystem.name + "/" +
                                        parametricSystem.name + ".prefab", parametricSystem.gameObject);
            parametricSystem.enabled = false;
        }
        else
            Debug.Log("A system so named already existed; rename your specimen");
	}

	public void DefaultSystem()
	{
		InstantiateSystem();
		parametricSystem.BuildDefaultSystem();
		parametricSystem.gameObject.transform.position = new Vector3(0, 0, 0);
		FindObjectOfType<LaboratoryCamera>().SetFocus(parametricSystem.gameObject);
		OutputArea.text = parametricSystem.PrintSystem();
	}

    public void LoadSystem()
    {
		ClearProductions();
        InstantiateSystem();
		parametricSystem.name = SystemName;
        parametricSystem.ImportProductions();

        foreach(KeyValuePair<char, List<SystemModule>> kvp in parametricSystem.Productions)
        {	
        	AddProduction();
        	currentProduction.currentLHS.value = Production.SymbolIndexConverter(kvp.Key);
    		for(int i = kvp.Value.Count; i > 0; i--) 
    		{
                SystemModule sm = kvp.Value[i - 1];
    			currentProduction.AppendModule(sm);
    		}
        }
    }
}
