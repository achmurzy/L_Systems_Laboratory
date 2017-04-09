using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LineBuilder : MonoBehaviour 
{
	public Toggle bezierToggle, jointToggle;
	public GameObject BezierPanel;
	public VectorUIElement controlPoint1, controlPoint2;
	public Slider resolutionSlider;
	public Slider widthSlider, lengthSlider;
	public ColorPicker colorPicker;

	public LineModule currentModule;

	public const float MAX_LINE_WIDTH = 2.5f;
	public const int MAX_SEGMENTS = 25;

	void Awake()
	{
		SetColor();

		bezierToggle.onValueChanged.AddListener ( delegate { ToggleBezier();	} );
		jointToggle.onValueChanged.AddListener ( delegate {	ToggleJointed();	} );

		widthSlider.maxValue = MAX_LINE_WIDTH;
		widthSlider.onValueChanged.AddListener( delegate { SetInitialWidth(); } );

		lengthSlider.maxValue = MAX_LINE_WIDTH;
		lengthSlider.onValueChanged.AddListener(delegate {	SetInitialLength();	} );

		resolutionSlider.maxValue = MAX_SEGMENTS;
		resolutionSlider.onValueChanged.AddListener ( delegate { SetResolution(); } );

		controlPoint1.ValueDelegate(SetControls);
		controlPoint2.ValueDelegate(SetControls);
	}

	public void Init (LineModule lm) 
	{
		currentModule = lm;
		ToggleBezier();
		ToggleJointed();
		SetColor();
		SetControls();
		SetResolution();
		SetInitialWidth();
		SetInitialLength();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void ToggleBezier()
	{
		currentModule.Bezier = bezierToggle.isOn;
		BezierPanel.SetActive(bezierToggle.isOn);
	}

	public void ToggleJointed ()
	{
		currentModule.Jointed = jointToggle.isOn;
	}

	public void SetControls()
	{
		currentModule.ControlPoint1 = controlPoint1.Vector;
		currentModule.ControlPoint2 = controlPoint2.Vector;
	}

	public void SetResolution()
	{
		currentModule.bezierResolution = (int)resolutionSlider.value;
	}

	public void SetInitialWidth ()
	{
		currentModule.LineWidth = widthSlider.value;
	}

	public void SetInitialLength ()
	{
		currentModule.LineLength = lengthSlider.value;
	}

	public void SetColor()
	{
		if(currentModule != null)
			currentModule.DrawColor = colorPicker.CurrentColor;
	}
}
