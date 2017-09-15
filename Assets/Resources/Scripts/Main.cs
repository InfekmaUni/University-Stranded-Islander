using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* Class Author: Alex DS  */
// game info class for globaly accessible variables.
static class GameInfo {
	
	// centralised player object name
	private static string PLAYER_GAMEOBJECT_NAME = "Player";	
	public static string GetPlayerObjectName(){ return PLAYER_GAMEOBJECT_NAME; }
	private static GameObject PlayerObject;

	public static GameObject GetPlayerObject(){
		return GameObject.FindGameObjectWithTag(PLAYER_GAMEOBJECT_NAME);
	}
	
	// variable to determine whether game is playing
	// predominately used by script objects in the game to control their update
	private static bool isGamePlaying = false;
	public static bool GamePlaying{ get { return isGamePlaying; }
									set{ isGamePlaying = value; }
	}
	
	// variable to enable/disable debug camera
	// used to restrict playerobject
	private static bool DebugCameraFreeLook = false;
	public static bool DebugCamera{ get { return DebugCameraFreeLook; } 
									set{ DebugCameraFreeLook = value; }}
	
	// game speed
	private static int GameSpeedModifier = 1;
	public static int GameSpeed{ get{ return GameSpeedModifier; } 
									set{ GameSpeedModifier = value; }
	}
	
	// game phase intervals
	private static int GAME_PHASE_INTERVAL = 60;
	public static int GamePhaseInterval{ get { return GAME_PHASE_INTERVAL;}
										 set { GAME_PHASE_INTERVAL = value;}
										}
}

public enum GameState{
	PreGame,
	Loading,
	Playing,
	EndGame
}
	
/* Class Author: Alex DS  */
public class Main : MonoBehaviour {
	// enum which is used to re-direct the data flow of messages recieved by main.
	private GameState curGameState = GameState.PreGame;
	
	/* Managers */
	private HudManager mHudManager;
	public GameManager GetGameManager(){return mGameManager;}
	private PreGameManager mPreManager;
	private GameManager mGameManager;
	
	/* Game Objects and their components */
	private PlayerCamera mCamera;
	//private PlayerScript mLocalPlayer;
	//private GameObject mLocalPlayerObject;
	private GameObject mPreGameUI;
    public GameObject mInGameUI;
    public GameObject mPlayerManager;
    public GameObject mNPCManager;
    public GameObject mMiniMap;
	public GameObject mEndGameUI;

  	/* Method Author: Alex DS */
	// function which initialises many of the objects and saves many components to be used later.
    void Start () {
		mEndGameUI = GameObject.Find("EndGame");
		mEndGameUI.SetActive(false);
        mInGameUI = GameObject.Find("In-Game UI");
		mInGameUI.SetActive(false);
		mPreGameUI = GameObject.Find("PreGame");
        mPlayerManager = GameObject.Find("PlayerManager");
        mNPCManager = GameObject.Find("NPCManager");
        mPreManager = new PreGameManager(this);
		mHudManager = new HudManager(this);
		mGameManager = new GameManager();
		mCamera = GameObject.Find("PlayerCamera").GetComponent<PlayerCamera>();
        mMiniMap = GameObject.Find("MiniMapCamera");
		//mLocalPlayerObject = GameInfo.GetPlayerObject();
		//mLocalPlayer = mLocalPlayerObject.GetComponent<PlayerScript>();
    }
	
	/* Method Author: Alex DS */	
	// Update function which is controlled by the gamestate
	void Update () {
		Object.Destroy(GameObject.Find("Main Camera")); // destroy camera from map scene
		switch(curGameState){
			case GameState.PreGame:
                mMiniMap.SetActive(false); // de-active minimap
				mPreManager.Update(); // call the pregame manager update
				GameInfo.GamePlaying = false; // set game to playing as false
			break;
			case GameState.Playing:
				GameInfo.GamePlaying = true; // set game to playing as true
                mMiniMap.SetActive(true); // activate minimap
                mGameManager.Update(); // update call for game manager
				mHudManager.Update(); // update call for hudmanager
				mCamera.UpdateCamera(); // update call for camera
			break;
			case GameState.Loading:
				if( GameObject.Find("MainGameObject" ) != null )
					StartGame();
			break;
			case GameState.EndGame:
				GameInfo.GamePlaying = false;
			
				if( Input.GetKeyDown("escape") )
					QuitGame();
			break;			
		}
	}
	private Watch endWatch = new Watch();
	/* Method Author: Ka Ming Li */
    //sends message to specific manager when button pressed
    public void ButtonClicked(string message) {
        switch (curGameState)
        {
            case GameState.PreGame:
                mPreManager.ButtonClicked(message);
                break;
            case GameState.Playing:
                {
                    mHudManager.ButtonClicked(message);
                    if(mPlayerManager.GetComponent<PlayerManager>().mTradingSystem.mIsTrading)
                    {
                        mPlayerManager.GetComponent<PlayerManager>().ButtonClicked(message);
                    }
                    break;
                }
            case GameState.EndGame:
                break;
        }
    }

	/* Method Author: Alex DS */
	// starts the game and sets needed managers and components, called by the pregame manager
	public void StartGame(){
		curGameState = GameState.Playing; // set state to playing
        Debug.Log("Game is now playing");
		
		// enable/disable ui objects
        mInGameUI.SetActive(true);
		mPreGameUI.SetActive(false);

		// enable managers 
        mPlayerManager.GetComponent<PlayerManager>().enabled = true;
        GameObject.Find("FarmConstructor").GetComponent<FarmConstructor>().enabled = true;
        mNPCManager.GetComponent<NPCManager>().enabled = true;
		mGameManager.Start();
    }

	/* Method Author: Alex DS */
	// starts the game with setup information - called from pregame manager
	
	// Setup information :
    // Map Size, 0 = small, 1 = medium, 2 = large
	// Map resources, 0 = scarce, 1 = standard, 2 = abundant
	// info.mapSize
	//info.playerCount - int for how many ai players to create
	// info.mapResource
	public void StartGame(SetupGameInfo info){
		GameInfo.GamePhaseInterval = info.gamePhaseCycleTime;
        mNPCManager.GetComponent<NPCManager>().mNumberOfNPCS = info.playerCount;
		mPlayerManager.GetComponent<PlayerManager>().mDefaultNumberOfResources = info.playerStartingResources;
		curGameState = GameState.Loading;
    }
	
	/* Method Author: Alex DS */
	// ends the game and sets its properties
	public void EndGame(bool victory){
		curGameState = GameState.EndGame;        
		mInGameUI.SetActive(false);
		mMiniMap.SetActive(false); // de-activate minimap
		mEndGameUI.SetActive(true);	
		if( victory )
			GameObject.Find("Victory Text").GetComponent<Text>().text = "You Won, you were the first to leave!";
		else
			GameObject.Find("Victory Text").GetComponent<Text>().text = "You Lost, somebody left the island before you :(";
	}	
	
	/* Method Author: Alex DS */
	// quits the game - called from managers
	public void QuitGame(){
		Application.Quit();
	}	
}