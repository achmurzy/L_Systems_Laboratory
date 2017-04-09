using UnityEngine;
using System.Collections;

public class LaboratoryController : MonoBehaviour 
{
	public LaboratoryCamera LabCam;
	public Szilard_L_System activeSystem;
	public Szilard_L_System generatedSystem;
	public TransformDOL.RawDOLSystem LastRawDOL;
	public GameObject BlankPivot;

	RaycastHit info;

	public static SzilardAlgorithm sAlgorithm;
	public static TransformDOL tDOL;

	void Awake()
	{
		LabCam = FindObjectOfType<LaboratoryCamera> ();
	}

	// Use this for initialization
	void Start () 
	{
		sAlgorithm = new SzilardAlgorithm ();
		tDOL = new TransformDOL ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		ScreenCast ();
	}

	private void ScreenCast()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray clickCast = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(clickCast, out info))
			{
				activeSystem = info.collider.gameObject.GetComponentInParent<Szilard_L_System>();
				if(activeSystem != null)
					LabCam.SetFocus(activeSystem.GetComponentInChildren<Szilard_Turtle>().gameObject);
			}
		}

		if(activeSystem != null && Input.GetKeyDown(KeyCode.Space))
		{
			activeSystem.GrowthControl();
		}
	}

	public void SetSystem(Szilard_L_System sls)
	{
		if(generatedSystem != null)
			GameObject.Destroy (generatedSystem.gameObject);
		generatedSystem = sls;
		generatedSystem.transform.position = BlankPivot.transform.position;
	}
}
