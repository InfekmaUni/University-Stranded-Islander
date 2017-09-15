using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public struct PlayerAvatar{
	// 0 = glasses
	// 1 = no glasses
	public int Model;
	// 0 = red
	// 1 = blue
	// 2 = grey
	public int Color;
	// 0 = Shocked
	// 1 = Blink
	// 2 = Neutral
	// 3 = Sad
	// 4 = Smile
	public int FacialExpression;
}

/// <summary>
/// Class Author: Albert Dulian/// 
/// </summary
public class PlayerManager : MonoBehaviour
{    
    public int mStartingHealth;
    public int mStartingHydration;
    public int mDefaultNumberOfResources;
    public float mPlayerVelocity;
    public float mPlayerRotationSpeed;
    public Slider mHealthSlider;
    public Slider mHydrationSlider;
    public Text mResourceText;
    public GameObject mTradeWindow;
    public GameObject[] mPlayers;
    public Texture2D[] mClothColors;
    public Texture2D[] mFacialExpressions;

    [HideInInspector]
    public TradingSystem mTradingSystem;  
    
    private GameObject mPlayerToInstantiate;     
    private List<Transform> mSpawnPoints;
    private PlayerMovement mPlayerMovement;
    private PlayerResources mPlayerResources;
    private bool mIsDead;

    [HideInInspector]
	public PlayerAvatar AvatarSettings = new PlayerAvatar();
    [HideInInspector]
    public bool mIsAvatartCustomized = false;

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Start()
    {
        RandomSpawnPlayer();      
        mIsDead = false;

        mPlayerMovement = GetComponentInChildren<PlayerMovement>();
        mPlayerResources = GetComponentInChildren<PlayerResources>();

        mTradingSystem = new TradingSystem(mTradeWindow);
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Spawn player in one of the spawn points
    void RandomSpawnPlayer()
    {
        mSpawnPoints = new List<Transform>();
        GameObject SpawnPoints = GameObject.Find("SpawnPoints_Player");
        
        for(int i = 0; i < SpawnPoints.transform.childCount; i++)
        {
            mSpawnPoints.Add(SpawnPoints.transform.GetChild(i));            
        }
        
        if (mSpawnPoints.Count <= 0)
            return;
        
            
        int rand = Random.Range(0, mSpawnPoints.Count);

        CustomizePlayer();

        GameObject child = Instantiate(mPlayerToInstantiate, mSpawnPoints[rand].position, mSpawnPoints[rand].rotation) as GameObject;       
        child.transform.parent = transform;        
		CreatePlayerTerritory(child, mSpawnPoints[rand].position, true);
    }
				
	/* Author: Alex DS */
	public static void CreatePlayerTerritory(GameObject player, Vector3 pos, bool localPlayer){
		GameObject obj;
		
		// create territory collider
		obj = Instantiate(Resources.Load("Prefabs/Territory"), pos, Quaternion.identity) as GameObject;
		obj.name = "Territory_Zone_"+player.name;
		obj.GetComponent<Territory>().mAttachedPlayer = player;
		obj.GetComponent<Territory>().mLocalPlayer = localPlayer;
		
		// create boat for that player
		obj = Instantiate(Resources.Load("Prefabs/Boat prefab"), pos + new Vector3(0,0,-25), Quaternion.identity) as GameObject;
		obj.name = "Territory_Boat_"+player.name;
        obj.GetComponent<BoatScript>().owner = player;
	}

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Customize Model of the player
    void CustomizePlayer()
    {
        //If player customization has been skipped, set the avatar to default settings 
        if(!mIsAvatartCustomized)
        {
            AvatarSettings = DefaultAvatar();
        }

        mPlayerToInstantiate = mPlayers[AvatarSettings.Model];    

        //Cloths
        mPlayerToInstantiate.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[0].mainTexture = mClothColors[AvatarSettings.Color];
        
        //Facial Expression
        mPlayerToInstantiate.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials[1].mainTexture = mFacialExpressions[AvatarSettings.FacialExpression];
        
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    PlayerAvatar DefaultAvatar()
    {
        PlayerAvatar tempAvatar  = new PlayerAvatar();
        tempAvatar.Model = 0;
        tempAvatar.Color = 0;
        tempAvatar.FacialExpression = 0;

        return tempAvatar;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Update()
    {		
        if (GameInfo.GamePlaying && !mIsDead)
        {
            if(mPlayerResources.GetCurrentHealth <= 0)
            { Death(); } //--->GameOverScene

            UpdateResourcesInfo();
            mTradingSystem.Update();
        }
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Updates Players Resource UI
    void UpdateResourcesInfo()
    {
        if (mResourceText == null)
            return;
        mResourceText.text = mPlayerResources.GetNumberOfWood.ToString() + "\n\n";
        mResourceText.text += mPlayerResources.GetNumberOfAdhesives.ToString() + "\n\n";
        mResourceText.text += mPlayerResources.GetNumberOfFabric.ToString() + "\n\n";
        mResourceText.text += mPlayerResources.GetNumberOfBerries.ToString() + "\n\n";
        mResourceText.text += mPlayerResources.GetNumberOfFish.ToString() + "\n\n";
           
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Increase Speed (Only in ExplorationPhase)
    public void SetSpeedBonus(float increaseSpeedPercentage)
    {
        float bonusSpeed = mPlayerVelocity * (increaseSpeedPercentage / 100);       
        mPlayerVelocity = mPlayerVelocity + bonusSpeed;        
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void ButtonClicked(string button)
    {        
        mTradingSystem.TradeButtonClicked(button);
    }
    
    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Death()
    {
        if(!mIsDead)
            mIsDead = true;
    }

    //------------------------------------------------------------   
    //Methods Author: Albert Dulian
    //GET PROPERTIES
    public PlayerMovement GetPlayerTransformInfo
    {
        get { return mPlayerMovement; }
    }

    public PlayerResources GetPlayerResourcesInfo
    {
        get { return mPlayerResources; }
    }  

    public float GerRotationSpeed
    {
        get { return mPlayerRotationSpeed; }
    }    

    public float GetPlayerVelocity
    {
        get { return mPlayerVelocity; }
    }

    public bool IsPlayerDead
    {
        get { return mIsDead; }
    }
}
