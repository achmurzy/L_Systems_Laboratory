using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VectorUIElement : MonoBehaviour 
{
    public Vector3 Vector { get { return GetVector(); } }
    public delegate void VectorSetter();
    public VectorSetter VectorFunction;

    // Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ValueDelegate (VectorSetter vs)
	{
		VectorFunction = vs;
		foreach (InputField inF in this.GetComponentsInChildren<InputField>())
        {
        	inF.onValueChanged.RemoveAllListeners();
        	inF.contentType = InputField.ContentType.DecimalNumber;
     		inF.onEndEdit.AddListener( delegate { VectorSetterInvoke(); } );
        }
	}

	public void VectorSetterInvoke()
	{
		VectorFunction.Invoke();
	}

    private Vector3 GetVector()
    {
        int count = 0;
        Vector3 thisVec = new Vector3();
        foreach (InputField inF in this.GetComponentsInChildren<InputField>())
        {
     		thisVec[count] = float.Parse(inF.text, System.Globalization.NumberStyles.Float);
            count++;
        }
        return thisVec;
    }
}
