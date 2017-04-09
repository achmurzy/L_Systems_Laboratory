using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This script dynamically generates and animates L_Systems in the player's trail and around his feet.
public class L_System_Trail : MonoBehaviour {

	private Object[] possiblePlants;

	private List<GameObject> plantList;
	private List<GameObject> deadList;

	private float circleRadius = 2.5f;

	private int maxPlantPopulation = 5;
	private float plantTimer;
	private float newSpawnInterval = 2.0f;

	//Spawned plant parameters
	private float maxHeight = 0.1f;
	private float maxHeightInCircle = 0.25f;
	private float maxAngle = 45.0f;
	private float minAngle = 10.0f;
	private int maxGenerationsOnSpawn = 3;
	private float lifetimeOutsideCircle = 5.0f;

	// Use this for initialization
	void Start () 
	{
		possiblePlants = Resources.LoadAll ("Prefabs/L_Systems/Animated");
		plantList = new List<GameObject> ();
		deadList = new List<GameObject> ();

		plantTimer = newSpawnInterval;
	}
	
	// Update is called once per frame
	void Update () 
	{
		L_System_Animated lsa;

		//Add new plants
		if(plantList.Count < maxPlantPopulation)
		{
			if(plantTimer > newSpawnInterval)
			{
				//Instantiate a new prefab's location using trigonometry
				//	eventually we will randomly select a prefab of different L_Systems
				int numLSystems = possiblePlants.Length;

				Vector2 xz = Random.insideUnitCircle * circleRadius;
				GameObject newPlant = 
						(GameObject)Instantiate(possiblePlants[Mathf.FloorToInt(Random.Range(0, numLSystems))],
							new Vector3(xz.x + transform.position.x, 0, xz.y + transform.position.z), 
					               	Quaternion.identity);

				lsa = newPlant.GetComponent<L_System_Animated>();
				// -set draw parameters
				lsa.maxEdge = Random.Range(0.05f, maxHeightInCircle);
				Debug.Log (lsa.maxEdge);
				lsa.maxAngle = Random.Range(minAngle, maxAngle);
				// -set maxGenerations for propogation
				lsa.generations = Mathf.FloorToInt(Random.Range(2, maxGenerationsOnSpawn));
				// -the actual value for both of these parameters 
				//	should be randomly generated within the acceptable range
				plantList.Add(newPlant);
				//Add it to the list

				plantTimer = 0;
			}	
		}

		//Check up on those already living
		foreach(GameObject ls in plantList)
		{
			lsa = ls.GetComponent<L_System_Animated>();
			//Check if a plant has left the circle and change its maxHeight, start killing it
			if(lsa.inCircle)
			{
				Vector3 playerPos = new Vector3(transform.position.x, 0, transform.position.z);
				//In the future, we do not want jumping to affect this calculation. Keep that in mind.
				if(Vector3.Magnitude(lsa.transform.position - playerPos) > circleRadius)
				{
					lsa.inCircle = false;
					lsa.maxEdge = maxHeight;
				}
			}
			//Check the list for "dead" plants: outside the circle, and a full life-span
			else
			{
				if(lsa.age > lifetimeOutsideCircle)
				{
					//Add to destruction list - remember we cannot destroy objects inside of a foreach loop
					deadList.Add (ls);
				}
				else
					lsa.age += Time.deltaTime;
			}
		}

		//Get rid of plants too old
		foreach(GameObject ls in deadList)
		{
			plantList.Remove(ls);
			Turtle turtle = ls.GetComponentInChildren<Turtle>();
			turtle.destroyLines();
			Destroy(ls);
		}

		deadList.Clear ();

		plantTimer += Time.deltaTime;
	}
}
