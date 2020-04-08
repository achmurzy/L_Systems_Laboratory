using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class PhysicsMoverModule : SystemModule, IMoverController
{
    public string Path;
    public GameObject PhysicsModule;
    public Vector3 turtlePosition = Vector3.zero;
    public Quaternion turtleOrientation = Quaternion.identity;

    public ApexModule ProximalApex;
    public List<SystemModule> ProximalModules;

    public PhysicsMoverModule() { }
    public PhysicsMoverModule(char sym, float a, float term, int growth, string path) : base(sym, a, term, growth)
    {
        Path = path;
    }

    public override SystemModule CopyModule()
    {
        PhysicsMoverModule mm = PhysicsMoverModule.CreateInstance<PhysicsMoverModule>();
        mm.Symbol = Symbol;
        mm.Age = Age;
        mm.TerminalAge = TerminalAge;
        mm.Growth = Growth;
        mm.Path = Path;

        return mm;
    }

    public override void InstantiateModule(Parametric_Turtle turtle)
    {
        PhysicsModule = GameObject.Instantiate(Resources.Load(Path) as GameObject, turtle.transform.parent);
        //PhysicsModule.transform.localPosition = turtle.transform.localPosition;
        //PhysicsModule.transform.localRotation = Quaternion.identity;
        PhysicsModule.GetComponent<ModuleBehaviour>().Module = this;
        PhysicsModule.GetComponent<PhysicsMover>().MoverController = this;

        ProximalApex = turtle.apexStack.Peek();
    }

    public override void UpdateModule(Parametric_Turtle turtle)
    {
        turtlePosition = turtle.transform.position;
        turtleOrientation = Quaternion.identity;


    }

    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        goalPosition = turtlePosition;
        goalRotation = turtleOrientation;
    }
}
