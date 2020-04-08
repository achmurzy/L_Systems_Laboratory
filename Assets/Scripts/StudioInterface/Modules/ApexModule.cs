using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApexModule : SystemModule
{
    public Apex Apex;
    public string ApexPath;

    public int Depth, Axis;
    public ApexModule Parent;
    public List<ApexModule> Children;

    public bool MainAxis = false;
    public bool mature = false;

    public ApexModule() { }
    public ApexModule(char sym, float a, float term, int growth, string path, bool main = false) : base(sym, a, term, growth)
    {
        ApexPath = path;
        MainAxis = main;
    }

    public override SystemModule CopyModule()
    {
        ApexModule am = ApexModule.CreateInstance<ApexModule>();
        am.Symbol = Symbol;
        am.Age = Age;
        am.TerminalAge = TerminalAge;
        am.Growth = Growth;
        am.ApexPath = ApexPath;
        am.MainAxis = MainAxis;
        return am;
    }

    public override void InstantiateModule(Parametric_Turtle turtle)
    {
        this.Parent = turtle.apexStack.Peek();
        this.Children = new List<ApexModule>();
        this.Parent.Children.Add(this);
        this.Depth = turtle.DepthCounter;
        this.Axis = turtle.p_l_sys.GetProximalMainAxisApex(this).Depth;

        GameObject ModuleObject = GameObject.Instantiate(Resources.Load(ApexPath), turtle.transform.parent) as GameObject;
        if (ModuleObject.GetComponent<Apex>() != null)
        {
            Apex = ModuleObject.GetComponent<Apex>();
            Apex.ApexModule = this;
        }
        //this.UpdateModule(turtle);
        //turtle.apexStack.Pop();
    }

    public override void UpdateModule(Parametric_Turtle turtle)
    {
        Apex.transform.localPosition = turtle.transform.localPosition;
        Apex.transform.localPosition += turtle.transform.up.normalized * 0.1f;
        Apex.transform.localRotation = turtle.transform.localRotation;
        Apex.transform.localScale = Vector3.one * Depth * 0.1f;

        turtle.DepthCounter++;
        turtle.apexStack.Push(this);
    }

}
