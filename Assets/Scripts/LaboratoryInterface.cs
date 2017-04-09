using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LaboratoryInterface : MonoBehaviour 
{
	private LaboratoryController labControl;
	private GameObject[] PremadeSystems;
	
	private Slider PolynomialDegreeInput;
	private List<Slider> PolynomialCoefficients;

	public Button CoefficientGenerator;
	public Button OneRule, IndexRules, ReverseRules;
	public Button WBEButton;

	private GameObject CoefficientPrefab;
	private const float SliderOffset = 50f;
	private const float CoefficientRange = 10f;
	private Text MatrixText, DOLText, DerivationText;

	public bool getText = false;

	void Awake()
	{
		labControl = GameObject.FindObjectOfType<LaboratoryController> ();
		PolynomialCoefficients = new List<Slider> ();
		PolynomialDegreeInput = FindObjectOfType<Slider> ();
		PolynomialDegreeInput.value = 1;
		PolynomialDegreeInput.onValueChanged.AddListener(delegate{DegreeUpdate();});

		CoefficientPrefab = Resources.Load ("Prefabs/UI/SliderGroup") as GameObject;

		CoefficientGenerator.onClick.AddListener(delegate { SzilardAlgorithm(); } );
		OneRule.onClick.AddListener( delegate { MorphRawDOL(0); } );
		IndexRules.onClick.AddListener( delegate { MorphRawDOL(1); } );
		ReverseRules.onClick.AddListener( delegate { MorphRawDOL(2); } );
		WBEButton.onClick.AddListener ( delegate { WBE(); } );

		MatrixText = GameObject.Find ("MatrixBox").GetComponent<Text> ();
		DOLText = GameObject.Find ("SystemBox").GetComponent<Text> ();
		DerivationText = GameObject.Find ("DerivationBox").GetComponent<Text> ();
	}

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(labControl.generatedSystem != null)
			DerivationText.text = labControl.generatedSystem.DerivationString();
	}

	public void WBE ()
	{
		labControl.SetSystem(TransformDOL.BuildWBESystem(15, 2, 1, (1f/6f)));
	}

	public void SzilardAlgorithm()
	{
		float[] coeffs = new float[PolynomialCoefficients.Count];
		for (int i = 0; i < PolynomialCoefficients.Count; i++)
			coeffs [i] = PolynomialCoefficients [i].value;
		MatrixText.text = LaboratoryController.sAlgorithm.GetSzilardMatrix ((int)PolynomialDegreeInput.value, coeffs);
		TransformDOL.RawDOLSystem rawDOL = LaboratoryController.sAlgorithm.GenerateRawSystem ((int)PolynomialDegreeInput.value);
		DOLText.text = rawDOL.PrintSystem ();

		labControl.LastRawDOL = rawDOL;
		labControl.SetSystem (TransformDOL.ConvertRawSystem (rawDOL));
	}

	public void MorphRawDOL(int switchCase)
	{
		switch(switchCase)
		{
			case 0:
				TransformDOL.ProductionStrategy = TransformDOL.BuildOneRuleProductions;
			break;

			case 1:
				TransformDOL.ProductionStrategy = TransformDOL.BuildIndexedRules;
			break;

			case 2:
				TransformDOL.ProductionStrategy = TransformDOL.ReverseIndexedRules;
			break;
		
			default:
				Debug.Log ("How'd you get here?");
			break;
		}

		Szilard_L_System sls = TransformDOL.ConvertRawSystem (labControl.LastRawDOL);
		DOLText.text = sls.PrintSystem ();
		labControl.SetSystem (sls);
	}

	public void DegreeUpdate()
	{
		if(PolynomialCoefficients.Count > PolynomialDegreeInput.value)
		{
			while(PolynomialCoefficients.Count > PolynomialDegreeInput.value + 1)
			{
				Slider s = PolynomialCoefficients[PolynomialCoefficients.Count - 1];
				PolynomialCoefficients.RemoveAt(PolynomialCoefficients.Count - 1);
				Destroy(s.gameObject);
			}
		}
		else
		{
			while(PolynomialCoefficients.Count <= PolynomialDegreeInput.value)
				AddCoefficient();
		}
	}

	private void AddCoefficient()
	{
		GameObject slideObject;
		if (CoefficientPrefab != null)
			slideObject = GameObject.Instantiate (CoefficientPrefab);
		else
			return;

		Slider inField = slideObject.GetComponent<Slider> ();
		inField.value = 0;
		inField.wholeNumbers = false;
		inField.minValue = -CoefficientRange;
		inField.maxValue = CoefficientRange;

		PolynomialCoefficients.Add (inField);

		slideObject.AddComponent<LayoutElement> ();
		slideObject.transform.parent = PolynomialDegreeInput.transform;
		slideObject.transform.position = PolynomialDegreeInput.transform.position + 
			(Vector3.down * PolynomialCoefficients.Count * SliderOffset);
	}
}
