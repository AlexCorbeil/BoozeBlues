using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPooler : MonoBehaviour
{



    public GameObject pitStopPlatform;
    public GameObject endPlatform;

    private List<GameObject> platforms;
    private int NumPlatsPerSection = 5;
    private int platformIndex; 
    private Vector3 lastPlatPos; 
    private int sectionsNum = 2; 
    private Vector3 initPos;

    void Awake()
    {
        //Creates the list of platforms and initializes the platforms to zero and index to 1
        platforms = new List<GameObject>();
        initPos = Vector3.zero;
        lastPlatPos = Vector3.zero;
        platformIndex = 1;

        //Adds all the child platforms to the platform list
        foreach (Transform child in transform)
        {
            platforms.Add(child.gameObject);
        }

        //Deactivates all the platforms in the list and makes sure they are set to their initial position
        foreach (GameObject plat in platforms)
        {
            plat.SetActive(false);
            plat.transform.position = initPos;
        }

        //There's gotta be a better way to write all this but whatever...
        //spawns a set of platforms for the level
        SpawnPlatforms();

        while (sectionsNum > 0)
        {
            SpawnSpecialPlatform(lastPlatPos, pitStopPlatform);
            SpawnPlatforms();
            sectionsNum--;
        }

        SpawnSpecialPlatform(lastPlatPos, endPlatform);

    }

    //Picks a number at random and returns the platform with that index number, then removes it from the list
    GameObject PickRandomPlatform()
    {
        int randoNum = Random.Range(0, platforms.Count);
        if (platforms[randoNum].tag != "PitStopPlatform") //Makes sure it doesn't add a pitstop to the randomized platforms
        {
            GameObject platPicked = platforms[randoNum];
            platforms.RemoveAt(randoNum);
            return platPicked;
        }
        else
        {
            return null; //Returns nothing if a pitstop platform is selected by the randomizer
        }
    }

    //Takes the random number from the PickRandomPlatform function and spawns the platform assigned to it
    void SpawnPlatforms()
    {
        GameObject currentPlatform;

        for (int i = 0; i < NumPlatsPerSection; i++)
        {
            currentPlatform = PickRandomPlatform();

            //If the randompicker returns nothing (aka a pitstop), it'll skip it and try again
            if (currentPlatform != null)
            {
                currentPlatform.SetActive(true); //Turns on the platforms
                currentPlatform.transform.position += new Vector3(40 * platformIndex, 0, 0); //Makes sure the platforms don't stack on one another
                lastPlatPos = currentPlatform.transform.position; //Stores the last platform's position
                platformIndex++; //increases the platform index to make sure the next one is placed in the correct spot
            }
            else
            {
                i--;
            }
        }
        lastPlatPos += new Vector3(40, 0, 0); //Moves the position of the last platform to prevent platforms overlapping with the pitstop

    }

    //Spawns a special platform (pitstop or endplatform) after the last platform

    void SpawnSpecialPlatform(Vector3 pos, GameObject plat)
    {
        GameObject specialPlat = Instantiate(plat, pos, Quaternion.identity) as GameObject;
        specialPlat.transform.parent = transform;
        lastPlatPos += new Vector3(40, 0, 0);
        platformIndex++;
    }

    //Puts the platforms back in the pool
    //Public function as it will be used in the Shredder script
    public void AddPlatformBack(GameObject platform)
    {
        platforms.Add(platform);
    }
}