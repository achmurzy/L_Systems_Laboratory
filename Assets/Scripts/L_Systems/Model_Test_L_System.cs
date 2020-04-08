using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model_Test_L_System : MonoBehaviour
{
    Parametric_L_System model_system;
    Parametric_Turtle turtle;
    bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        model_system = GetComponent<Parametric_L_System>();
        turtle = GetComponent<Parametric_Turtle>();

        LineModule ln = new LineModule('F', 0, 1, GrowthList.LINEAR);
        ln.LineWidth = 0.1f;
        ln.LineLength = 0.5f;
        List<SystemModule> lm = new List<SystemModule>();
        lm.Add(ln);
        lm.Add(new SystemModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL));
        lm.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), 45f));
        lm.Add(ln.CopyModule());
        lm.Add(new SystemModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL));
        lm.Add(new SystemModule('2', 0, 1, GrowthList.NON_DEVELOPMENTAL));

        List<SystemModule> lm2 = new List<SystemModule>();
        lm2.Add(ln.CopyModule());
        lm2.Add(new SystemModule('[', 0, 1, GrowthList.NON_DEVELOPMENTAL));
        lm2.Add(new RotationModule('+', 0, 1, GrowthList.LINEAR, new Vector3(0, 0, 1), -45f));
        lm2.Add(ln.CopyModule());
        lm2.Add(new SystemModule(']', 0, 1, GrowthList.NON_DEVELOPMENTAL));
        lm2.Add(new SystemModule('1', 0, 1, GrowthList.NON_DEVELOPMENTAL));

        model_system.Productions.Add('1', lm);
        model_system.Productions.Add('2', lm2);
    }

    // Update is called once per frame
    void Update()
    {
        if (collided)
        {
            if (model_system.Age < model_system.Maturity)
            {
                this.GetComponentInChildren<Parametric_Turtle>().TurtleAnalysis(Time.deltaTime);
                model_system.Age += (Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        if (other.GetComponent<CharacterMachine>() != null)
            collided = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterMachine>() != null)
            collided = false;
    }
}
