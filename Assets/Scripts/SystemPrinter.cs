using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemPrinter : MonoBehaviour
{
    public Parametric_L_System L_System;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Text>().text = L_System.PrintDerivation();
    }
}
