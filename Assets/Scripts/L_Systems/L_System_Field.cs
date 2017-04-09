using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*Quite similar in function to L_System_Trail, generates juvenile L_Systems
 * in a much larger radius around the player. They are interactable (growable)
 * and senesce based on distance in much the same way as the non-interactive
 * specimens in the trail.*/
/*Repurposed as our general generation scheme. Encodes four rectangles which stipulate the three areas
 * found within the game. Calls and maintains different generation schema based on the player's position
 * in these areas*/
//The terrain is assumed to be 500x500

public class L_System_Field : MonoBehaviour {

	//A 200x200 rectangle in the bottom left corner
	private Rect AreaI = new Rect (0, 0, 200, 200);

	private Rect AreaIIi = new Rect (0, 200, 350, 300);
	private Rect AreaIIii = new Rect (200, 0, 300, 350);

	private Rect StaticZone = new Rect (0, 0, 0, 0);
	private float zoneWidth = 100;
	private float zoneHeight = 100;

	private Rect AreaIII;
	public GameObject streets;

	private Object[] possiblePlants;
	private Object[] possibleStatics;
	
	private static List<GameObject> plantList1, plantList2, plantList3;
	private List<GameObject> deadList, staticList;

	private float circleRadius = 50.0f;
	private float killDistance = 150.0f;
	private float generationCleanup = 20.0f;

	private float verticalCastDistance = 1500.0f;
	private float horizontalCastDistance = 5.0f;
	RaycastHit info;
	private int castMask = 251;
	
	private int maxPlantPopulationZone2 = 30;
	private int maxPlantPopZones1and3 = 15;

	private int maxStaticPopulation = 20;

	private int currentZone = 0;
	private int lastZone = 0;

	// Use this for initialization
	void Start () 
	{
		possiblePlants = Resources.LoadAll ("Prefabs/L_Systems/Interactable");
		possibleStatics = Resources.LoadAll ("Prefabs/L_Systems/Static");
		plantList1 = new List<GameObject> ();
		plantList2 = new List<GameObject> ();
		plantList3 = new List<GameObject> ();

		deadList = new List<GameObject> ();
		staticList = new List<GameObject> ();
		AreaIII = new Rect (0, 0, streets.transform.localScale.x, streets.transform.localScale.z);

		StartCoroutine ("generationUpdate");
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	private void AddPlant()
	{
		if(currentZone == 0)
		{
			if(plantList1.Count < maxPlantPopZones1and3)
				zoneOneGeneration();
		}
		else if(currentZone == 1)
		{
			if(plantList2.Count < maxPlantPopulationZone2)
				zoneTwoGeneration();
		}
		else
		{
			if(plantList3.Count < maxPlantPopZones1and3)
				zoneThreeGeneration();
		}
	}

	private void setPlayerZone()
	{
		Vector2 xz = new Vector2 (this.transform.position.x, this.transform.position.z);
		if(AreaI.Contains(xz))
		{
			lastZone = 1;
			currentZone = 0;
			if(plantList1.Count < maxPlantPopZones1and3 / 2)
				generationCleanup = 3.0f;
			else
				generationCleanup = 10.0f;
		}
		else if(AreaIIi.Contains(xz) || AreaIIii.Contains(xz))
		{
			if(currentZone != 1)
			{
				if(currentZone == 0)
					lastZone = 0;
				else
					lastZone = 2;
			}
			currentZone = 1;

			if(plantList2.Count < maxPlantPopulationZone2 / 2)
				generationCleanup = 3.0f;
			else
				generationCleanup = 10.0f;
		}
		else
		{
			lastZone = 1;
			currentZone = 2;

			if(plantList3.Count < maxPlantPopZones1and3 / 2)
				generationCleanup = 3.0f;
			else
				generationCleanup = 10.0f;
		}
	}

	private void zoneOneGeneration()
	{
		Vector3 plantPos;
		GameObject newPlant;
		float height;
		Vector2 xz = new Vector2(Random.Range(AreaI.min.x, AreaI.max.x), 
		                 Random.Range(AreaI.min.y, AreaI.max.y));
		plantPos = new Vector3 (xz.x, 0, xz.y);
		plantPos.y = Terrain.activeTerrain.SampleHeight (plantPos);	//for use with complex terrains

		while(!rayCastingDetermination(xz, out height))
		{
			xz = new Vector2(Random.Range(AreaI.min.x, AreaI.max.x), 
			                 Random.Range(AreaI.min.y, AreaI.max.y));
		}

		plantPos = new Vector3 (xz.x, height, xz.y);

		if(checkPlantOverlap(plantPos))
		{
			newPlant = (GameObject)Instantiate(possiblePlants[Random.Range(0, possiblePlants.Length)],
			                                   plantPos, Quaternion.identity);
			plantList1.Add(newPlant);
		}
	}

	private void zoneTwoGeneration()
	{
		Vector2 xz;
		Vector3 plantPos;
		GameObject newPlant;
		float height;
		bool terrainSpawn = true;

		if(AreaIIi.Contains(new Vector2(this.transform.position.x, this.transform.position.z)))
		{
			xz = new Vector2(Random.Range(AreaIIi.min.x, AreaIIi.max.x), 
			                         Random.Range(AreaIIi.min.y, AreaIIi.max.y));
			while(!rayCastingDetermination(xz, out height))
			{
				xz = new Vector2(Random.Range(AreaIIi.min.x, AreaIIi.max.x), 
				                 Random.Range(AreaIIi.min.y, AreaIIi.max.y));
			}
			plantPos = new Vector3 (xz.x, height, xz.y);
		}
		else
		{
			xz = new Vector2(Random.Range(AreaIIii.min.x, AreaIIii.max.x), 
			                         Random.Range(AreaIIii.min.y, AreaIIii.max.y));
			while(!rayCastingDetermination(xz, out height))
			{
				xz = new Vector2(Random.Range(AreaIIi.min.x, AreaIIi.max.x), 
				                 Random.Range(AreaIIi.min.y, AreaIIi.max.y));
			}
			plantPos = new Vector3 (xz.x, height, xz.y);
		}

		if(checkPlantOverlap(plantPos))
		{
			newPlant = (GameObject)Instantiate(possiblePlants[Random.Range(0, possiblePlants.Length)],
			                                   plantPos, Quaternion.identity);
			plantList2.Add(newPlant);
		}

	}

	private void zoneThreeGeneration()
	{
		float height;
		Vector2 xz = new Vector2(Random.Range(AreaIII.min.x, AreaIII.max.x), 
		                         Random.Range(AreaIII.min.y, AreaIII.max.y));
		xz = streetPointConversion (xz);

		while(!rayCastingDetermination(xz, out height))
		{
			xz = new Vector2(Random.Range(AreaIII.min.x, AreaIII.max.x), 
			                         Random.Range(AreaIII.min.y, AreaIII.max.y));
			xz = streetPointConversion(xz);
		}

		Vector3 plantPos = new Vector3 (xz.x, height, xz.y);
		GameObject newPlant;

		if(checkPlantOverlap(plantPos))
		{
			newPlant = (GameObject)Instantiate(possiblePlants[Random.Range(0, possiblePlants.Length)],
			                                   plantPos, Quaternion.identity);
			plantList3.Add(newPlant);
		}
	}
	
	private bool checkPlantOverlap(Vector3 newPlantPos)
	{
		if(currentZone == 0)
			foreach(GameObject go in plantList1)
			{
				if((newPlantPos - go.transform.position).magnitude < horizontalCastDistance)
					return false;
			}
		else if(currentZone == 1)
			foreach(GameObject go in plantList2)
			{
				if((newPlantPos - go.transform.position).magnitude < horizontalCastDistance)
					return false;
			}
		else
			foreach(GameObject go in plantList3)
			{
				if((newPlantPos - go.transform.position).magnitude < horizontalCastDistance)
					return false;
			}
		return true;
	}

	public bool rayCastingDetermination(Vector2 xzPos, out float height)
	{
		if(Physics.Raycast(new Vector3(xzPos.x, verticalCastDistance/2, xzPos.y), Vector3.down, out info,
		                   	 verticalCastDistance+1.0f, castMask))
		{
			height = info.point.y;

			if(info.collider.gameObject.tag == "Water")
			{
				SendMessageUpwards("PositionEvent", info.point);
				return false;
			}
			else if(horizontalCastingTest(new Vector3(info.point.x, info.point.y + 1, info.point.z)))
			{
				return true;
			}
			return false;
		}

		height = 0;
		return false;
	}

	private bool horizontalCastingTest(Vector3 point)
	{
		for(int i=0;i<4;i++)
		{
			if(Physics.Raycast(point, new Vector3(Mathf.Cos (Mathf.PI/2 * i), 0, Mathf.Sin (Mathf.PI/2 * i)), 
			                   	out info, horizontalCastDistance))
			{	
				if(info.collider.gameObject.name != "CityStreet" && info.collider.gameObject.name != "Terrain")
				{
					return false;
				}
			}
		}
		return true;
	}

	private Vector2 streetPointConversion(Vector2 old)
	{
		return new Vector2 ((streets.transform.position.x - streets.transform.localScale.x/2) + old.x,
		                  (streets.transform.position.z - streets.transform.localScale.z/2) + old.y);
	}

	private IEnumerator generationUpdate()
	{
		while(true)
		{
			yield return new WaitForSeconds(generationCleanup);
			setPlayerZone();
			scrubOtherZones();
			AddPlant();
			//updateStaticZone();
		}
	}

	//Currently, we only scrub the zone the player resides in. Its possible to kill a plant in one zone 
	//from within another one, and that is a bug right now. Unity's runtime is robust and we don't give a shit though.
	private void scrubOtherZones()
	{
		if(currentZone == 0)
		{
			foreach(GameObject ls in plantList3)
			{
				deadList.Add(ls);
			}
			cleanUp(plantList3);
			foreach(GameObject ls in plantList2)
			{
				if(Vector3.Magnitude(ls.transform.position - transform.position) > killDistance)
				{
					deadList.Add (ls);
				}
			}
			cleanUp(plantList2);
		}
		else if(currentZone == 1)
		{
			foreach(GameObject ls in plantList1)
			{
				if(Vector3.Magnitude(ls.transform.position - transform.position) > killDistance)
				{
					deadList.Add (ls);
				}
			}
			cleanUp(plantList1);
			foreach(GameObject ls in plantList3)
			{
				if(Vector3.Magnitude(ls.transform.position - transform.position) > killDistance)
				{
					deadList.Add (ls);
				}
			}
			cleanUp(plantList3);
		}
		else
		{
			foreach(GameObject ls in plantList1)
			{
				deadList.Add(ls);
			}
			cleanUp(plantList1);
			foreach(GameObject ls in plantList2)
			{
				if(Vector3.Magnitude(ls.transform.position - transform.position) > killDistance)
				{
					deadList.Add (ls);
				}
			}
			cleanUp(plantList2);
		}
	}

	private void cleanUp(List<GameObject> plantList)
	{
		foreach(GameObject ls in deadList)
		{
			plantList.Remove(ls);
			ls.GetComponentInChildren<Parametric_Turtle>().destroySystem();
		}
		deadList.Clear ();
	}

	private void staticGeneration()
	{
		Vector3 plantPos;
		GameObject newPlant;
		float height;
		Vector2 xz = new Vector2(Random.Range(StaticZone.min.x, StaticZone.max.x), 
		                         Random.Range(StaticZone.min.y, StaticZone.max.y));
		
		while(!rayCastingDetermination(xz, out height))
		{
			xz = new Vector2(Random.Range(StaticZone.min.x, StaticZone.max.x), 
			                 Random.Range(StaticZone.min.y, StaticZone.max.y));
		}
		
		plantPos = new Vector3 (xz.x, height, xz.y);
		
		newPlant = (GameObject)Instantiate(possibleStatics[Random.Range(0, possibleStatics.Length)],
		                                   plantPos, Quaternion.identity);
		L_System lsa = newPlant.GetComponent<L_System>();
		// -set draw parameters
		lsa.edgeLength = 1.0f;
		lsa.angle = 20.0f;
		lsa.generations = Mathf.FloorToInt(Random.Range(2, 3));
		staticList.Add(newPlant);
	}

	private void updateStaticZone()
	{
		StaticZone = new Rect (transform.position.x-(zoneWidth/2), 
		                       transform.position.z-(zoneHeight/2), 
		                       zoneWidth, zoneHeight);
		foreach(GameObject go in staticList)
			if(!StaticZone.Contains(go.transform.position))
				deadList.Add(go);
		cleanUp (staticList);
		
		if(staticList.Count < maxStaticPopulation)
			staticGeneration();
	}

	public static void RemoveKilledLSystem(GameObject LSys)
	{
		if(plantList1.Contains(LSys))
			plantList1.Remove(LSys);
		else if(plantList1.Contains(LSys))
			plantList2.Remove(LSys);
		else
			plantList3.Remove(LSys);
	}
}
