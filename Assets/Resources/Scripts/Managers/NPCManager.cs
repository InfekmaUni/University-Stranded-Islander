using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class Author: Albert Dulian
/// </summary>
public class NPCManager : MonoBehaviour
{
    public int mNumberOfNPCS;
    public int mStartingNrOfResources;   
    public GameObject mNPC;

    private List<Transform> mSpawnPoints;

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Start()
    {
        SpawnNPCs();      
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Update()
    {
       
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Spawn x number of NPCs at spawn points
    void SpawnNPCs()
    {
        mSpawnPoints = new List<Transform>();

        GameObject SpawnPoints = GameObject.Find("SpawnPoints_AI");

        for (int i = 0; i < SpawnPoints.transform.childCount; i++)
        {
            mSpawnPoints.Add(SpawnPoints.transform.GetChild(i));
        }

        if (mSpawnPoints.Count <= 0)
            return;
 


        for (int i = 0; i < mNumberOfNPCS; i++)
        {            
            GameObject child = Instantiate(mNPC, mSpawnPoints[i].position, mSpawnPoints[i].rotation) as GameObject;
            child.transform.parent = transform;
			
			PlayerManager.CreatePlayerTerritory(child, mSpawnPoints[i].position, false);
			
            NPCResources childRes = child.GetComponent<NPCResources>();
            childRes.SetDefaultAmountOfResources(mStartingNrOfResources);
        }
    }
}