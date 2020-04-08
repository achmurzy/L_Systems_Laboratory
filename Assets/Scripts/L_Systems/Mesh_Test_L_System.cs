using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Test_L_System : MonoBehaviour
{
    Parametric_L_System mesh_system;
    Parametric_Turtle turtle;

    bool collided = false;

    private void Awake()
    {
        mesh_system = GetComponent<Parametric_L_System>();
        turtle = GetComponentInChildren<Parametric_Turtle>();

        mesh_system.Axiom = new ApexModule('1', 0, 1, GrowthList.LOGISTIC, "Prefabs/ModuleObjects/Apex");
        mesh_system.Axiom.InstantiateModule(turtle);
    }
    // Start is called before the first frame update
    void Start()
    {
        MeshModule mm = new MeshModule('M', 0, 1, GrowthList.LINEAR);
        RotationModule rm = new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), 75f);
        SystemModule bom = new BranchModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL, true);
        SystemModule bcm = new BranchModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL, false);

        List<SystemModule> lm = new List<SystemModule>();
        lm.Add(mm);
        lm.Add(bom);
        lm.Add(rm);
        lm.Add(mm.CopyModule());
        lm.Add(new ApexModule('1', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
        lm.Add(bcm);
        lm.Add(new ApexModule('2', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
        mesh_system.Productions.Add('1', lm);

        List<SystemModule> lm2 = new List<SystemModule>();
        lm2.Add(mm.CopyModule());
        lm2.Add(bom.CopyModule());
        lm2.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), -75f));
        lm2.Add(mm.CopyModule());
        lm2.Add(new ApexModule('2', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
        lm2.Add(bcm.CopyModule());
        lm2.Add(new ApexModule('1', 0, 1, GrowthList.LINEAR, "Prefabs/ModuleObjects/Apex"));
        mesh_system.Productions.Add('2', lm2);
    }

    // Update is called once per frame
    void Update()
    {
        if(collided)
        {
            if (mesh_system.Age < mesh_system.Maturity)
            {
                this.GetComponentInChildren<Parametric_Turtle>().TurtleAnalysis(Time.deltaTime);
                mesh_system.Age += (Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
            collided = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
            collided = false;
    }
}
