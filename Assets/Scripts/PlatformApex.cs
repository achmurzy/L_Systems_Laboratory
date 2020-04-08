using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformApex : Apex
{
    PhytoControllerMachine phyto;
    public ApexModule MainAxis;

    private float peak_time;


    private new void Awake()
    {
        base.Awake();
        phyto = FindObjectOfType<PhytoControllerMachine>();
        phyto.DoubleJump.AddListener(DerivePlatform);
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (player_triggered)
        {
            float phyto_velocity = 1.01f - (phyto.Motor.Velocity.y / peak_time);
            float jump_age = Mathf.Lerp(0, 1f, phyto_velocity);
            foreach (SystemModule m in ProximalModules)
            {
                if(m is RotationModule)
                {
                    RotationModule rm = m as RotationModule;
                    rm.RotationAxis = Vector3.Cross(phyto.transform.position - this.transform.position, Vector3.up).normalized;
                    rm.RotationScalar = -Vector3.Angle(phyto.transform.position - this.transform.position, Vector3.up);
                }
                else if (m is LineModule)
                {
                    LineModule lm = m as LineModule;
                    lm.LineLength = Vector3.Distance(phyto.transform.position, this.transform.position);
                }
                
                if (m.Age < m.TerminalAge)
                {
                    m.Age = jump_age;
                }

            }
            MainAxis.Age = jump_age;

            Turtle.TurtleAnalysis(0f);
            if (phyto_velocity >= 1)
            {
                player_triggered = false;
                phyto.DoubleJump.RemoveListener(DerivePlatform);
            }
        }
    }

    void DerivePlatform(float peak_velocity)
    {
        if(ApexModule.Age >= ApexModule.TerminalAge)
        {
            player_triggered = true;
            this.peak_time = peak_velocity;
            
            MainAxis.Apex.ActivateApex();
        }
    }
}
