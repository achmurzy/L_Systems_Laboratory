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

	public Button addButton;
	public Button removeButton;

	public InputField Name;

	public Text OutputArea;

	void Awake()
	{
		productionSet = new List<GameObject>();
	}

	// Use this for initialization
	void Start () 
	{
	
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
			prod.transform.parent = ProductionPanel.transform;
			prod.transform.localPosition = ProductionPanel.GetComponent<RectTransform>().sizeDelta/2;
			productionSet.Add(prod);

			currentProduction = prod.GetComponentInChildren<Production>();
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

	private void InstantiateSystem()
	{
		if(parametricSystem != null)
			GameObject.Destroy(parametricSystem.gameObject);
		GameObject system = GameObject.Instantiate(Resources.Load("Prefabs/L_Systems/BlankParametric")) as GameObject;
		parametricSystem = system.GetComponent<Parametric_L_System>();
	}

	public void BuildSystem()
	{
		InstantiateSystem();
		foreach(GameObject go in productionSet)
		{
			Production pp = go.GetComponent<Production>();
			parametricSystem.Productions.Add(pp.LHS, pp.RHS);

			string fuck = "";
			//foreach(SystemModule m in pp.RHS)
			//	fuck += m.Print() + "\n";
			//Debug.Log(pp.LHS + fuck);
		}
		parametricSystem.name = SystemName;
		parametricSystem.gameObject.transform.position = new Vector3(15, 0, 15);
		parametricSystem.gameObject.transform.LookAt(Vector3.zero);
		OutputArea.text = parametricSystem.PrintSystem();
	}

	public void SaveSystem()
	{
		if(!AssetDatabase.IsValidFolder("Assets/Resources/" + ModuleHolder.SAVE_PATH + "/" + parametricSystem.name))
        {
	        ModuleHolder mh = ScriptableObject.CreateInstance<ModuleHolder>();
			mh.StoreLSystem(parametricSystem);
			InstantiateSystem();
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
		parametricSystem.gameObject.transform.position = new Vector3(10, 2.5f, 10);
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
    		foreach(SystemModule sm in kvp.Value)
    		{
    			currentProduction.AppendModule(sm);
    		}
        }
    }
}
