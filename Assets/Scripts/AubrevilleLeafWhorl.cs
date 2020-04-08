using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class AubrevilleLeafWhorl : ModuleBehaviour
{
    PhysicsMover PhysicsMover;
    ApexModule proximalApex;
    bool player_triggered, finish = false;
    public AubrevilleTurtle Turtle;
    private AudioSource rustle_sound;

    private void Awake()
    {
        rustle_sound = GetComponent<AudioSource>();
        ProximalModules = new List<SystemModule>();
    }
    // Start is called before the first frame update
    void Start()
    {
        PhysicsMover = GetComponent<PhysicsMover>();
        Turtle = this.GetComponentInParent<Parametric_L_System>().Turtle as AubrevilleTurtle;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_triggered)
        {
            foreach (SystemModule m in ProximalModules)
            {
                if (m.Age < m.TerminalAge)
                {
                    m.Age += Time.deltaTime;
                }
            }

            (Turtle as AubrevilleTurtle).TurtleAnalysis(0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null && !finish)
        {
            player_triggered = true;
            finish = true;
            if (!rustle_sound.isPlaying)
                rustle_sound.Play();

            proximalApex = (Module as PhysicsMoverModule).ProximalApex;
            proximalApex.Apex.GetComponent<MeshRenderer>().material.color = Color.blue;
            proximalApex.Parent.Parent.Apex.GetComponent<MeshRenderer>().material.color = Color.cyan;
            ApexModule siblingApex = null;
            foreach (ApexModule am in proximalApex.Parent.Parent.Children)
            {
                siblingApex = am;
                if (siblingApex.Children.Contains(proximalApex))
                    continue;
                else
                    break;
            }
            if (siblingApex != null)
                (siblingApex.Apex as AubrevilleApex).ActivateApex();
            else
                Debug.Log("No sibling apex found");

            if (proximalApex.Depth >= Turtle.model.MaxLateralDepth)
            {
                Turtle.model.ApicesTriggered[0][Turtle.LateralTrigger] = 1;
                Turtle.LateralTrigger++;
                if (Turtle.LateralTrigger >= Turtle.model.BranchWhorls)
                {
                    Turtle.LateralTrigger = 0;
                    Turtle.model.CurrentLeaderAxisDepth++;
                    ApexModule leader_axis = Turtle.p_l_sys.GetLeaderApex();
                    (leader_axis.Apex as AubrevilleApex).ActivateApex();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
        {
            player_triggered = false;
            if (!rustle_sound.isPlaying)
                rustle_sound.Play();
        }
    }
}
