using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectBuilder : MonoBehaviour 
{
	public const string OBJECT_PATH = "Prefabs/ModuleObjects/";
	public static Dictionary<int, string> objectDictionary;
	public Dropdown objectDropdown;
	public ObjectModule currentModule;

	public VectorUIElement ObjectEuler, ObjectScale;

	public Toggle Trigger, Jointed;

	void Awake()
	{
		
	}

	void Start ()
	{
		objectDictionary = new Dictionary<int, string>();
		objectDropdown.onValueChanged.AddListener ( delegate {	SetPath();	});
		ModuleBuilder.FillSymbolDropdown(ref objectDropdown, ref objectDictionary, typeof(ObjectList));

		ObjectEuler.ValueDelegate(SetEuler);
		ObjectScale.ValueDelegate(SetScale);

		Trigger.onValueChanged.AddListener( delegate {	SetToggle();	});
		Jointed.onValueChanged.AddListener( delegate {	SetJointed();	});
	}

	public void Init (ObjectModule om) 
	{
		currentModule = om;
		SetPath();
		SetToggle();
		SetJointed();
		SetScale();
		SetEuler();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void SetPath()
	{
		currentModule.ObjectPath = OBJECT_PATH + objectDictionary[objectDropdown.value];
	}

	public void SetToggle ()
	{
		currentModule.trigger = Trigger.isOn;
	}

	public void SetJointed ()
	{
		currentModule.jointed = Jointed.isOn;
	}

	public void SetEuler()
	{
		currentModule.rotation = ObjectEuler.Vector;
	}

	public void SetScale()
	{
		currentModule.scale = ObjectScale.Vector;
		Debug.Log(ObjectScale.Vector);
	}
}
