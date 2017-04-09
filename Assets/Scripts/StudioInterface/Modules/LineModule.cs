using UnityEngine;
using Vectrosity;
using System.Collections;

[System.Serializable]
public class LineModule : SystemModule 
{
	public GameObject LineObject;
	public VectorLine Line;
	public Vector3 Origin, End;
	public Color DrawColor = Color.green;
	public float LineWidth;
	public float LineLength;

	public bool Bezier = false, Jointed = false;
	public int bezierResolution = 10;
	public Vector3 ControlPoint1, ControlPoint2;

	public const string MATERIAL_PATH = "Materials/LineMaterial";
	public Material LineMaterial;

	public LineModule(){}
	public LineModule(char sym, float a, float term, int growth) : base(sym, a, term, growth)
	{
		
	}

	public override SystemModule CopyModule()
	{
		LineModule lm = new LineModule(Symbol, Age, TerminalAge, Growth);
		lm.Bezier = Bezier;
		lm.Jointed = Jointed;

        if(Bezier)
        {
			lm.ControlPoint1 = ControlPoint1;
			lm.ControlPoint2 = ControlPoint2;
        }

		lm.DrawColor = DrawColor;
		lm.LineWidth = LineWidth;
		lm.LineLength = LineLength;
		return lm;
	}

	// Use this for initialization
	void Start () 
	{
		MakeLine();
	}

	public void MakeLine()
	{
		if(Bezier)
		{
			LineMaterial = Instantiate(Resources.Load(MATERIAL_PATH)) as Material;
			LineMaterial.color = DrawColor;
			Line = new VectorLine("BezierLine", new Vector3[bezierResolution + 1], 
								LineMaterial, LineWidth, LineType.Continuous, Joins.Weld);
			
			Line.MakeCurve(Origin, ControlPoint1, End, ControlPoint2, bezierResolution);
		}
		else
		{
			Line = VectorLine.SetLine3D(DrawColor, Origin, End);
		}
	}

	void OnDestroy()
	{
		VectorLine.Destroy(ref Line);
	}
}
