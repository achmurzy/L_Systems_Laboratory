using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LineBuilder : MonoBehaviour 
{
	private Toggle jointToggle;	
    public LabeledValueSlider widthSlider, lengthSlider;
	public ColorPicker colorPicker;

	private LineModule currentModule;

	public const float MAX_LINE_WIDTH = 1.0f;

	void Awake()
	{
		SetColor();
	}

    void Start()
    {
        jointToggle = GameObject.Find("JointToggle").GetComponent<Toggle>();
        jointToggle.onValueChanged.AddListener(delegate { ToggleJointed(); });

        widthSlider.Slider.value = 1;
        widthSlider.Slider.maxValue = MAX_LINE_WIDTH;
        widthSlider.Slider.onValueChanged.AddListener(delegate { SetInitialWidth(); });

        lengthSlider.Slider.value = 1;
        lengthSlider.Slider.maxValue = MAX_LINE_WIDTH;
        lengthSlider.Slider.onValueChanged.AddListener(delegate { SetInitialLength(); });
    }

	public void Init (LineModule lm) 
	{
		currentModule = lm;
		ToggleJointed();
		SetColor();
		SetInitialWidth();
		SetInitialLength();
	}

    public void SetUI(LineModule lm)
    {
        currentModule = lm;
        jointToggle.isOn = lm.jointed;
        widthSlider.Slider.value = lm.LineWidth;
        lengthSlider.Slider.value = lm.LineLength;
        colorPicker.CurrentColor = lm.DrawColor;
    }
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void ToggleJointed ()
	{
		currentModule.jointed = jointToggle.isOn;
	}

	public void SetInitialWidth ()
	{
		currentModule.LineWidth = widthSlider.Slider.value;
	}

	public void SetInitialLength ()
	{
		currentModule.LineLength = lengthSlider.Slider.value;
	}

	public void SetColor()
	{
		if(currentModule != null)
			currentModule.DrawColor = colorPicker.CurrentColor;
	}
}
