using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class Author: Albert Dulian
/// -----Edited by: Callum Milner-----
/// Trigger collider when interacting with resources,
/// update the number of resources.
/// harvests gameObject from world
/// </summary>
public class PlayerResources : MonoBehaviour
{
    enum PickupResourceType {WOOD, ADHESIVES, FABRIC, BERRIES, FISH, WATER, DEFAULT };

    private PlayerManager mPlayerManager;
    private Slider mHealthSlider;
    private Slider mHydrationSlider;
    private float mHealthTimer;
    private Collider otherCollider;

    //Resources
    private int mWood, mAdhesives, mFabric, mBerries, mFish;
    public bool mHammer = false, mFishingRod = false, mSturdyBoots = false, mProtectiveGloves = false; //Used to determine if the player has each of the items
	private int mFarmHarvestRate = 4;
    private int mCurrentHealth;
    private int mCurrentHydration;
    private PickupResourceType mType = PickupResourceType.DEFAULT;
    private bool mIsTrigger = false;
    private bool mCooldown = false;
    private Watch mCooldownWatch = new Watch();

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Awake()
    {
       
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Start()
    {
        mPlayerManager = GetComponentInParent<PlayerManager>();
        mCurrentHealth = mPlayerManager.mStartingHealth;
        mCurrentHydration = mPlayerManager.mStartingHydration;

        mHealthSlider = mPlayerManager.mHealthSlider;
        mHydrationSlider = mPlayerManager.mHydrationSlider;

        SetDefaultNrOfResources();
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Update()
    {
        if (GameInfo.GamePlaying && !mPlayerManager.IsPlayerDead)
        {
            Degenerate();
            UpdateStats();
            PickupResources();
  
            if(Input.GetKeyDown("e"))
            {
                consumeBerries();
            }
        }
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Edited by: Callum Milner
    //Edited to harvest resources fromt the world. 
    void PickupResources()
    {
        //PICKUP RESOURCES
        if (mIsTrigger)
        {
            if (Input.GetKeyDown("f") && !mCooldown)
            {
                if (otherCollider != null)
                {
                    if (otherCollider.name.Contains("Farm"))
                    { // is farm
                        HarvestFromFarm();

                        for (int i = 0; i < mFarmHarvestRate - 1; i++)
                            AddResources(mType);
                    }else if( otherCollider.name.Contains("Boat")){
						Debug.Log("interacted with boat");
						otherCollider.gameObject.GetComponent<BoatScript>().BuildBoat(this.gameObject);
					}
                    else
                    {
                        bool successfulHarvest;
                        successfulHarvest = HarvestResourceFromWorld();
                        if(successfulHarvest == true)
                        {
                            AddResources(mType); //Wont run if the resource being harvested is depleted
                        }else
							GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("This resource is empty", true);
                    }
                }

				
                mCooldown = true;
                mCooldownWatch.Start(.5f); //Wait .5 sec before pickin up resources again
            }
        }

        //Check for the cooldown 
        if (mCooldown)
        {
            mCooldownWatch.Update();

            if (mCooldownWatch.Done())
            {
                mCooldown = false;
            }
        }
    }
	
	/* Method Author: Alex DS */
	// method which handles the functions needed to be called if harvesting from construction object.
	private void HarvestFromFarm(){
		Farm farm = otherCollider.gameObject.GetComponent<Farm>();
		if( farm != null ){
			if( !farm.isEmpty ){
				farm.Collect(mFarmHarvestRate); // function which is triggered when collected from
			}else{
				mType = PickupResourceType.DEFAULT; // sets the gathering to false
				GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Cannot collect from an empty farm", true);
			}
		}else
			Debug.Log("logic fell into HarvestFromFarm but object is not farm");
	}

    //Method author: Callum Milner
    //Used to harvest from the resources. This returns false if the resource is depleted
    //Also adds items to the player and removes them from the map if taken
    bool HarvestResourceFromWorld()
    {
        bool successfulHarvest = false;

        if(otherCollider.gameObject.name.Contains("Bush"))
        {
            successfulHarvest = otherCollider.gameObject.GetComponent<BushScript>().harvestBerries();
        }
        if(otherCollider.gameObject.name.Contains("Adhesive"))
        {
            successfulHarvest = otherCollider.gameObject.GetComponent<AdhesiveScript>().harvestAdhesive();
        }
        if(otherCollider.gameObject.name.Contains("Wood"))
        {
            successfulHarvest = otherCollider.gameObject.GetComponent<WoodScript>().harvestWood();
        }
        if(otherCollider.gameObject.name.Contains("Fabric"))
        {
            successfulHarvest = otherCollider.gameObject.GetComponent<FabricScript>().harvestFabric();
        }

        if(otherCollider.gameObject.name.Contains("Hammer"))
        {
            mHammer = true;
            Destroy(otherCollider.gameObject);
			successfulHarvest = true;
			GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Found a Hammer");	
        }
        if(otherCollider.gameObject.name.Contains("Sturdy Boots"))
        {
            mSturdyBoots = true;
            Destroy(otherCollider.gameObject);
			successfulHarvest = true;
			GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Found Sturdy Boots");
        }
        if(otherCollider.gameObject.name.Contains("Protective Gloves"))
        {
            mProtectiveGloves = true;
            Destroy(otherCollider.gameObject);
			successfulHarvest = true;
			GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Found protective gloves");
        }
        if (otherCollider.gameObject.name.Contains("Fishing Rod"))
        {
            mFishingRod = true;
            Destroy(otherCollider.gameObject);
			successfulHarvest = true;
			GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Found a fishing rod");
        }
        return successfulHarvest;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Check type of resource
    //Edited by: Callum Milner
    //Edited to accept items as triggers and set the otherCollider so that the resources can be harvested
    void OnTriggerEnter(Collider other)
    {
        otherCollider = other;
        if (other.gameObject.name.Contains("Wood"))
        {
            mIsTrigger = true;
            mType = PickupResourceType.WOOD;
        }
        else if (other.gameObject.name.Contains("Adhesive"))
        {
            mIsTrigger = true;
            mType = PickupResourceType.ADHESIVES;
        }
        else if (other.gameObject.name.Contains("Fabric"))
        {
            mIsTrigger = true;
            mType = PickupResourceType.FABRIC;
        }
        else if (other.gameObject.name.Contains("Bush") || other.gameObject.name.Contains("Berry")) //Berries
        {
            mIsTrigger = true;
            mType = PickupResourceType.BERRIES;
        }
        else if (other.gameObject.name.Contains("Sardine")) //Fish
        {
            mIsTrigger = true;
            mType = PickupResourceType.FISH;
        }
        

        else if(other.gameObject.name.Contains("Sturdy Boots"))
        {
            mIsTrigger = true;
        }
        else if (other.gameObject.name.Contains("Fishing Rod"))
        {
            mIsTrigger = true;
        }
        else if (other.gameObject.name.Contains("Hammer"))
        {
            mIsTrigger = true;
        }
        else if (other.gameObject.name.Contains("Protective Gloves"))
        {
            mIsTrigger = true;
        }else if( other.gameObject.name.Contains("Boat") )
			mIsTrigger = true;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Edited by: Callum Milner
    //Edited to set otherCollider to null when the player exits the collider
    void OnTriggerExit(Collider other)
    {
        mIsTrigger = false;
        mCooldown = false;
        mType = PickupResourceType.DEFAULT;
        otherCollider = null;
    }

    //------------------------------------------------------------    
    //Method Author: Albert Dulian
    //Updates the slider values and RegenerateHealth
    void UpdateStats()
    {        
        ReplenishHealth();

        mHealthSlider.value = mCurrentHealth;
        mHydrationSlider.value = mCurrentHydration;
    }
	
	/* Method Author: Alex DS */
	// this method handles the degeneration of the hydration and health
	private Watch mDegenWatch = new Watch();
	private int degenInterval = 2;
	void Degenerate(){
		if( mDegenWatch.Done() ){ // if watch is done
			if( mCurrentHydration > 0 ) // if hydration left
				mCurrentHydration -= 1;
			else // if no hydration, remove health
				mCurrentHealth -= 1;
			
			mDegenWatch.Start(degenInterval); // restart watch
		}else if( !mDegenWatch.isInit() ) // if watch has not been initialised
			mDegenWatch.Start(degenInterval); // start watch
		
		mDegenWatch.Update(); // update watch
	}

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Replenish Health Every Sec
    void ReplenishHealth()
    {                
        if (mCurrentHealth < 100 && mCurrentHydration > 0)
        {
            mHealthTimer += Time.deltaTime;
            float replenishHealthTime = 2.5f;

            while (mHealthTimer > replenishHealthTime)
            {
                mCurrentHealth++;
                mHealthTimer -= replenishHealthTime;

                if (mCurrentHealth == 100)
                    break;              
            }
        }
    }

    //Method Author: Callum Milner
    //Replenishes health and hydration slightly and decreases the number of berries by 1
    void consumeBerries()
    {
        if(mBerries > 0)
        {
            mBerries--;
            if(mCurrentHydration < 100)
            {
                mCurrentHydration += 5;
            }
            if(mCurrentHealth < 100)
            {
                mCurrentHealth += 5;
            }
        }
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Pick up the resource depending on the type 
    void AddResources(PickupResourceType type)
    {
        switch (type)
        {
            case PickupResourceType.WOOD:
                {
                    mWood++;
					GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Collected 1x Wood");
                }
                break;
            case PickupResourceType.ADHESIVES:
                {
                    mAdhesives++;
					GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Collected 1x Adhesives");
                }
                break;
            case PickupResourceType.FABRIC:
                {
                    mFabric++;
					GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Collected 1x Fabric");
                }
                break;
            case PickupResourceType.BERRIES:
                {
                    mBerries++;
					GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Collected 1x Berries");
                }
                break;
            case PickupResourceType.FISH:
                {
                    mFish++;
					GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Collected 1x fish");
                }
                break;
            case PickupResourceType.WATER:
                {
                    
                }
                break;
            case PickupResourceType.DEFAULT:
                {
                }
                break;
        }
    }

    public void SubtractConstructionResources(Dictionary<string, int> constructionCosts)
    {
        mWood -= constructionCosts["Wood"];
        mAdhesives -= constructionCosts["Adhesive"];
        mFabric -= constructionCosts["Fabric"];
        mBerries -= constructionCosts["Berries"];
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void AddTradingResources(Dictionary<string,int> offeredByNPC)
    {
        mWood += offeredByNPC["Wood"];
        mAdhesives += offeredByNPC["Adhesive"];
        mFabric += offeredByNPC["Fabric"];
        mBerries += offeredByNPC["Berries"];
        mFish += offeredByNPC["Fish"];      
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void SubtractTradingResources(Dictionary<string, int> offeredByPlayer)
    {
        mWood -= offeredByPlayer["Wood"];
        mAdhesives -= offeredByPlayer["Adhesive"];
        mFabric -= offeredByPlayer["Fabric"];
        mBerries -= offeredByPlayer["Berries"];
        mFish -= offeredByPlayer["Fish"];
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void SubtractHealth(int value)
    {
        mCurrentHealth -= value;

        if (mCurrentHealth < 0)
            mCurrentHealth = 0;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void SetDefaultNrOfResources()
    {
        mWood = mPlayerManager.mDefaultNumberOfResources;
        mAdhesives = mPlayerManager.mDefaultNumberOfResources;
        mFabric = mPlayerManager.mDefaultNumberOfResources;
        mBerries = mPlayerManager.mDefaultNumberOfResources;
        mFish = mPlayerManager.mDefaultNumberOfResources;
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

    public int GetNumberOfAllResources
    {
        get { return mWood + mAdhesives + mFabric + mBerries + mFish; }
    }

    public int GetCurrentHealth
    {
        get { return mCurrentHealth; }
    }

    public int GetCurrentHydration
    {
        get { return mCurrentHydration; }
    }
}
