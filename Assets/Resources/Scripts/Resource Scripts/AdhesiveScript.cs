using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// -----Author: Callum Milner-----
/// Handles the spawning of adhesives
/// </summary>
public class AdhesiveScript : MonoBehaviour {

    //Holds values for the maximum amount of adhesive, the current amount of adhesive and the time it takes to respawn
    public int numberOfAdhesives = 5;
    public int maxAdhesives = 10;
    public float respawnRate = 20;
    private Color adhesiveColor;
    private Watch spawnAdhesive = new Watch();

    //Method author: Callum Milner
    //Handles the harvesting of adhesives. Returns false if it is already fully depleted
    public bool harvestAdhesive()
    {
        if(numberOfAdhesives > 0)
        {
            numberOfAdhesives--;
            if(numberOfAdhesives == 0)
            {
                adhesiveColor.a = 0.2f;
                this.gameObject.GetComponent<Renderer>().material.color = adhesiveColor;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //Method Author: Callum Milner
    //Initialises the colour of the adhesive which is used for setting transparency and starts the respawn timer
    void Start () {
        adhesiveColor = this.gameObject.GetComponent<Renderer>().material.color;
        spawnAdhesive.Start(respawnRate);
	}
	
    //Method Author: Callum Milner
    //Updates the respawn timer and adds to the adhesive count
	void Update () {
        spawnAdhesive.Update();

        if(numberOfAdhesives < maxAdhesives - 1)
        {
            if(spawnAdhesive.Done())
            {
                adhesiveColor.a = 1;
                this.gameObject.GetComponent<Renderer>().material.color = adhesiveColor;
                numberOfAdhesives++;
                if(numberOfAdhesives < maxAdhesives)
                {
                    spawnAdhesive.Start(respawnRate);
                }
            }
        }
	}
}
