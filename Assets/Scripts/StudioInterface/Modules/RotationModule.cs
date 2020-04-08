using UnityEngine;
using System.Collections;

[System.Serializable]
public class RotationModule : SystemModule 
{
	public Vector3 RotationAxis;
	public float RotationScalar;
    public bool global = false;

	public RotationModule(){}
    public RotationModule(char sym, float a, float term, int growth, Vector3 axis, float rot) : base(sym, a, term, growth)
    {
        RotationAxis = axis;
        RotationScalar = rot;
    }

    public RotationModule(char sym, float a, float term, int growth, Vector3 axis, float rot, bool glob) : base(sym, a, term, growth)
	{
		RotationAxis = axis;
		RotationScalar = rot;
        global = glob;
	}
	public override SystemModule CopyModule()
	{
        RotationModule jm = RotationModule.CreateInstance<RotationModule>();
        jm.Symbol = Symbol;
        jm.Age = Age;
        jm.TerminalAge = TerminalAge;
        jm.Growth = Growth;
        jm.RotationAxis = RotationAxis;
        jm.RotationScalar = RotationScalar;

        jm.global = global;
        return jm;
	}

    public override void InstantiateModule(Parametric_Turtle turtle)
    { }

    public override void UpdateModule(Parametric_Turtle turtle)
    {
        if(global)
            turtle.transform.localRotation = Quaternion.AngleAxis(GrowthFunction(RotationScalar), RotationAxis);
        else
            turtle.transform.Rotate(RotationAxis, GrowthFunction(RotationScalar));
    }
}
