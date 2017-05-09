using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JointBuilder : MonoBehaviour 
{
	public const string JOINT_PATH = "Prefabs/JointObjects/";
    public const float LIMIT_MIN = -180, LIMIT_MAX = 180, SPRING_MAX = 100;

	public static Dictionary<int, string> jointDictionary;
	private JointModule currentModule;

	private Dropdown jointDropdown;

	public LabeledValueSlider JointLimitMin, JointLimitMax, JointSpring, JointDamper;
	public GameObject CharacterJoint;
	public LabeledValueSlider CharacterJointLimitMin, CharacterJointLimitMax, CharacterJointSpring, CharacterJointDamper;
	public VectorUIElement TwistAxis;

	public Toggle Gravity;
	public LabeledValueSlider Mass, Drag, AngularDrag;

	void Awake()
	{
        jointDropdown = this.GetComponentInChildren<Dropdown>();
		jointDropdown.onValueChanged.AddListener ( delegate {	UpdateJoint();	});
		jointDictionary = new Dictionary<int, string>();
		ModuleBuilder.FillSymbolDropdown(ref jointDropdown, ref jointDictionary, typeof(JointModule));

		TwistAxis.ValueDelegate(SetTwistAxis);
	}

    void Start()
    {
        JointLimitMin.Slider.onValueChanged.AddListener(delegate { SetJointLimitMin(); });
        JointLimitMin.Slider.minValue = LIMIT_MIN;
        JointLimitMin.Slider.maxValue = 0f;
        JointLimitMax.Slider.onValueChanged.AddListener(delegate { SetJointLimitMax(); });
        JointLimitMax.Slider.minValue = 0f;
        JointLimitMax.Slider.maxValue = LIMIT_MAX;

        JointSpring.Slider.onValueChanged.AddListener(delegate { SetSpring(); });
        JointSpring.Slider.maxValue = SPRING_MAX;
        JointDamper.Slider.onValueChanged.AddListener(delegate { SetDamper(); });
        JointDamper.Slider.maxValue = SPRING_MAX;

        CharacterJointLimitMin.Slider.onValueChanged.AddListener(delegate { SetTwistLimitMin(); });
        CharacterJointLimitMin.Slider.minValue = LIMIT_MIN;
        CharacterJointLimitMin.Slider.maxValue = 0f;
        CharacterJointLimitMax.Slider.onValueChanged.AddListener(delegate { SetTwistLimitMax(); });
        CharacterJointLimitMax.Slider.minValue = 0f;
        CharacterJointLimitMax.Slider.maxValue = LIMIT_MAX;

        CharacterJointSpring.Slider.onValueChanged.AddListener(delegate { SetTwistSpring(); });
        CharacterJointSpring.Slider.maxValue = SPRING_MAX;
        CharacterJointDamper.Slider.onValueChanged.AddListener(delegate { SetTwistDamper(); });
        CharacterJointDamper.Slider.maxValue = SPRING_MAX;

        Gravity.onValueChanged.AddListener(delegate { SetGravity(); });
        Mass.Slider.onValueChanged.AddListener(delegate { SetMass(); });
        Drag.Slider.onValueChanged.AddListener(delegate { SetDrag(); });
        AngularDrag.Slider.onValueChanged.AddListener(delegate { SetAngularDrag(); });
    }

	// Use this for initialization
	public void Init (JointModule jm) 
	{
		currentModule = jm;
		SetJoint();
		UpdateJoint();

		SetJointLimitMin();
		SetJointLimitMax();
		SetDamper();
		SetSpring();

		SetTwistAxis();
		SetTwistLimitMin();
		SetTwistLimitMax();
		SetTwistDamper();
		SetTwistSpring();

		SetMass();
		SetGravity();
		SetDrag();
		SetAngularDrag();
	}

    public void SetUI(JointModule jm)
    {
        currentModule = jm;
        int jointType = 0;
        if (jm.ObjectPath == JOINT_PATH + JointModule.HINGE_JOINT)
        { }
        else if (jm.ObjectPath == JOINT_PATH + JointModule.SPRING_JOINT)
        {
            jointType = 1;
        }
        else
        {
            jointType = 2;
        }
        jointDropdown.value = jointType;

        Gravity.isOn = jm.gravity;
        Mass.Slider.value = jm.mass;
        Drag.Slider.value = jm.drag;
        AngularDrag.Slider.value = jm.angularDrag;

        JointLimitMin.Slider.value = jm.jointLimitMin;
        JointLimitMax.Slider.value = jm.jointLimitMax;
        JointDamper.Slider.value = jm.jointSpringDamper;
        JointSpring.Slider.value = jm.jointSpringSpring;

        CharacterJointLimitMin.Slider.value = jm.twistLimit1;
        CharacterJointLimitMax.Slider.value = jm.twistLimit2;
        CharacterJointDamper.Slider.value = jm.twistSpringDamper;
        CharacterJointSpring.Slider.value = jm.twistSpringSpring;

        TwistAxis.Vector = jm.twistAxis;
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

	public void SetJointLimitMin()
	{ 	currentModule.jointLimitMin = JointLimitMin.Slider.value;	}
	public void SetJointLimitMax()
	{	currentModule.jointLimitMax = JointLimitMax.Slider.value;	}
	public void SetDamper()
	{	currentModule.jointSpringDamper = JointDamper.Slider.value;	}
	public void SetSpring()
	{	currentModule.jointSpringSpring = JointSpring.Slider.value; 	}

	public void SetTwistAxis()
	{	currentModule.twistAxis = TwistAxis.Vector;	}

	public void SetTwistLimitMin()
	{ 	currentModule.twistLimit1 = CharacterJointLimitMin.Slider.value;	}
	public void SetTwistLimitMax()
	{	currentModule.twistLimit2 = CharacterJointLimitMax.Slider.value;	}
	public void SetTwistDamper()
	{	currentModule.twistSpringDamper = CharacterJointDamper.Slider.value;	}
	public void SetTwistSpring()
	{	currentModule.twistSpringSpring = CharacterJointSpring.Slider.value; 	}

	public void SetGravity()
	{	currentModule.gravity = Gravity.isOn; }
	public void SetMass()
	{	currentModule.mass = Mass.Slider.value;	}
	public void SetDrag()
	{	currentModule.drag = Drag.Slider.value;	}
	public void SetAngularDrag()
	{	currentModule.angularDrag = AngularDrag.Slider.value;	}
}
