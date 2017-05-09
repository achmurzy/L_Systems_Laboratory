using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Production : MonoBehaviour 
{
	public char LHS { get; private set; }
	public UIModule SelectedModule { get; set; }
	public List<UIModule> RHS { get; private set; }

    public GameObject panelLHS;
	public Dropdown currentLHS;
    public GameObject panelRHS;

	void Awake ()
	{
		RHS = new List<UIModule>();
        panelRHS = this.transform.GetChild(1).gameObject;
        SelectedModule = this.GetComponentInChildren<UIModule>();
        SelectedModule.Production = this;
        RHS.Add(this.GetComponentInChildren<UIModule>());
	}

	// Use this for initialization
	void Start () 
	{
		ModuleBuilder.FillSymbolDropdown(ref currentLHS, typeof(Parametric_Turtle));
        currentLHS.onValueChanged.AddListener(delegate { SetSymbol(); });
		SetSymbol();
        Debug.Log(LHS);
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void AppendModule(SystemModule m)
	{
		SystemModule newMod = m.CopyModule();
        GameObject newRHS = GameObject.Instantiate(Resources.Load("Prefabs/UI/Module")) as GameObject;
        newRHS.transform.SetParent(panelRHS.transform, false);
        newRHS.transform.SetSiblingIndex(SelectedModule.gameObject.transform.GetSiblingIndex());
        newRHS.GetComponentInChildren<Text>().text = m.Symbol.ToString();

        UIModule uim = newRHS.GetComponent<UIModule>();
        SelectedModule = uim;
        RHS.Insert(SelectedModule.transform.GetSiblingIndex(), uim);
        uim.Production = this;
        uim.Module = newMod;
        uim.InstantiateUI();
	}

	public void RemoveSystemModule () 
	{		
		if(RHS.Count > 0)
		{
            int selectedIndex = SelectedModule.transform.GetSiblingIndex();
            if (selectedIndex != RHS.Count - 1) //Cannot remove last element
            {
                panelRHS.transform.GetChild(selectedIndex + 1).GetComponent<UIModule>().InstantiateUI();
                RHS.Remove(RHS[selectedIndex]); //this is very, very bad
                GameObject.Destroy(panelRHS.transform.GetChild(selectedIndex).gameObject); // because SelectedModule changes
            }   //and it isn't obvious that it happens
		}
	}

	public void SetAsCurrent()
	{
		Production cp = FindObjectOfType<ProductionBuilder>().currentProduction;
        if(cp != null)
            cp.panelRHS.GetComponent<Image>().color = Color.white;
        FindObjectOfType<ProductionBuilder>().currentProduction = this;
        this.panelRHS.GetComponent<Image>().color = Color.green;
	}

	public void SetSymbol()
	{
		LHS = ModuleBuilder.dropdownDictionary[currentLHS.value][0];
	}

	public static int SymbolIndexConverter(char c)
	{
		switch(c)
		{
			case '1':
				return Parametric_Turtle.PRODUCTION_ONE - 1;
			break;
			case '2':
				return Parametric_Turtle.PRODUCTION_TWO - 1;
			break;
			case '3':
				return Parametric_Turtle.PRODUCTION_THREE - 1;
			break;
			case '4':
				return Parametric_Turtle.PRODUCTION_FOUR - 1;
			break;
			case '5':
				return Parametric_Turtle.PRODUCTION_FIVE - 1;
			break;
			case Parametric_Turtle.DRAW:
				return 5;
			break;
			case Parametric_Turtle.OBJECT:
				return 6;
			break;
			case Parametric_Turtle.JOINT_OPEN:
				return 7;
			break;
			case Parametric_Turtle.JOINT_CLOSE:
				return 8;
			break;
			case Parametric_Turtle.BRANCH_OPEN:
				return 9;
			break;
			case Parametric_Turtle.BRANCH_CLOSE:
				return 10;
			break;
			case Parametric_Turtle.ROTATE:
				return 11;
			break;
			default:
				Debug.Log("Symbol not found in Turtle Alphabet");
				return -1;
		}
	}
}
