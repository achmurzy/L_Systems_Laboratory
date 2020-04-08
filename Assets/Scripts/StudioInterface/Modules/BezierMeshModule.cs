using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMeshModule : SystemModule
{
    public GameObject MeshObject;
    MeshFilter l_mesh;
    MeshRenderer renderer;

    public float length = 0.1f, height = 0.5f, width = 0.1f;
    int n = 0;
    public const int MAX_N = 5;
    public Vector3 Start, End, ControlPoint1;
    public bool apposition;
    public BezierMeshModule() { }
    public BezierMeshModule(char sym, float a, float term, int growth) : base(sym, a, term, growth)
    { }

    public override void InstantiateModule(Parametric_Turtle turtle)
    {
        MeshObject = new GameObject();
        MeshObject.transform.parent = turtle.transform.parent;
        MeshObject.layer = LayerMask.NameToLayer("Plant");
        l_mesh = MeshObject.AddComponent<MeshFilter>();
        MeshObject.AddComponent<BoxCollider>();
        l_mesh.mesh = new Mesh();

        renderer = MeshObject.AddComponent<MeshRenderer>();
        renderer.material = Resources.Load("Materials/SimpleWoodMat") as Material;

    }
    public override SystemModule CopyModule()
    {
        BezierMeshModule mm = BezierMeshModule.CreateInstance<BezierMeshModule>();
        mm.Symbol = Symbol;
        mm.Age = Age;
        mm.TerminalAge = TerminalAge;
        mm.Growth = Growth;
        mm.width = width;
        mm.height = height;
        mm.length = length;
        mm.apposition = apposition;
        return (mm);
    }

    public override void UpdateModule(Parametric_Turtle turtle)
    {
        Vector3 localPos = turtle.transform.localPosition;
        Vector3 heading = turtle.transform.up;
        MeshObject.transform.localPosition = localPos;
        MeshObject.transform.up = heading;
        int currentAge = (int)(MAX_N * Age / TerminalAge);

        Vector3 gravitropic_vector = turtle.transform.forward*length;
        if (apposition)
            gravitropic_vector = turtle.transform.up;
        Start = new Vector3(0, 0, 0);
        End = new Vector3(0, currentAge * height, 0);
        Vector3 age_vector = Vector3.Lerp(Vector3.zero, gravitropic_vector, Age / TerminalAge);
        ControlPoint1 = new Vector3(0, (currentAge * height) / 2, 0) + age_vector;

        MakeQuads(currentAge);

        Destroy(this.MeshObject.GetComponent<BoxCollider>());
        this.MeshObject.AddComponent<BoxCollider>();

        turtle.transform.localPosition += heading * currentAge * height;
    }

    void MakeQuads(int n)
    {
        int vert_count = (n + 1) * 4;
        int tri_count = n * 8 * 3;
        int normal_count = ((n - 1) * 5) + (2 * 8);

        var vertices = new Vector3[vert_count];
        vertices[0] = new Vector3(-width/2, 0, -length/2);
        vertices[1] = new Vector3(-width/2, 0, length/2);
        vertices[2] = new Vector3(width/2, 0, length/2);
        vertices[3] = new Vector3(width/2, 0, -length/2);

        var tris = new int[tri_count];

        /* At this point, the Flat shader can live without normals and UVs
         * var normals = new Vector3[normal_count];
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;

        var uvs = new Vector2[normal_count];*/

        int ind = -1;
        int last_ind = -1;

        for (int i = 1; i < (n + 1); i++)
        {
            ind = 4 * i;
            last_ind = ind - 4;
            float t = (float)i / (n);
            
            Vector3 term1 = (1 - t) * (((1 - t) * Start) + (t * ControlPoint1));
            Vector3 term2 = t * (((1 - t)*ControlPoint1) + (t*End));
            Vector3 pos = term1 + term2;
            Vector3 offset = new Vector3(width, height, length);
            //float iy = i * y;
            vertices[ind] = pos - new Vector3(width / 2, 0, length / 2);
            vertices[ind + 1] = pos + new Vector3(-width / 2, 0, length / 2);
            vertices[ind + 2] = pos + new Vector3(width / 2, 0, length / 2);
            vertices[ind + 3] = pos + new Vector3(width / 2, 0, -length / 2);

            int tri_ind = (i - 1) * 24;
            /*Front Face*/
            // lower left triangle
            tris[tri_ind] = last_ind;
            tris[tri_ind + 1] = ind;
            tris[tri_ind + 2] = last_ind + 3;
            // upper right triangle
            tris[tri_ind + 3] = ind + 3;
            tris[tri_ind + 4] = last_ind + 3;
            tris[tri_ind + 5] = ind;

            /*Left Face*/
            tris[tri_ind + 6] = last_ind + 1;
            tris[tri_ind + 7] = ind + 1;
            tris[tri_ind + 8] = last_ind;

            tris[tri_ind + 9] = ind;
            tris[tri_ind + 10] = last_ind;
            tris[tri_ind + 11] = ind + 1;

            /*Back Face*/
            tris[tri_ind + 12] = last_ind + 2;
            tris[tri_ind + 13] = ind + 2;
            tris[tri_ind + 14] = last_ind + 1;

            tris[tri_ind + 15] = ind + 1;
            tris[tri_ind + 16] = last_ind + 1;
            tris[tri_ind + 17] = ind + 2;

            /*Right Face*/
            tris[tri_ind + 18] = last_ind + 3;
            tris[tri_ind + 19] = ind + 3;
            tris[tri_ind + 20] = last_ind + 2;

            tris[tri_ind + 21] = ind + 2;
            tris[tri_ind + 22] = last_ind + 2;
            tris[tri_ind + 23] = ind + 3;

            /* var uv = new Vector2[4]
            {
             new Vector2(0, 0),
             new Vector2(1, 0),
             new Vector2(0, 1),
             new Vector2(1, 1)
            };*/
        }
        l_mesh.mesh.vertices = vertices;
        l_mesh.mesh.triangles = tris;
        //l_mesh.mesh.normals = normals;
        //l_mesh.mesh.uv = uv;
    }

    /*void CombineMeshes()  Move to top-level method on L-System instead of Module
    {
        MeshFilter[] meshes = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] ci = new CombineInstance[meshes.Length];
        for (int i = 0; i < meshes.Length; i++)
        {
            ci[i].mesh = meshes[i].sharedMesh;
            ci[i].transform = meshes[i].transform.localToWorldMatrix;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(ci, true, true);
    }*/
}
