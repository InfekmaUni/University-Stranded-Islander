using UnityEngine;
using System.Collections;

// Alex DS
public enum FarmType{
	WoodFarm,
	BerryFarm,
	AdhesiveFarm
}

// Alex DS
public struct ResourceCost{
	private int wood,adhesives,berries,fabric;
	
	public void Init(int inWood, int inAdhesives, int inFabric, int inBerries){ wood = inWood; adhesives = inAdhesives; berries = inBerries; fabric = inFabric;}
	public int GetWoodCost(){ return wood; }
	public int GetAdhesivesCost(){ return adhesives; }
	public int GetBerriesCost(){ return berries; }
	public int GetFabricCost(){ return fabric; }
}

// Alex DS
public struct Resource{
	public string resourceName;
	public int amount;
	public int max;
}

/* Class Author : Alex DS */
public class Farm : MonoBehaviour {

	private int mAmountGeneratedPerCycle = 0;
	private int mGenerationInterval = 0;
	//private int mMaxCapacity = 0;
	//private int mResource = 0;
	
	private string mName = "";
	public string Name { get{ return Name; } }
	public bool isFarm = true;
	
	public bool isEmpty{ get{ return mResource.amount <= 0; } }
	
	private Watch[] mWatch = new Watch[2] { new Watch(), new Watch() };
	// watch index 0 = watch for resource generation
	// watch index 1 = watch to enable building if previously disabled
	private FarmType mFarm = FarmType.WoodFarm;
	public FarmType GetFarmType{ get { return mFarm; } }
	
	private Resource mResource = new Resource();
		
	private ResourceCost mResourceCost = new ResourceCost();
	public ResourceCost ResourceCost{ get{return mResourceCost;} }
	
	public bool disabled = false;
	private int mDisabledTime = GameInfo.GamePhaseInterval * 3; // gets renabled next time invasion starts
	
	/* Method Author: Alex DS */
	// initialisating method
	void Start () {
		mName = this.gameObject.name;
		// determine farm type
		if( mName.Contains("Wood") )
			mFarm = FarmType.WoodFarm;
		else if( mName.Contains("Adhesive") )
			mFarm = FarmType.AdhesiveFarm;
		else
			mFarm = FarmType.BerryFarm;
	
		int interval = 20;	
		switch(mFarm){
			case FarmType.WoodFarm:
				SetFarmProperties("Wood", 4,interval,80);
				mResourceCost.Init(35,8,8,0); // resource cost = 35 wood, 8 adhesives, 8 fabric, 0 berries
			break;
			case FarmType.BerryFarm:
				SetFarmProperties("Berry", 10,interval,200);
				mResourceCost.Init(15,8,8,60);// resource cost = 15 wood, 8 adhesives, 8 fabric, 60 berries
			break;
			case FarmType.AdhesiveFarm:
				SetFarmProperties("Adhesive", 2,interval,40);
				mResourceCost.Init(15,35,8,0); // resource cost = 15 wood, 35 adhesives, 8 fabric, 0 berries
			break;
		}
		
		//DebugLogFarmCost();
	}

	/* Method Author: Alex DS */
	// method which prints out the cost of the farm
	private void DebugLogFarmCost(){
		Debug.Log("start");
		Debug.Log("Wood "+mResourceCost.GetWoodCost());
		Debug.Log("Adhesive "+mResourceCost.GetAdhesivesCost());
		Debug.Log("Berries "+mResourceCost.GetBerriesCost());
		Debug.Log("Fabrics "+mResourceCost.GetFabricCost());
		Debug.Log("end");
	}
	/* Method Author: Alex DS */
	// this method sets the initial farm properties
	private void SetFarmProperties(string name, int i, int j, int k){
		mAmountGeneratedPerCycle = i;
		mGenerationInterval = j;
		mResource.max = k;
		mResource.resourceName = name;
		mResource.amount = 1;
	}

	void Update () {
		if( !disabled ){ // if farm is not disabled
			Generate();
		}else
			DisabledUpdate();
	}
	
	/* Method Author: Alex DS */
	// this method handles the updating when the farm is disabled, once this method is succesfull the farm is re-enabled.
	private void DisabledUpdate(){
		if(mWatch[1].Done()){ // if watch is done
			disabled = false;
			mWatch[1].Start(mDisabledTime);
		}else if( !mWatch[1].isInit() ) // if watch was previously not initialised
			mWatch[1].Start(mDisabledTime); // start watch
		
		mWatch[1].Update();
	}

	/* Method Author: Alex DS */
	// method which is triggered everytime the farm is interacted with, makes sure the right amount of resources are removed
	public int Collect(int amount){
		int val = amount; // current value to be harvested
		if( mResource.amount <= 0 ) // if empty
			val = 0; // gather nothing
		else if( mResource.amount < amount ) // if amount to be collected is smaller then what farm contains
			val = mResource.amount;
		
		GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("Collected "+val+"x "+mResource.resourceName);
		
		mResource.amount -= val;
		return val;
	}

	/* Method Author: Alex DS */
	// this method handles the generation of the farm and controls when the maximum capcity is reached
	private void Generate(){
		if(mWatch[0].Done()){ // if watch is done
			mResource.amount += mAmountGeneratedPerCycle; // add to resource
			mWatch[0].Start(mGenerationInterval); // restart watch
		}else if( !mWatch[0].isInit() ) // if watch was previously not initialised
			mWatch[0].Start(mGenerationInterval); // start watch
		
		if( mResource.amount < mResource.max ) // if resource is not yet at max capacity, continue
			mWatch[0].Update();
	}

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Zone_NPC"))
        {
            GetComponentInParent<FarmConstructor>().IsInNPCTerritory(true);    
        }
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Zone_NPC"))
        {
            GetComponentInParent<FarmConstructor>().IsInNPCTerritory(false);
        }
    }

}