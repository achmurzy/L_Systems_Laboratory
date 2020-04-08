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
        LineModule lm = LineModule.CreateInstance<LineModule>();
        lm.Symbol = Symbol;
        lm.Age = Age;
        lm.TerminalAge = TerminalAge;
        lm.Growth = Growth;
        
		lm.DrawColor = DrawColor;
		lm.LineWidth = LineWidth;
		lm.LineLength = LineLength;
        lm.jointed = jointed;
		return lm;
	}

    public override void InstantiateModule(Parametric_Turtle turtle)
    {
        ModuleObject = GameObject.Instantiate(Resources.Load(LineModule.STEM_PATH), turtle.p_l_sys.gameObject.transform) as GameObject;
        ModuleObject.name = "LineSystemModule";

        this.UpdateModule(turtle);
    }

    public override void UpdateModule(Parametric_Turtle turtle)
    {
        float width = GrowthFunction(LineWidth);
        float length = GrowthFunction(LineLength);
        ModuleObject.transform.localScale = new Vector3(width, length, width);

        Vector3 heading = length * turtle.transform.up;

        /*if (jointed)
        {
            Joint thisJoint = jointStack.Peek();    //Assume every branch is jointed
            if (thisJoint is CharacterJoint)
            {
                CharacterJoint cj = thisJoint as CharacterJoint;
                cj.swingAxis = Vector3.Cross(p_l_sys.transform.up, heading.normalized);
            }
            else
            {
                thisJoint.axis = Vector3.Cross(p_l_sys.transform.up, heading.normalized);
            }
            parent = thisJoint.gameObject;
            line.ObjectInitialize(parent);
            line.ModuleObject.transform.localPosition = Vector3.zero;
        }
        else*/
        {
            ModuleObject.transform.localPosition = turtle.transform.localPosition;
            ModuleObject.transform.up = turtle.transform.up;
        }

        turtle.objList.Add(ModuleObject);

        turtle.transform.localPosition += heading;
    }
}
