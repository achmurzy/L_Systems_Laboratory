using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RotationBuilder : MonoBehaviour 
{
	private RotationModule currentModule;

	public VectorUIElement rotationAxis;
	public LabeledValueSlider rotationSlider; 

	public const float MIN_ROTATION = -180;
	public const float MAX_ROTATION = 180;

	void Awake()
	{
		rotationSlider.Slider.maxValue = MAX_ROTATION;
		rotationSlider.Slider.minValue = MIN_ROTATION;
		rotationSlider.Slider.onValueChanged.AddListener( delegate { SetRotation(); } );
		rotationAxis.ValueDelegate(SetAxis);
	}

	public void Init (RotationModule rm) 
	{
		currentModule = rm;
		SetRotation();
		SetAxis();
	}

    public void SetUI(RotationModule rm)
    {
        currentModule = rm;
        rotationAxis.Vector = (rm.RotationAxis);
        rotationSlider.Slider.value = rm.RotationScalar;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetRotation ()
	{
		currentModule.RotationScalar = rotationSlider.Slider.value;
	}

	public void SetAxis()
	{
		currentModule.RotationAxis = rotationAxis.Vector;
	}
}
