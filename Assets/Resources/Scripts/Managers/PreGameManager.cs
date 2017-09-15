using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// enum
public enum MenuState{
	Default,
	Setup,
	Customization
}

// Map Size, 0 = small, 1 = medium, 2 = large
// Map resources, 0 = scarce, 1 = standard, 2 = abundant
public struct SetupGameInfo{
	public int playerCount;
	public int mapSize;
	public int mapResources;
	public int gamePhaseCycleTime;
	public int playerStartingResources;
}
	
/* Class Author: Alex DS  */
public class PreGameManager{
	
	// variables
	private Main mMain;
	private MenuState mCurMenu = MenuState.Default;
	private bool mConfirmation = false;	
	public GameObject mMainMenu;
	public GameObject mSetupMenu;
	public GameObject mCustomizeMenu;
	public GameObject mPlayerManager;
	public GameObject mSetupPlayerAmountSlider;
	
	/* Method Author: Alex DS */
	// constructor which is called at the start() of game
    public PreGameManager(Main main){
		mMain = main;
		
	    mMainMenu = GameObject.Find("Main Menu");
		mSetupMenu = GameObject.Find("Setup Menu");
		mCustomizeMenu = GameObject.Find("Customization Menu");
		mPlayerManager = GameObject.Find("PlayerManager");

		mPlayerManager.GetComponent<PlayerManager>().AvatarSettings.Model = 1;
		mPlayerManager.GetComponent<PlayerManager>().AvatarSettings.Color = 1;
		mPlayerManager.GetComponent<PlayerManager>().AvatarSettings.FacialExpression = 1;
		mSetupPlayerAmountSlider = GameObject.Find("PlayerAmountSlider");
        MainMenu();
	}

	/* Method Author: Alex DS */
	// UpdateMenu is called once per frame
	public void Update() {
		if( !mConfirmation ){ // incase we want functionality to stop cause we're waiting for a confirmation window
			MenuKeyboardHandler();
			
			Slider mSlider;
			Text mText;
			Dropdown mDropdown;
			switch( mCurMenu ){
				case MenuState.Default:
                    // if menu is not visible
                    // make visible
                    // hide all other menus or change draw priority?
				
					// make buttons which hookup to:
					// StartGame();
					// QuitGame();
					
					// Future implementation:
					// Setup() button
				
				break;
				case MenuState.Setup:
					mText = GameObject.Find("SliderPlayerCounter").GetComponent<Text>();
					mSlider = GameObject.Find("PlayerAmountSlider").GetComponent<Slider>();
					
					// update text slider
					mText.text = "Player Count: "+(int)mSlider.value;
					// save player count
					ChangePlayerCount((int)mSlider.value);
					
					// save map resource value
					mDropdown = GameObject.Find("Map Resources").GetComponent<Dropdown>();
					ChangeMapResource(mDropdown.value);
					
					// save map size value
					mDropdown = GameObject.Find("Map Size Dropdown").GetComponent<Dropdown>();
					ChangeMapSize(mDropdown.value);
					
					

					if( mDropdown.value == 0 ){
						mSetupPlayerAmountSlider.GetComponent<Slider>().maxValue = 3;
						if( mSetupPlayerAmountSlider.GetComponent<Slider>().value > 3 )
							mSetupPlayerAmountSlider.GetComponent<Slider>().value = 1;
					}else{					
						mSetupPlayerAmountSlider.GetComponent<Slider>().maxValue = 8;
					}
					string phaseText = GameObject.Find("Game Phase Cycle Time").GetComponent<InputField>().text;
					if( phaseText != null && phaseText != "" )
						mSetupInfo.gamePhaseCycleTime = int.Parse(phaseText);
					
					string resourceText = GameObject.Find("Starting Resources").GetComponent<InputField>().text;
					if( resourceText != null && resourceText != "")
						mSetupInfo.playerStartingResources = int.Parse(resourceText);
				break;
				case MenuState.Customization:
						// 0 = glasses
						// 1 = no glasses
						mDropdown = GameObject.Find("Model").GetComponent<Dropdown>();
						mPlayerManager.GetComponent<PlayerManager>().AvatarSettings.Model = mDropdown.value;
						// 0 = red
						// 1 = blue
						// 2 = grey
						mDropdown = GameObject.Find("Colors").GetComponent<Dropdown>();
						mPlayerManager.GetComponent<PlayerManager>().AvatarSettings.Color = mDropdown.value;
						// 0 = Shocked
						// 1 = Blink
						// 2 = Neutral
						// 3 = Smile
						// 4 = Sad
						mDropdown = GameObject.Find("Facial Expressions").GetComponent<Dropdown>();
						mPlayerManager.GetComponent<PlayerManager>().AvatarSettings.FacialExpression = mDropdown.value;
                        mPlayerManager.GetComponent<PlayerManager>().mIsAvatartCustomized = true;
                break;
			}
		}
	}

	/* Method Author: Alex DS */	
	// main handler for the menu keys
	private void MenuKeyboardHandler(){
		if( Input.GetKeyDown("escape") ){  // cancel any currently active windows
			Return(); // return out of current active pregame menu
		}
		if( Input.GetKeyDown("f1") ) // DEBUG
			StartGame();
	}

	/* Method Author: Alex DS */	
	// called when escape is detected from menukeyhandler() which de-actives all ui elements and sets main menu active
	private void Return(){
		if( mCurMenu != MenuState.Default ){ // set main menu as active
			mCurMenu = MenuState.Default;
			Debug.Log("Returning out of current menu");
			MainMenu();
		}else{
			Debug.Log("waiting for confirmation");
			// confirmation window to quit game
			// if confirmation is true
			// Quit();
		}
	}
	
	/* Method Author: Alex DS */
	// method which sets main menu active ui elements active and de-active others
	private void MainMenu(){
		mCurMenu = MenuState.Default;
		mSetupMenu.SetActive(false);	
		mMainMenu.SetActive(true);
		mCustomizeMenu.SetActive(false);
	}
		
	/* Method Author: Alex DS */
	// setup struct mutators - player count
	public void ChangePlayerCount(int count){
		mSetupInfo.playerCount = count;
	}
	/* Method Author: Alex DS */
	// setup struct mutators - map size
	public void ChangeMapSize(int mapSize){
		mSetupInfo.mapSize = mapSize;
	}
	/* Method Author: Alex DS */
	// setup struct mutators - map resources
	public void ChangeMapResource(int mapResources){
		mSetupInfo.mapResources = mapResources;
	}
	/* Method Author: Alex DS */
	// method which sets setup menu ui elements active and de-active others
	private SetupGameInfo mSetupInfo;
	private void Setup(){
		mCurMenu = MenuState.Setup;
		mMainMenu.SetActive(false);
		mSetupMenu.SetActive(true);
		mSetupInfo = new SetupGameInfo();
	}
	
	/* Method Author: Alex DS */
	// change to settings state / unused
	private void Customize(){
		mCurMenu = MenuState.Customization;
		mMainMenu.SetActive(false);
		mSetupMenu.SetActive(false); 
		mCustomizeMenu.SetActive(true);
	}
    
	/* Method Author: Alex DS */
    // starts the game - calls main.Start()
    private void StartGame(){
		mMainMenu.SetActive(false);
		mSetupMenu.SetActive(false);     
        mMain.StartGame();
	}
	
	/* Method Author: Alex DS */
	// starts the game with game info - calls main.Start(info)
    private void StartGame(SetupGameInfo info){
		mMainMenu.SetActive(false);
		mSetupMenu.SetActive(false);
		
		if( info.mapSize == 0 ){
			Debug.Log("small map");
			SceneManager.LoadScene("SmallMap", LoadSceneMode.Additive);	 // loads map
		}else if( info.mapSize == 1 ){
			Debug.Log("medium map");
			SceneManager.LoadScene("Map", LoadSceneMode.Additive);	 // loads map
		}else{
			Debug.Log("large map doesnt exist yet, loading medium instead");
			SceneManager.LoadScene("Map", LoadSceneMode.Additive);	 // loads map
		}

        mMain.StartGame(info);
	}
	
	/* Method Author: Alex DS */
	// quits the game 
	private void Quit(){
		mMain.QuitGame();
	}

	/* Method Author: Ka Ming Li */
	/* Method Editor: Alex DS */
    // reads incoming message from Main and sends to appropriate window
    public void ButtonClicked(string message)
    {
        switch (message)
        {
			case "Back": // returns active menu to main menu
				MainMenu();
			break;
            case "Setup": // set setup as active
                Setup();
                break;
            case "Play":
                StartGame();
                break;
			case "PlaySetup": // button click which starts the game and calls the right function
				StartGame(mSetupInfo);
			break;
			case "Customize":
				Customize();
			break;
            case "Quit":
			 #if UNITY_EDITOR // if unity editor
				UnityEditor.EditorApplication.isPlaying = false; // stop playing
			 #else // if not unity editor - it is an application
				Application.Quit(); // quit application
			 #endif
                break;
        }

    }
}
