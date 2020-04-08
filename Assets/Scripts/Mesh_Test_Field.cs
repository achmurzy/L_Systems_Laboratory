using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mesh_Test_Field : MonoBehaviour
{
    public Object Test_Mesh;
    // Start is called before the first frame update
    GameObject[] test_meshes;

    int X_ = 10;
    int Y_ = 10;

    private void Awake()
    {
        test_meshes = new GameObject[X_ * Y_];
    }
    void Start()
    {
        for(int i = 0; i < X_*Y_; i++)
        {
            test_meshes[i] = GameObject.Instantiate(Test_Mesh as GameObject);
            test_meshes[i].transform.parent = this.transform;
            var mod_x = i % X_;
            test_meshes[i].transform.localPosition = new Vector3(mod_x, 0, (i - mod_x)/Y_);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
