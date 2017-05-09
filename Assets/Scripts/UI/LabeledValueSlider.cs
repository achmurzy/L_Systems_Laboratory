using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LabeledValueSlider : MonoBehaviour 
{
	private Slider slider;
    public Slider Slider { get { return slider; } }
	private Text text;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        text = transform.GetChild(2).GetComponent<Text>();
        slider.onValueChanged.AddListener(delegate { text.text = slider.value.ToString(); });
    }

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void UpdateValue ()
	{
		text.text = slider.value.ToString();
	}
}
