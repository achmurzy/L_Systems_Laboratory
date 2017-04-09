using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LabeledValueSlider : MonoBehaviour 
{
	public Slider slider;
	public Text text;

	// Use this for initialization
	void Start () 
	{
		slider = GetComponentInChildren<Slider>();
		text = transform.GetChild(2).GetComponent<Text>();
		slider.onValueChanged.AddListener( delegate {	text.text = slider.value.ToString();	} );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void UpdateValue ()
	{
		text.text = slider.value.ToString();
	}
}
