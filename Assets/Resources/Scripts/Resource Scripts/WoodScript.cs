using UnityEngine;
using System.Collections;

/// <summary>
/// -----Author: Callum Milner-----
/// Handles the spawning of wood
/// </summary>
public class WoodScript : MonoBehaviour {

    //Used to store values for maximum amount of wood; respawn rate of wood and current amount of wood.
    public int maxWood = 10;
    public int amountOfWood = 5;
    public float respawnRate = 20;

    //Used for setting the wood transparency
    private Color woodColor;

    //Watch used for the respawning of wood
    private Watch woodSpawn = new Watch();

    //Method author: Callum Milner
    //Used for the harvesting of wood. Returns false if already fully depleted
    public bool harvestWood()
    {
        if (amountOfWood > 0)
        {
            amountOfWood--;
            if (amountOfWood == 0)
            {
                this.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0f);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    //Method Author: Callum Milner
    //Used to initialise the color and start the respawn watch
    void Start()
    {
        woodColor = this.gameObject.GetComponent<Renderer>().material.color;
        woodSpawn.Start(respawnRate);
    }

    //Method Author: Callum Milner
    //Used to update respawn watch and to add to the current amount of wood if the timer has finished
    void Update()
    {
        woodSpawn.Update();

        if (amountOfWood < maxWood - 1)
        {
            if (woodSpawn.Done())
            {
                woodColor.a = 1;
                this.gameObject.GetComponent<Renderer>().material.color = woodColor;
                if (amountOfWood < maxWood)
                {
                    woodSpawn.Start(respawnRate);
                }
                amountOfWood++;

            }
        }
    }
}
