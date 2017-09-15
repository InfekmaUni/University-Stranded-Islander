using UnityEngine;
using System.Collections;

/// <summary>
/// -----Author: Callum Milner-----
/// Handles the spawning of fabric
/// </summary>
public class FabricScript : MonoBehaviour {

    //Stores the maximum amount of fabric; the current amount of fabric and the respawn rate of fabric
    public int maxFabric = 10;
    public int amountOfFabric = 5;
    public float respawnRate = 20;

    //Sets the initial colour of fabric and the respawn timer
    private Color fabricColor;
    private Watch fabricSpawn = new Watch();

    //Method Author: Callum Milner
    //Allows the harvesting of fabric. If already depleted then returns false
    public bool harvestFabric()
    {
        if(amountOfFabric > 0)
        {
            amountOfFabric--;
            if(amountOfFabric == 0)
            {
                fabricColor.a = 0.1f;
                this.gameObject.GetComponent<Renderer>().material.color = fabricColor;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

	// Method Author: Callum Milner
    //Initialises the fabric colour to be used for transparency and begins the respawn timer
	void Start () {
        fabricColor = this.gameObject.GetComponent<Renderer>().material.color;
        fabricSpawn.Start(respawnRate);
	}
	
	// Method Author: Callum Milner
    //Updates the respawn timer and adds to the numberOf Fabric if the timer is done
	void Update () {
        fabricSpawn.Update();

        if(amountOfFabric < maxFabric - 1)
        {
            if(fabricSpawn.Done())
            {
                fabricColor.a = 1;
                this.gameObject.GetComponent<Renderer>().material.color = fabricColor;
                amountOfFabric++;
                if(amountOfFabric < maxFabric)
                {
                    fabricSpawn.Start(respawnRate);
                }
            }
        }
	}
}
