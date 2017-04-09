using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RotationBuilder : MonoBehaviour 
{
	public RotationModule currentModule;

	public VectorUIElement rotationAxis;
	public Slider rotationSlider; 

	public const float MIN_ROTATION = -180;
	public const float MAX_ROTATION = 180;

	void Awake()
	{
		rotationSlider.maxValue = MAX_ROTATION;
		rotationSlider.minValue = MIN_ROTATION;
		rotationSlider.onValueChanged.AddListener( delegate { SetRotation(); } );
		rotationAxis.ValueDelegate(SetAxis);
	}

	public void Init (RotationModule rm) 
	{
		currentModule = rm;
		SetRotation();
		SetAxis();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetRotation ()
	{
		currentModule.RotationScalar = rotationSlider.value;
	}

	public void SetAxis()
	{
		currentModule.RotationAxis = rotationAxis.Vector;
	}
}
