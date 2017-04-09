using UnityEngine;
using System.Collections;

[System.Serializable]
public class RotationModule : SystemModule 
{
	public Vector3 RotationAxis;
	public float RotationScalar;

	public RotationModule(){}
	public RotationModule(char sym, float a, float term, int growth, Vector3 axis, float rot) : base(sym, a, term, growth)
	{
		RotationAxis = axis;
		RotationScalar = rot;
	}

	public override SystemModule CopyModule()
	{
		RotationModule jm = new RotationModule(Symbol, Age, TerminalAge, Growth, RotationAxis, RotationScalar);
        return jm;
	}


}
