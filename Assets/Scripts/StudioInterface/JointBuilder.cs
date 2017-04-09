using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JointBuilder : MonoBehaviour 
{
	public const string JOINT_PATH = "Prefabs/JointObjects/";

	public static Dictionary<int, string> jointDictionary;
	public JointModule currentModule;

	public Dropdown jointDropdown;

	public const float LIMIT_MIN = -180, LIMIT_MAX = 180, SPRING_MAX = 100;

	public Slider JointLimitMin, JointLimitMax, JointSpring, JointDamper;
	public GameObject CharacterJoint;
	public Slider CharacterJointLimitMin, CharacterJointLimitMax, CharacterJointSpring, CharacterJointDamper;
	public VectorUIElement TwistAxis;

	public Toggle Gravity;
	public Slider Mass, Drag, AngularDrag;

	void Awake()
	{
		jointDropdown.onValueChanged.AddListener ( delegate {	UpdateJoint();	});
		jointDictionary = new Dictionary<int, string>();
		ModuleBuilder.FillSymbolDropdown(ref jointDropdown, ref jointDictionary, typeof(JointModule));

		TwistAxis.ValueDelegate(SetTwistAxis);

		JointLimitMin.onValueChanged.AddListener ( delegate {	SetJointLimitMin();	} );
		JointLimitMin.minValue = LIMIT_MIN;
		JointLimitMin.maxValue = 0f;
		JointLimitMax.onValueChanged.AddListener ( delegate {	SetJointLimitMax();	} );
		JointLimitMax.minValue = 0f;
		JointLimitMax.maxValue = LIMIT_MAX;

		JointSpring.onValueChanged.AddListener ( delegate {		SetSpring();	} );
		JointSpring.maxValue = SPRING_MAX;
		JointDamper.onValueChanged.AddListener ( delegate {		SetDamper();	} );
		JointDamper.maxValue = SPRING_MAX;

		CharacterJointLimitMin.onValueChanged.AddListener ( delegate {	SetTwistLimitMin();	} );
		CharacterJointLimitMin.minValue = LIMIT_MIN;
		CharacterJointLimitMin.maxValue = 0f;
		CharacterJointLimitMax.onValueChanged.AddListener ( delegate {	SetTwistLimitMax();	} );
		CharacterJointLimitMax.minValue = 0f;
		CharacterJointLimitMax.maxValue = LIMIT_MAX;

		CharacterJointSpring.onValueChanged.AddListener ( delegate {	SetTwistSpring();	} );
		CharacterJointSpring.maxValue = SPRING_MAX;
		CharacterJointDamper.onValueChanged.AddListener ( delegate {	SetTwistDamper();	} );
		CharacterJointDamper.maxValue = SPRING_MAX;

		Gravity.onValueChanged.AddListener ( delegate {		SetGravity();	} );
		Mass.onValueChanged.AddListener ( delegate {		SetMass();	} );
		Drag.onValueChanged.AddListener ( delegate {		SetDrag();	} );
		AngularDrag.onValueChanged.AddListener ( delegate {		SetAngularDrag();	} );
	}

	// Use this for initialization
	public void Init (JointModule jm) 
	{
		currentModule = jm;
		SetJoint();
		UpdateJoint();

		SetJointLimitMin ();
		SetJointLimitMax();
		SetDamper();
		SetSpring();

		SetTwistAxis();
		SetTwistLimitMin ();
		SetTwistLimitMax();
		SetTwistDamper();
		SetTwistSpring();

		SetMass();
		SetGravity();
		SetDrag();
		SetAngularDrag();
	}
	
	// Update is called once per frame
	void Update () 
	{
			
	}

	public void SetJoint()
	{
		currentModule.ObjectPath = JOINT_PATH + jointDictionary[jointDropdown.value];
	}

	public void UpdateJoint ()
	{
		if(jointDictionary[jointDropdown.value] == JointModule.CHARACTER_JOINT)
		{
			CharacterJoint.SetActive(true);
		}
		else 
		{
			CharacterJoint.SetActive(false);
		}
		SetJoint();
	}

	public void SetJointLimitMin ()
	{ 	currentModule.jointLimitMin = JointLimitMin.value;	}
	public void SetJointLimitMax()
	{	currentModule.jointLimitMax = JointLimitMax.value;	}
	public void SetDamper()
	{	currentModule.jointSpringDamper = JointDamper.value;	}
	public void SetSpring()
	{	currentModule.jointSpringSpring = JointSpring.value; 	}

	public void SetTwistAxis()
	{	currentModule.twistAxis = TwistAxis.Vector;	}

	public void SetTwistLimitMin ()
	{ 	currentModule.twistLimit1 = CharacterJointLimitMin.value;	}
	public void SetTwistLimitMax()
	{	currentModule.twistLimit2 = CharacterJointLimitMax.value;	}
	public void SetTwistDamper()
	{	currentModule.twistSpringDamper = CharacterJointDamper.value;	}
	public void SetTwistSpring()
	{	currentModule.twistSpringSpring = CharacterJointSpring.value; 	}

	public void SetGravity()
	{	currentModule.gravity = Gravity.isOn; }
	public void SetMass()
	{	currentModule.mass = Mass.value;	}
	public void SetDrag()
	{	currentModule.drag = Drag.value;	}
	public void SetAngularDrag()
	{	currentModule.angularDrag = AngularDrag.value;	}
}
