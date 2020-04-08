using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassartsModel : MonoBehaviour
{
    Parametric_L_System system;
    Parametric_Turtle turtle;

    public int BranchWhorls = 4;

    private void Awake()
    {
        system = GetComponent<Parametric_L_System>();
        turtle = GetComponentInChildren<Parametric_Turtle>();

        system.Axiom = new ApexModule('1', 0, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/Apex");
        system.Axiom.InstantiateModule(turtle);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Isn't this what we wanted to avoid via the L_System Laboratory? 
        //Soon we will find ourselves re-compiling and twisting these parameters endlessly, rather than an interactive editor...
        ObjectModule leaf_module = new ObjectModule('O', 0, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/ManilkaraLeaf");
        leaf_module.scale = Vector3.one * 5f;

        MeshModule stem_module = new MeshModule('F', 0, 1, GrowthList.LINEAR);

        RotationModule rotation_module = new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), 75f);
        SystemModule branch_open_module = new BranchModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL, true);
        SystemModule branch_close_module = new BranchModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL, false);

        List<SystemModule> main_axis = new List<SystemModule>();
        main_axis.Add(stem_module);
        for(int i = 0; i < BranchWhorls; i++)
        {
            main_axis.Add(branch_open_module);
            main_axis.Add(new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, new Vector3(0, 1, 0), i * 360f / BranchWhorls));
            main_axis.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), 90f));
            main_axis.Add(new ApexModule('2', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
            main_axis.Add(branch_close_module);
        }
        main_axis.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, Vector3.up, 45f));
        main_axis.Add(new ApexModule('1', 0, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/Apex"));
        system.Productions.Add('1', main_axis);

        List<SystemModule> whorl_axis = new List<SystemModule>();
        whorl_axis.Add(stem_module.CopyModule());
        whorl_axis.Add(branch_open_module.CopyModule());
        whorl_axis.Add(new ApexModule('3', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
        whorl_axis.Add(branch_close_module.CopyModule());
        whorl_axis.Add(new ApexModule('2', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
        system.Productions.Add('2', whorl_axis);

        List<SystemModule> flowering_axis = new List<SystemModule>();
        flowering_axis.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), -90f));
        flowering_axis.Add(stem_module.CopyModule());
        flowering_axis.Add(new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, Vector3.up, 90f));
        system.Productions.Add('3', flowering_axis);

        List<SystemModule> leaf_whorl = new List<SystemModule>();
        for(int i = 0;i<4;i++)
        {
            leaf_whorl.Add(branch_open_module.CopyModule());
            leaf_whorl.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, Vector3.right, -45f));
            leaf_whorl.Add(leaf_module.CopyModule());
            leaf_whorl.Add(branch_close_module.CopyModule());
            leaf_whorl.Add(new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, Vector3.up, 90f));
        }
        leaf_whorl.Add(stem_module.CopyModule());
        system.Productions.Add('M', leaf_whorl);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
