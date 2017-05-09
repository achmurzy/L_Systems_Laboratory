using UnityEngine;
using Vectrosity;
using System.Collections;

[System.Serializable]
public class LineModule : ObjectModule 
{
	public GameObject LineObject;
	public float LineWidth;
	public float LineLength;

	public const string MATERIAL_PATH = "Materials/LineMaterial";
    public Color DrawColor = Color.green;
	public Material LineMaterial;

    public const string STEM_PATH = "Prefabs/ModuleObjects/SimpleStem";

	public LineModule(){}
	public LineModule(char sym, float a, float term, int growth, string path = LineModule.STEM_PATH) : 
        base(sym, a, term, growth, path)
	{
		
	}

	public override SystemModule CopyModule()
	{
		LineModule lm = new LineModule(Symbol, Age, TerminalAge, Growth);

		lm.DrawColor = DrawColor;
		lm.LineWidth = LineWidth;
		lm.LineLength = LineLength;
        lm.jointed = jointed;
		return lm;
	}

	// Use this for initialization
	void Start () 
	{
	
	}

    public override GameObject ObjectInstantiate()
    {
        ModuleObject = GameObject.Instantiate(Resources.Load(LineModule.STEM_PATH)) as GameObject;
        ModuleObject.name = "LineSystemModule";
        return ModuleObject;
    }

    public override void ObjectInitialize(GameObject parent)
    {
        ModuleObject.transform.parent = parent.transform;
        float width = GrowthFunction(LineWidth);
        float length = GrowthFunction(LineLength);
        ModuleObject.transform.localScale = new Vector3(width, length, width);

    }
}
