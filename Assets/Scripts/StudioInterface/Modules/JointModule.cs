using UnityEngine;
using System.Collections;

[System.Serializable]
public class JointModule : ObjectModule 
{
	public const string HINGE_JOINT = "HingeJoint", SPRING_JOINT = "SpringJoint", CHARACTER_JOINT = "CharacterJoint";
	public Joint NodeJoint { get; private set; }
	public Rigidbody NodeBody { get; private set; }

	//Rigidbody properties
	public float mass, drag, angularDrag;
	public bool gravity;

	//Joint Properties
	public Vector3 rotateAxis, twistAxis;

	public float jointLimitMin;
	public float jointLimitMax;

	public float jointSpringSpring;
	public float jointSpringDamper;

	public float twistLimit1, twistLimit2;
	public float twistSpringSpring;
	public float twistSpringDamper;

	public JointModule(){}
	public JointModule(char sym, float a, float term, int growth, string path) : base(sym, a, term, growth, path)
	{}

	public override SystemModule CopyModule()
	{
		JointModule om = new JointModule(Symbol, Age, TerminalAge, Growth, ObjectPath);
		om.mass = mass;
		om.angularDrag = angularDrag;
		om.drag = drag;
		om.gravity = gravity;

		om.jointLimitMin = jointLimitMin;
		om.jointLimitMax = jointLimitMax;
		om.jointSpringSpring = jointSpringSpring;
		om.jointSpringDamper = jointSpringDamper;

		om.twistLimit1 = twistLimit1;
		om.twistLimit2 = twistLimit2;

		om.twistSpringSpring = twistSpringSpring;
		om.twistSpringDamper = twistSpringDamper;
        return om;
	}

	public override GameObject ObjectInstantiate()
	{
		ModuleObject = GameObject.Instantiate(Resources.Load(ObjectPath)) as GameObject;
		NodeJoint = ModuleObject.GetComponent<Joint>();
		NodeBody = ModuleObject.GetComponent<Rigidbody>();
		return ModuleObject;
	}

	public override void ObjectInitialize(GameObject parent)
	{
		NodeJoint = ModuleObject.GetComponent<Joint>();
		NodeJoint.connectedBody = parent.GetComponent<Rigidbody>();
		NodeJoint.connectedAnchor = ModuleObject.transform.localPosition - parent.transform.localPosition;

		NodeBody.mass = GrowthFunction(mass);
		NodeBody.drag = GrowthFunction(drag);
		NodeBody.angularDrag = GrowthFunction(angularDrag);
		NodeBody.useGravity = gravity;
		/*Debug.Log("ModuleWorld: " + ModuleObject.transform.position);
		Debug.Log("ModuleLocal: " + ModuleObject.transform.localPosition);
		Debug.Log("ParentWorld: " + parent.transform.position);
		Debug.Log("ParentLocal: " + parent.transform.localPosition);
		Debug.Log("Joint Distance: " + NodeJoint.connectedAnchor); 
		Debug.DrawRay(parent.transform.position, NodeJoint.connectedAnchor, Color.black, 100f);*/

		if(NodeJoint is SpringJoint)
		{
			SpringJoint sj = NodeJoint as SpringJoint;
			sj.axis = rotateAxis;
			sj.damper = GrowthFunction(jointSpringDamper);
			sj.spring = GrowthFunction(jointSpringSpring);	
			sj.minDistance = GrowthFunction(jointLimitMin);
			sj.maxDistance = GrowthFunction(jointLimitMax);
		}
		else 
		{
			if(NodeJoint is CharacterJoint)
			{
				CharacterJoint cj = NodeJoint as CharacterJoint;
				SoftJointLimit sj1 = new SoftJointLimit();
				SoftJointLimit sj2 = new SoftJointLimit();
			
				sj1.limit = GrowthFunction(-jointLimitMin);
				sj2.limit = GrowthFunction(jointLimitMax);
				cj.swing1Limit = sj1;
				cj.swing2Limit = sj2;

				SoftJointLimitSpring sjlp = new SoftJointLimitSpring();
				sjlp.damper = GrowthFunction(jointSpringDamper);
				sjlp.spring = GrowthFunction(jointSpringSpring);
				cj.swingLimitSpring = sjlp;
				cj.swingAxis = rotateAxis;

				SoftJointLimit stl1 = new SoftJointLimit();
				SoftJointLimit stl2 = new SoftJointLimit();

				stl1.limit = GrowthFunction(twistLimit1);
				stl2.limit = GrowthFunction(twistLimit2);
				cj.lowTwistLimit = stl1;
				cj.highTwistLimit = stl2;

				SoftJointLimitSpring sjls = new SoftJointLimitSpring();
				sjls.spring = GrowthFunction(twistSpringSpring);
				sjls.damper = GrowthFunction(twistSpringDamper);
				cj.twistLimitSpring = sjls;
				cj.axis = twistAxis;
			}
			else if (NodeJoint is HingeJoint)
			{
				HingeJoint hj = NodeJoint as HingeJoint;
				JointSpring sj = new JointSpring();
				sj.spring = GrowthFunction(jointSpringSpring);
				sj.damper = GrowthFunction(jointSpringDamper);
				hj.spring = sj;

				JointLimits jl = new JointLimits();
				jl.min = GrowthFunction(jointLimitMin);
				jl.max = GrowthFunction(jointLimitMax);

				hj.limits = jl;
				hj.axis = rotateAxis;
			}
		}
	}
}
