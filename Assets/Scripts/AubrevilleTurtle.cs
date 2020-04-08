using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class AubrevilleTurtle : Parametric_Turtle
{
    public AubrevillesModel model;
    public int LateralTrigger = 0;

    void Awake()
    {
        Debug.Log("Turtle awake");
        branchStack = new Stack<TransformHolder>();
        apexStack = new Stack<ApexModule>();
        objList = new List<GameObject>();
        additionsList = new Dictionary<int, List<SystemModule>>();

        p_l_sys = this.GetComponentInParent<Parametric_L_System>();
        model = this.transform.parent.GetComponent<AubrevillesModel>();
    }

    // Use this for initialization
    void Start()
    {
        //We have to start turtle analysis at the same place each time we stir
        initialPosition = new Vector3(transform.localPosition.x,
                                       transform.localPosition.y,
                                       transform.localPosition.z);
        VectorLine.SetCamera3D(FindObjectOfType<Camera>());
    }

    void Update()
    {

    }

    public new void TurtleAnalysis(float age)
    {
        List<SystemModule> word = p_l_sys.returnList;

        p_l_sys.BaseNode.transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(initialPosition.x,
                                              initialPosition.y,
                                              initialPosition.z);

        saveOrientation = new Quaternion(transform.rotation.x,
                                         transform.rotation.y,
                                         transform.rotation.z,
                                         transform.rotation.w);

        for (int i = 0; i < word.Count; i++)
        {
            SystemModule mod = word[i];
            mod.Age += age;
            mod.Age = Mathf.Clamp(mod.Age, 0, mod.TerminalAge);
            mod.UpdateModule(this);

            if (p_l_sys.Productions.ContainsKey(mod.Symbol))
            {
                bool apex_ready = true;
                if (mod is ApexModule)
                {
                    ApexModule am = mod as ApexModule;
                    if (am.mature)
                    {
                        apex_ready = false;
                    }
                    else if((am.MainAxis && am.Depth > model.CurrentLeaderAxisDepth))
                    {
                        apex_ready = false;
                    }
                    else if(!am.MainAxis && am.Axis >= model.CurrentLeaderAxisDepth)
                    {
                        apex_ready = false;
                    }
                    else if(am.Depth - am.Axis > model.MaxLateralDepth)
                    {
                        apex_ready = false;
                    }
                }
                //May need to move this to inside modules so that they can develop independently?
                if (mod.Age >= mod.TerminalAge && mod.TerminalAge >= 0 && apex_ready)
                {

                    List<SystemModule> newList = new List<SystemModule>();
                    for (int j = 0; j < p_l_sys.Productions[mod.Symbol].Count; j++)
                    {
                        SystemModule m = p_l_sys.Productions[mod.Symbol][j].CopyModule();
                        m.InstantiateModule(this);
                        newList.Add(m);
                    }

                    if (mod is ApexModule)
                    {
                        ApexModule am = mod as ApexModule;
                        am.mature = true;
                        am.Apex.ProximalModules = new List<SystemModule>(newList);
                    }
                    else if (mod is PhysicsMoverModule)
                    {
                        PhysicsMoverModule pmm = mod as PhysicsMoverModule;
                        pmm.ProximalModules = newList;
                        pmm.PhysicsModule.GetComponent<ModuleBehaviour>().ProximalModules = newList;
                    }
                    additionsList.Add(i, newList);
                }
            }
        }
        apexStack.Clear();

        if (additionsList.Count != 0)
        {
            p_l_sys.newString = true;
            int i = -1;

            foreach (KeyValuePair<int, List<SystemModule>> kvp in additionsList)
            {
                if (i == -1) //First iteration
                {
                    i = kvp.Value.Count;
                    //p_l_sys.returnList.RemoveAt(kvp.Key);
                    p_l_sys.returnList.InsertRange(kvp.Key + 1, kvp.Value);
                }
                else //every other
                {
                    //p_l_sys.returnList.RemoveAt(i + kvp.Key);
                    p_l_sys.returnList.InsertRange(i + kvp.Key + 1, kvp.Value);
                    i += kvp.Value.Count;
                }
            }
            additionsList.Clear();
        }
        transform.rotation = new Quaternion(saveOrientation.x,
                                            saveOrientation.y,
                                            saveOrientation.z,
                                            saveOrientation.w);
    }
}
