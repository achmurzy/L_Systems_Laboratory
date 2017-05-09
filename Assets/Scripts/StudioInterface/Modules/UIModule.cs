using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIModule : MonoBehaviour 
{
    public Production Production { get; set; }
    public SystemModule Module { get; set; }

    void Awake()
    {
        Module = new SystemModule();
        Module.Symbol = Parametric_Turtle.EMPTY;
        GetComponentInChildren<Text>().text = "-";
    }

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InstantiateUI()
    {
        Production.SelectedModule = this;
        FindObjectOfType<ModuleBuilder>().FillBuilderUI(this);
        Production.SetAsCurrent();
    }
}
