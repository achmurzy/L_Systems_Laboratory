using UnityEngine;
using System.Collections;

public class Phototropism : MonoBehaviour 
{
	public const string OBJECT_PATH = "Prefabs/ModuleObjects/";
	// Use this for initialization
	void Start () 
	{
		AddRayFlowers();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void AddRayFlowers(int resolution = 25)
	{
		GameObject Rayflower;
		Rayflower = Resources.Load(OBJECT_PATH + ObjectList.FERN_LEAFLET) as GameObject;

		Material Raycolor;
		//Raycolor = Resources.Load("") as Material;

		GameObject Disc = this.transform.GetChild(0).GetChild(0).gameObject;

		for(int i = 0; i < resolution - 1; i++)	//This is always a pain
		{
			GameObject ray = GameObject.Instantiate(Rayflower) as GameObject;
			float index = ((float)i)/((float)resolution);
			ray.transform.parent = Disc.transform;
			ray.transform.localPosition = new Vector3(Mathf.Cos(360f*index), Mathf.Sin(360f*index), 1f);
			ray.transform.up = (ray.transform.position - Disc.transform.position).normalized;
			ray.transform.localScale = new Vector3(10f, 15f, 10f) * 2f;
		}
	}
}
