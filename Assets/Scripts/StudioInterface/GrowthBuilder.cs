using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GrowthBuilder : MonoBehaviour 
{
	public const string NON_DEVELOPMENTAL = "NONDEVELOPMENTAL";
	public const string LOGISTIC = "LOGISTIC";
	public const string EXPONENTIAL = "EXPONENTIAL";
	public const string LINEAR = "LINEAR";

	public static Dictionary<int, string> growthDictionary;
	public Dropdown growthDropdown;
	public SystemModule currentModule;

	void Awake()
	{
		growthDictionary = new Dictionary<int, string>();
		growthDropdown.onValueChanged.AddListener ( delegate {	SetFunction();	});
		ModuleBuilder.FillSymbolDropdown(ref growthDropdown, ref growthDictionary, typeof(GrowthBuilder));
	}

	// Use this for initialization
	void Start () 
	{
		SetFunction();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void Init (SystemModule om) 
	{
		currentModule = om;
		SetFunction();
	}

	//Assumes a one-to-one correspondence between the ordering of the items in the
	//dropdown and the values of the growth function constants in GrowthList
	void SetFunction ()
	{
		currentModule.Growth = growthDropdown.value;
	}
}
