using UnityEngine;
using System.Collections;

//Children of the ObjectModule should still share a symbol -- successive inheritances are to achieve polymorphic
//instantiations that simplify the operation of the turtle
[System.Serializable]
public class ObjectModule : SystemModule 
{
	public string ObjectPath;
	public GameObject ModuleObject { get; set; }
	public Vector3 rotation, scale;
	public bool trigger, jointed;

	public ObjectModule(){}
	public ObjectModule(char sym, float a, float term, int growth, string path) : base(sym, a, term, growth)
	{
		ObjectPath = path;

		scale = Vector3.one;
		rotation = Vector3.zero;
		trigger = true;
		jointed = false;
	}

	public override SystemModule CopyModule()
	{
		ObjectModule om = new ObjectModule(Symbol, Age, TerminalAge, Growth, ObjectPath);
		om.rotation = rotation;
		om.scale = scale;
		om.trigger = trigger;
		om.jointed = jointed;
        return om;
	}

	public virtual GameObject ObjectInstantiate()
	{
		ModuleObject = GameObject.Instantiate(Resources.Load(ObjectPath)) as GameObject;
		return ModuleObject;
	}

	public virtual void ObjectInitialize (GameObject parent)
	{
        ModuleObject.transform.rotation = Quaternion.Euler(rotation) * ModuleObject.transform.rotation;
		if(jointed)
		{
            ModuleObject.transform.parent = parent.transform;
            ModuleObject.transform.localPosition = Vector3.zero;
			Joint thisJoint = parent.GetComponent<Joint>();
			thisJoint.axis = ModuleObject.transform.right.normalized;
			thisJoint.enableCollision = !trigger;
		}
		ModuleObject.transform.localScale = Vector3.one * GrowthFunction(scale.magnitude);
		ModuleObject.GetComponentInChildren<Collider>().isTrigger = trigger;
	}
}
