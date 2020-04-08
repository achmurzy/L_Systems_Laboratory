using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    CharacterMachine character;
    Material grass_shader;
    void Start()
    {
        character = FindObjectOfType<CharacterMachine>();
        grass_shader = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            grass_shader.SetVector("_CharacterPosition",
                    new Vector3(character.transform.position.x, character.transform.position.y, character.transform.position.z));
            if (Vector3.Distance(character.transform.position, this.transform.position) < (1.4 + grass_shader.GetFloat("_CharacterGenerationRadius")))
            {
                //this.GetComponent<MeshRenderer>().enabled = true;

            }
            //else
            //    this.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
