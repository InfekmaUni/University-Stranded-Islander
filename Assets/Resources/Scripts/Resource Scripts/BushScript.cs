using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// -----Author: Callum Milner-----
/// Handles the spawning of berries
/// </summary>
public class BushScript : MonoBehaviour {

    //Holds values for the maximum amount of berries, the current amount of berries and the respawn rate.
    public int maxBerries = 10;
    public int numberOfBerries = 5;
    public float respawnRate = 20;

    //Holds values for the positions of berries and the color of the bush used for transparency
    private List<Vector3> berryPositions = new List<Vector3>();
    private Color bushColor;

    //Uses the berry gameobject to initialise the others. Initialised berries are stored in the list
    GameObject berry;
    List<GameObject> berries = new List<GameObject>();

    //Used to time the spawning of berries
    public Watch spawnBerries = new Watch();

    //Method Author: Callum Milner
    //Sets a berry's active value to false if there is one or more berries. Otherwise it returns false
    //Changes the transparency of the bush to half if fully harvested to make it obvious to the player
    public bool harvestBerries()
    {
        if (numberOfBerries > 0)
        {
            berries[numberOfBerries - 1].SetActive(false);
            numberOfBerries--;
            if(numberOfBerries == 0)
            {
                
                bushColor.a = 0.5f;
                this.gameObject.GetComponent<Renderer>().material.color = bushColor;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //Method Author: Callum Milner
    //Initialises the color and sets the positions for each of the berries.
    //For berries over the current amount, they are set as false to begin with
    void Start () {
        bushColor = this.gameObject.GetComponent<Renderer>().material.color;

        berry = this.gameObject.transform.GetChild(0).gameObject;

        berryPositions.Add(new Vector3(4, 4, 0));
        berryPositions.Add(new Vector3(-3.2f, 4, 0));
        berryPositions.Add(new Vector3(-3.2f, 4, -2));
        berryPositions.Add(new Vector3(1, 2, -4));
        berryPositions.Add(new Vector3(-2, 2, -4));
        berryPositions.Add(new Vector3(3, 2, 1));
        berryPositions.Add(new Vector3(4, 3, -2));
        berryPositions.Add(new Vector3(3, 3, -4));
        berryPositions.Add(new Vector3(-4, 4, 3));
        berryPositions.Add(new Vector3(0, 1, 5));

        for (int i = 0; i < maxBerries - 1; i++)
        {
            GameObject berryCopy = (GameObject)Instantiate(berry, (berryPositions[i] / 2) + new Vector3(0, 1, 0) + this.gameObject.transform.position, Quaternion.identity);
            //berryCopy.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            berries.Add(berryCopy);
            if(i > numberOfBerries - 1)
            {
                berryCopy.SetActive(false);
            }
        }
        Destroy(berry); //initial berry no longer needed

        spawnBerries.Start(respawnRate); //Begins the respawn timer
	}
	
    //Method Author: Callum Milner
    //Updates the respawn timer, sets a berry's active value to true if the respawn timer is done and increments numberOfBerries
	void Update () {
        spawnBerries.Update();
        //if watch has reached respawnRate then spawn berries and reset watch if the bush isn't full
        if(numberOfBerries < maxBerries - 1) //If we've not reached max amount of berries
        {
            if(spawnBerries.Done())
            {
                bushColor.a = 1;
                this.gameObject.GetComponent<Renderer>().material.color = bushColor;
                berries[numberOfBerries].SetActive(true);

                numberOfBerries++;
                if(numberOfBerries < maxBerries)
                {
                    spawnBerries.Start(respawnRate);
                }
            }
        }
	}
}
