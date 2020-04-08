using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_L_System : MonoBehaviour
{
    Parametric_L_System system;
    Parametric_Turtle turtle;

    private void Awake()
    {
        system = GetComponent<Parametric_L_System>();
        turtle = GetComponentInChildren<Parametric_Turtle>();

        //Hack to immediately derive the axiom and create a promordium containing a platform
        system.Axiom = new ApexModule('1', 1, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/Apex");
        system.Axiom.InstantiateModule(turtle);
    }

    // Start is called before the first frame update
    void Start()
    {
        LineModule stem_module = new LineModule('F', 0, 1, GrowthList.LINEAR);
        stem_module.LineWidth = 0.5f;
        stem_module.LineLength = 5.0f;
        //MeshModule stem_module = new MeshModule('M', 0, 1, GrowthList.LINEAR);
        //stem_module.x = 0.1f;
        //stem_module.y = 1f;
        //stem_module.z = 0.1f;

        RotationModule rotation_module = new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, new Vector3(0, 0, 1), 0f, true);
        SystemModule branch_open_module = new BranchModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL, true);
        SystemModule branch_close_module = new BranchModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL, false);

        ObjectModule leaf_module = new ObjectModule('O', 0, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/ManilkaraLeaf");
        leaf_module.scale = Vector3.one * 2.5f;
        
        List<SystemModule> main_axis = new List<SystemModule>();
        //main_axis.Add(branch_open_module);
        main_axis.Add(rotation_module);
        main_axis.Add(stem_module);
        main_axis.Add(new ApexModule('2', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/PlatformApex"));
        //main_axis.Add(branch_close_module);
        main_axis.Add(new ApexModule('1', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
        system.Productions.Add('1', main_axis);

        List<SystemModule> platform_axis = new List<SystemModule>();
        platform_axis.Add(branch_open_module);
        platform_axis.Add(rotation_module);
        platform_axis.Add(stem_module);
        platform_axis.Add(new PhysicsMoverModule('3', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/LeafWhorlMover"));
        platform_axis.Add(branch_close_module);
        system.Productions.Add('2', platform_axis);

        List<SystemModule> leaf_whorl = new List<SystemModule>();
        leaf_whorl.Add(branch_open_module);
        for (int i = 0; i < 4; i++)
        {
            leaf_whorl.Add(branch_open_module.CopyModule());
            leaf_whorl.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, Vector3.right, -45f, false));
            leaf_whorl.Add(leaf_module.CopyModule());
            leaf_whorl.Add(branch_close_module.CopyModule());
            leaf_whorl.Add(new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, Vector3.up, 90f, false));
        }
        leaf_whorl.Add(branch_close_module);
        system.Productions.Add('3', leaf_whorl);

        ApexModule axiom_apex = system.Axiom as ApexModule;
        turtle.TurtleAnalysis(0f);
        axiom_apex.Apex.ActivateApex();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
