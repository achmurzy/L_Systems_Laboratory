using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AubrevillesModel : MonoBehaviour
{
    Parametric_L_System system;
    AubrevilleTurtle turtle;
    PhytoControllerMachine phyto;

    public int BranchWhorls = 4;
    public int MaxLateralDepth = 5;
    public int MaxVerticalDepth = 5;

    public int CurrentLeaderAxisDepth = 1;

    public int[][] ApicesTriggered;

    //On birth, age to a sapling by completing 2 derivations (assume module Terminal age is 1);
    public float InitialAge = 5f;
    private bool sapling = false;

    private void Awake()
    {
        system = GetComponent<Parametric_L_System>();
        turtle = GetComponentInChildren<AubrevilleTurtle>();

        ApicesTriggered = new int[MaxVerticalDepth][];
        for (int i = 0; i < MaxVerticalDepth; i++)
        {
            ApicesTriggered[i] = new int[BranchWhorls];
        }

        phyto = FindObjectOfType<PhytoControllerMachine>();
        //phyto.DoubleJump.AddListener();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Set the axiom's current age to 1 so that it derives at time 0
        system.Axiom = new ApexModule('1', 1, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/AubrevilleApex", true);
        turtle.apexStack.Push(system.Axiom as ApexModule);
        system.Axiom.InstantiateModule(turtle);
        ApexModule am = (system.Axiom as ApexModule);
        am.Children.RemoveAt(am.Children.Count - 1);

        ObjectModule leaf_module = new ObjectModule('O', 0, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/ManilkaraLeaf");
        leaf_module.scale = Vector3.one * 3f;

        MeshModule stem_module = new MeshModule('F', 0, 1, GrowthList.LINEAR);
        BezierMeshModule branch_module = new BezierMeshModule('F', 0, 1, GrowthList.LINEAR);
        branch_module.apposition = true;
        BezierMeshModule flowering_branch = new BezierMeshModule('F', 0, 1, GrowthList.LINEAR);
        flowering_branch.height = 0.25f;

        RotationModule rotation_module = new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), 75f);
        SystemModule branch_open_module = new BranchModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL, true);
        SystemModule branch_close_module = new BranchModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL, false);
        SystemModule branch_close_module_no_apex = new BranchModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL, false, false);

        List<SystemModule> main_axis = new List<SystemModule>();
        main_axis.Add(stem_module);
        for (int i = 0; i < BranchWhorls; i++)
        {
            main_axis.Add(branch_open_module);
            main_axis.Add(new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, new Vector3(0, 1, 0), i * 360f / BranchWhorls));
            main_axis.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(1, 0, 0), 90f));
            main_axis.Add(new ApexModule('2', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/AubrevilleApex"));
            main_axis.Add(branch_close_module);
        }
        main_axis.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, Vector3.up, 45f));
        main_axis.Add(branch_open_module.CopyModule());
        main_axis.Add(new ApexModule('1', 0, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/AubrevilleApex", true));
        main_axis.Add(branch_close_module.CopyModule());
        system.Productions.Add('1', main_axis);

        List<SystemModule> whorl_axis = new List<SystemModule>();
        whorl_axis.Add(branch_module.CopyModule());
        whorl_axis.Add(branch_open_module.CopyModule());
        whorl_axis.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(1, 0, 0), -90f));
        whorl_axis.Add(new ApexModule('3', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/AubrevilleApex"));
        whorl_axis.Add(branch_close_module.CopyModule());
        whorl_axis.Add(branch_open_module.CopyModule());
        whorl_axis.Add(new ApexModule('2', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/AubrevilleApex"));
        whorl_axis.Add(branch_close_module.CopyModule());
        system.Productions.Add('2', whorl_axis);

        List<SystemModule> flowering_axis = new List<SystemModule>();
        flowering_axis.Add(flowering_branch.CopyModule());
        flowering_axis.Add(new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, Vector3.up, 90f));
        flowering_axis.Add(branch_open_module.CopyModule());
        flowering_axis.Add(new ApexModule('4', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/AubrevilleApex"));
        flowering_axis.Add(branch_close_module.CopyModule());
        system.Productions.Add('3', flowering_axis);

        List<SystemModule> leaf_whorl = new List<SystemModule>();
        leaf_whorl.Add(new PhysicsMoverModule('M', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/AubrevilleLeafWhorl"));
        for (int i = 0; i < 4; i++)
        {
            leaf_whorl.Add(branch_open_module.CopyModule());
            leaf_whorl.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, Vector3.right, -45f));
            leaf_whorl.Add(leaf_module.CopyModule());
            leaf_whorl.Add(branch_close_module_no_apex.CopyModule());
            leaf_whorl.Add(new RotationModule('+', 0, 1, GrowthList.NON_DEVELOPMENTAL, Vector3.up, 90f));
        }
        system.Productions.Add('4', leaf_whorl);
    }

    private void Update()
    {
        if (!sapling)
        {
            system.Age += Time.deltaTime;
            turtle.TurtleAnalysis(Time.deltaTime);
            if (system.Age > InitialAge)
            {
                sapling = true;
                DrawDebugTopology();
            }
        }
    }

    private void DrawDebugTopology()
    {
        ApexModule currentApex = system.Axiom as ApexModule;
        recurse_tree(currentApex);
    }

    private void recurse_tree(ApexModule am)
    {
        if(am.Children.Count < 1)
        {
            return;
        }
        else
        {
            foreach (ApexModule amc in am.Children)
            {
                Debug.DrawLine(am.Apex.transform.position, amc.Apex.transform.position, Color.red, 3000.0f, false);
                recurse_tree(amc);
            }
        }
    }
}
