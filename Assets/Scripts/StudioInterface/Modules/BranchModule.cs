using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchModule : SystemModule
{
    public bool Open;
    public bool Apex;

    public BranchModule() { }
    public BranchModule(char sym, float a, float term, int growth, bool open, bool apex = true) : base(sym, a, term, growth)
    {
        Open = open;
        Apex = apex;
    }

    public override SystemModule CopyModule()
    {
        BranchModule mm = BranchModule.CreateInstance<BranchModule>();
        mm.Symbol = Symbol;
        mm.Age = Age;
        mm.TerminalAge = TerminalAge;
        mm.Growth = Growth;
        mm.Open = Open;
        mm.Apex = Apex;
        return (mm);
    }

    public override void InstantiateModule(Parametric_Turtle turtle)
    {
        //this.UpdateModule(turtle);
    }

    public override void UpdateModule(Parametric_Turtle turtle)
    {
        if (Open)
        {
            Parametric_Turtle.TransformHolder pushTrans = new Parametric_Turtle.TransformHolder();
            pushTrans.Store(turtle.transform);
            turtle.branchStack.Push(pushTrans);
        }
        else
        {
            Parametric_Turtle.TransformHolder popTrans = (Parametric_Turtle.TransformHolder)turtle.branchStack.Pop();
            turtle.transform.localPosition = new Vector3(popTrans.position.x, popTrans.position.y, popTrans.position.z);
            turtle.transform.localRotation = new Quaternion(popTrans.rotation.x, popTrans.rotation.y,
                                            popTrans.rotation.z, popTrans.rotation.w);
            if(Apex)
            {
                turtle.DepthCounter--;
                turtle.apexStack.Pop();
            }
        }

    }
}
