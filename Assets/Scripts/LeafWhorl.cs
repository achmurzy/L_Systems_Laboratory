using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class LeafWhorl : ModuleBehaviour
{
    PhysicsMover PhysicsMover;
    bool player_triggered = false;
    public Parametric_Turtle Turtle;

    // Start is called before the first frame update
    void Start()
    {
        PhysicsMover = GetComponent<PhysicsMover>();
        Turtle = this.GetComponentInParent<Parametric_L_System>().Turtle;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_triggered)
        {
            Debug.Log("Leaf whorl triggered");
            
            foreach (SystemModule m in ProximalModules)
            {
                if (m.Age < m.TerminalAge)
                {
                    m.Age += Time.deltaTime;
                }
            }
            Turtle.TurtleAnalysis(0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
            player_triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
            player_triggered = false;
    }

    private void OnTriggerStay(Collider other)
    {}
   
}
