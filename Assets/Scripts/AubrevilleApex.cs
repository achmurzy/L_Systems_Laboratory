using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AubrevilleApex : Apex
{
    bool vertical = false;
    protected new void Awake()
    {
        ProximalModules = new List<SystemModule>();
    }

    protected void Start()
    {
        Turtle = this.GetComponentInParent<Parametric_L_System>().Turtle;
    }

    // Update is called once per frame
    void Update()
    {
        if(player_triggered)
        {
            this.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            
            if(ApexModule.MainAxis)
            {
                ApexModule.Parent.Apex.GetComponent<MeshRenderer>().material.color = Color.yellow;
                foreach (ApexModule am in this.ApexModule.Parent.Children)
                {
                    foreach (SystemModule m in am.Apex.ProximalModules)
                    {
                        if (m.Age < m.TerminalAge)
                        {
                            m.Age += Time.deltaTime;
                        }
                        else if(am.Symbol == '2')
                        {
                            foreach(ApexModule amc in am.Children)
                            {
                                foreach(SystemModule smc in amc.Apex.ProximalModules)
                                {
                                    if (smc.Age < smc.TerminalAge)
                                    {
                                        smc.Age += Time.deltaTime;
                                    }
                                    else
                                        player_triggered = false;
                                }
                            }
                        }
                    }
                }
                
                (Turtle as AubrevilleTurtle).TurtleAnalysis(0f);
            }
            else
            {
                foreach (ApexModule am in ApexModule.Children) //Responsible for branch apposition and deriving child apices
                {
                    foreach (SystemModule m in am.Apex.ProximalModules)
                    {
                        if (m.Age < m.TerminalAge)
                        {
                            m.Age += Time.deltaTime;
                        }
                        else
                            player_triggered = false;
                    }
                }
            }
            (Turtle as AubrevilleTurtle).TurtleAnalysis(0f);
        }
    }

    public new void ActivateApex()
    {
        player_triggered = true;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
            player_triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
            player_triggered = false;
    }*/
}
