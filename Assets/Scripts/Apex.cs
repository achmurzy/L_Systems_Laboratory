using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apex : ModuleBehaviour
{
    public Parametric_Turtle Turtle;
    protected bool player_triggered = false;

    public List<SystemModule> ProximalModules;
    public ApexModule ApexModule, PlatformModule, ChildModule;

    protected void Awake()
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
            foreach (SystemModule m in ProximalModules)
            {
                if (m.Age < m.TerminalAge)
                {
                    m.Age += Time.deltaTime;
                }
                else
                    player_triggered = false;
            }
            Turtle.TurtleAnalysis(0f);
        }
    }

    public void ActivateApex()
    {
        player_triggered = true;
        ChildModule = ProximalModules[3] as ApexModule;
        PlatformModule = ProximalModules[2] as ApexModule;
        PlatformApex platform = PlatformModule.Apex as PlatformApex;
        platform.MainAxis = ChildModule;
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
