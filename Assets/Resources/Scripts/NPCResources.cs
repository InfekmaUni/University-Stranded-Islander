using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class Author: Albert Dulian
/// Controls resources held by each NPC
/// </summary>
public class NPCResources : MonoBehaviour
{
    private int mWood, mAdhesives ,mFabric, mBerries, mFish, mWater;

    //------------------------------------------------------------
    void Start ()
    {
	
	}

    //------------------------------------------------------------
    void Update ()
    {
	
	}

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void SetDefaultAmountOfResources(int nrOfRes)
    {
        mWood = nrOfRes;
        mAdhesives = nrOfRes;
        mFabric = nrOfRes;
        mBerries = nrOfRes;
        mFish = nrOfRes;
        mWater = nrOfRes;        
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void AddTradingResources(Dictionary<string, int> offeredByPlayer)
    {
        mWood += offeredByPlayer["Wood"];
        mAdhesives += offeredByPlayer["Adhesive"];
        mFabric += offeredByPlayer["Fabric"];
        mBerries += offeredByPlayer["Berries"];
        mFish += offeredByPlayer["Fish"];
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void SubtractTradingResources(Dictionary<string, int> offeredByNPC)
    {
        mWood -= offeredByNPC["Wood"];
        mAdhesives -= offeredByNPC["Adhesive"];
        mFabric -= offeredByNPC["Fabric"];
        mBerries -= offeredByNPC["Berries"];
        mFish -= offeredByNPC["Fish"];
    }

    //------------------------------------------------------------
    //Methods Author: Albert Dulian
    //GET PROPERTIES
    public int GetNumberOfWood
    {
        get { return mWood; }
    }
    public int GetNumberOfAdhesives
    {
        get { return mAdhesives; }

    }
    public int GetNumberOfFabric
    {
        get { return mFabric; }
    }
    public int GetNumberOfBerries
    {
        get { return mBerries; }
    }
    public int GetNumberOfFish
    {
        get { return mFish; }
    }

    public int GetNumberOfWater
    {
        get { return mWater; }
    }

}
