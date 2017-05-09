using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectBuilder : MonoBehaviour 
{
	public const string OBJECT_PATH = "Prefabs/ModuleObjects/";
	private static Dictionary<int, string> objectDictionary;
    private static Dictionary<string, int> pathDictionary;
	private Dropdown objectDropdown;
	private ObjectModule currentModule;

	public VectorUIElement ObjectEuler, ObjectScale;

	public Toggle Trigger, Jointed;

	void Awake()
	{
		
	}

	void Start ()
	{
		objectDictionary = new Dictionary<int, string>();
        pathDictionary = new Dictionary<string, int>();
        objectDropdown = GetComponentInChildren<Dropdown>();
		objectDropdown.onValueChanged.AddListener ( delegate {	SetPath();	});
		ModuleBuilder.FillSymbolDropdown(ref objectDropdown, ref objectDictionary, typeof(ObjectList));
        foreach(KeyValuePair<int, string> kvp in objectDictionary)
        {
            pathDictionary[OBJECT_PATH+kvp.Value] = kvp.Key;
        }
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

    public void SetUI(ObjectModule om)
    {
        currentModule = om;
        objectDropdown.value = pathDictionary[om.ObjectPath];
        Trigger.isOn = om.trigger;
        Jointed.isOn = om.jointed;
        ObjectEuler.Vector = om.rotation;
        ObjectScale.Vector = om.scale;
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
