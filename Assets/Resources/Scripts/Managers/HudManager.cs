using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/* Class Author: Alex DS  */
public class HudManager
{
    // gameObjects
    GameObject mPlayerResourceWindow;
    GameObject mActionOptions;
    GameObject mPhase;

    private Main mMain;
    Watch mWatch;

    Dictionary<int, bool> VisibleOptions = new Dictionary<int, bool>();


    /* Method Author: Alex DS*/
    /* Editor: Ka Ming Li*/
    // constructor which setups objects and de-actives other menu's
    public HudManager(Main main)
    {
        mMain = main;
        mMain.mInGameUI.transform.FindChild("TradingWindow").gameObject.SetActive(false);        
        mActionOptions = mMain.mInGameUI.transform.FindChild("Action Bar").gameObject;
        for(int i = 1; i < 4; i++)
        {
            VisibleOptions.Add(i, false);
        }

        mPhase = mActionOptions.transform.Find("Action4").gameObject.transform.FindChild("Text").gameObject;
        mWatch = GameManager.mGameWatch;

        Text action11Info = mActionOptions.transform.Find("Action1-1 Info").gameObject.transform.FindChild("Values").gameObject.GetComponent<Text>();
        action11Info.text = "" + BerriesFarmConstructionValues()["Wood"] + "\n\n" + BerriesFarmConstructionValues()["Adhesive"] + "\n\n" +
                            BerriesFarmConstructionValues()["Fabric"] + "\n\n" + BerriesFarmConstructionValues()["Berries"];
        Text action12Info = mActionOptions.transform.Find("Action1-2 Info").gameObject.transform.FindChild("Values").gameObject.GetComponent<Text>();
        action12Info.text = "" + WoodFarmConstructionValues()["Wood"] + "\n\n" + WoodFarmConstructionValues()["Adhesive"] + "\n\n" +
                            WoodFarmConstructionValues()["Fabric"];
        Text action13Info = mActionOptions.transform.Find("Action1-3 Info").gameObject.transform.FindChild("Values").gameObject.GetComponent<Text>();
        action13Info.text = "" + AdhesivesFarmConstructionValues()["Wood"] + "\n\n" + AdhesivesFarmConstructionValues()["Adhesive"] + "\n\n" +
                            AdhesivesFarmConstructionValues()["Fabric"];
    }
	
	/* Method Author: Alex DS */
	// main update call
	public void Update() {
		if( !localPlayer ) // if player isnt set
			CheckAndSetPlayer(); // find and set player
		
		HudKeyHandler(); // hud key handler
		UpdateMinimap(); // update minimap
        UpdateBoatProgress();
        CanConstructUpdate();
        UpdatePhaseInfo();
    }
	
	/* Method Author: Alex DS */
	// update call for minimap
	private void UpdateMinimap(){
		
	}

	/* Method Author: Alex DS */	
	// this toggles the inventory visibility on and off
	bool visibleInventory = false;
	private void ToggleInventory(){
		visibleInventory = !visibleInventory; // possibility to use a boolean value.
                                              // toggle ui element containing inventory off / hide its visibility

	}

    /* Author: Ka Ming Li*/
    bool visibleMiniMap = false;
    private void ToggleMiniMap()
    {
        visibleMiniMap = !visibleMiniMap;
    }

    /* Method Author: Alex DS */
    // main keyboard handler for hudmanager
    private void HudKeyHandler(){
		// toggle inventory window
		if( Input.GetKeyDown("i") ){
			ToggleInventory();
			Debug.Log("inventory now :"+visibleInventory);
		}
		
		// cancel active windows 1 at a time
		if( Input.GetKeyDown("escape") ){
			if( visibleInventory ) // if active inventory - close it
				ToggleInventory();
			
			// close any other windows
		}
		
		if( visibleInventory ){ // if inventory is visible
			InventoryKeys(); // check for inventory keys
		}else{// if inventory is not active
			ActionBarKeys(); // actionbar key handler function
			MinimapKeys(); // minimap key handler function
		}
	}

	/* Method Author: Alex DS */		
	// keys only when inventory is active
	private void InventoryKeys(){
		if( Input.GetKeyDown("escape") ) // toggle off inventory
			visibleInventory = false;
	}

	/* Method Author: Alex DS */
    /*Editor: Ka Ming Li*/
	// keys for the actionbar only active when inventory is not
	private void ActionBarKeys(){
		for(int i =1; i <= 3; i++){ // loop through 1-3 keys and check
			if( Input.GetKeyDown(""+i) ){
                ShowOptions(i);
				Debug.Log("key pressed: "+i);
			}
		}
	}

    private bool optionsShowing = false;
    /*Author: Ka Ming Li*/
    // shows the appropriate action options when button is used from action bar
    private void ShowOptions(int action)
    {
        if(optionsShowing)
        {
            // if options for an action are showing, pressing 1-3 or 4 activates corresponding options
            Actions(action);
        }
        else
        {
            // if no action options showing, show options for action corresponding to buttons 1-3
            optionsShowing = !optionsShowing;
            VisibleOptions[action] = !VisibleOptions[action];
            for (int i = 1; i < 4; i++)
            {
                if (i != action && VisibleOptions[i])
                {
                    VisibleOptions[i] = !VisibleOptions[i];
                }
                mActionOptions.transform.Find("Action" + i + " Options").gameObject.SetActive(VisibleOptions[i]);
            }

        }
    }

    // enum for options of action 3
    enum OptionSelected
    {
        Option31,
        Option32,
        Option33,
        Default
    };
    private OptionSelected optionSelected = OptionSelected.Default;

    /*Author: Ka Ming Li*/
    // detects which option has been selected from the selected action and performs function
    private void Actions(int option)
    {
        int action = VisibleOptions.FirstOrDefault(x => x.Value == true).Key;
        switch (action)
        {
            // farms to be constructed
            case 1:
                PlayerResources playerResources = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetPlayerResourcesInfo;
                switch (option)
                {
                    // if not enough resources, does not carry out any actions
                    case 1:
                        if(!mActionOptions.transform.Find("Action1 Options").gameObject.transform
                                    .FindChild("Option1").gameObject.GetComponent<Button>().interactable)
                        {
                            break;
                        }
                        else
                        {
                            // build berries farm
                            GameObject.Find("FarmConstructor").GetComponent<FarmConstructor>().ChooseFarm(1);
                            playerResources.SubtractConstructionResources(BerriesFarmConstructionValues());
                        }
                        break;
                    case 2:
                        if (!mActionOptions.transform.Find("Action1 Options").gameObject.transform
                                    .FindChild("Option2").gameObject.GetComponent<Button>().interactable)
                        {
                            break;
                        }
                        else
                        {
                            // builds wood farm
                            GameObject.Find("FarmConstructor").GetComponent<FarmConstructor>().ChooseFarm(2);
                            playerResources.SubtractConstructionResources(WoodFarmConstructionValues());
                        }
                        break;
                    case 3:
                        if (!mActionOptions.transform.Find("Action1 Options").gameObject.transform
                                 .FindChild("Option3").gameObject.GetComponent<Button>().interactable)
                        {
                            break;
                        }
                        else
                        {
                            // builds adhesives farm
                            GameObject.Find("FarmConstructor").GetComponent<FarmConstructor>().ChooseFarm(3);
                            playerResources.SubtractConstructionResources(AdhesivesFarmConstructionValues());
                        }
                        break;
                }
                break;
            // items discovered and picked up by player
            case 2:
                switch (option)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
                break;
            //boat progress by amount of resources needed
            case 3:
                switch (option)
                {
                    case 1:
                        optionSelected = OptionSelected.Option31;
                        break;
                    case 2:
                        optionSelected = OptionSelected.Option32;
                        break;
                    case 3:
                        optionSelected = OptionSelected.Option33;
                        break;
                }
                break;
        }
        // hides options panel once choice is made
        VisibleOptions[action] = !VisibleOptions[action];
        mActionOptions.transform.Find("Action" + action + " Options").gameObject.SetActive(VisibleOptions[action]);
        optionsShowing = !optionsShowing;
    }

    /*Author: Ka Ming Li*/
    // determines if player has enough resources to construct farms
    private void CanConstructUpdate()
    {
        PlayerResources playerResources = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetPlayerResourcesInfo;

        GameObject option1 = mActionOptions.transform.Find("Action1 Options").gameObject.transform
                                    .FindChild("Option1").gameObject;

        GameObject option2 = mActionOptions.transform.Find("Action1 Options").gameObject.transform
                                    .FindChild("Option2").gameObject;

        GameObject option3 = mActionOptions.transform.Find("Action1 Options").gameObject.transform
                                    .FindChild("Option3").gameObject;

        // determines if berries farm can be constructed
        if (playerResources.GetNumberOfWood < BerriesFarmConstructionValues()["Wood"] || playerResources.GetNumberOfAdhesives < BerriesFarmConstructionValues()["Adhesive"] ||
            playerResources.GetNumberOfFabric < BerriesFarmConstructionValues()["Fabric"] || playerResources.GetNumberOfBerries < BerriesFarmConstructionValues()["Berries"])
        {
            // if not enough resources, button is unclickable and red
            option1.GetComponent<Button>().interactable = false;
        }
        else
        {
            // if enough resources, makes button clickable and green
            option1.GetComponent<Button>().interactable = true;
        }

        // determines if wood farm can be constructed
        if (playerResources.GetNumberOfWood < WoodFarmConstructionValues()["Wood"] || playerResources.GetNumberOfAdhesives < WoodFarmConstructionValues()["Adhesive"] || 
            playerResources.GetNumberOfFabric < WoodFarmConstructionValues()["Fabric"])
        {
            option2.GetComponent<Button>().interactable = false;
        }
        else
        {
            option2.GetComponent<Button>().interactable = true;
        }

        // determines if adhesives farm can be constructed
        if (playerResources.GetNumberOfWood < AdhesivesFarmConstructionValues()["Wood"] || playerResources.GetNumberOfAdhesives < AdhesivesFarmConstructionValues()["Adhesive"] ||
            playerResources.GetNumberOfFabric < AdhesivesFarmConstructionValues()["Fabric"])
        {
            option3.GetComponent<Button>().interactable = false;
        }
        else
        {
            option3.GetComponent<Button>().interactable = true;
        }
    }

    private int boatWoodNeeded = 210, boatAdhesivesNeeded = 50, boatFabric = 100;
    /*Author: Ka Ming Li*/
    // updates count for resource focused on in action bar for boat progress
    private void UpdateBoatProgress()
    {
        PlayerResources playerResources = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetPlayerResourcesInfo;

        GameObject option1 = mActionOptions.transform.Find("Action3 Options").gameObject.transform
                                    .FindChild("Option1").gameObject;

        GameObject option2 = mActionOptions.transform.Find("Action3 Options").gameObject.transform
                                    .FindChild("Option2").gameObject;

        GameObject option3 = mActionOptions.transform.Find("Action3 Options").gameObject.transform
                                    .FindChild("Option3").gameObject;

        // indicates when player has enough of a resource for the boat
        if (playerResources.GetNumberOfWood >= boatWoodNeeded)
        {
            option1.GetComponent<Image>().color = Color.green;
        }
        if (playerResources.GetNumberOfAdhesives >= boatAdhesivesNeeded)
        {
            option2.GetComponent<Image>().color = Color.green;
        }
        if (playerResources.GetNumberOfFabric >= boatFabric)
        {
            option3.GetComponent<Image>().color = Color.green;
        }

        //update all resource counts for boat progress
        option1.transform.FindChild("Text").GetComponent<Text>().text = "Wood\n" + playerResources.GetNumberOfWood + " / " + boatWoodNeeded;
        option2.transform.FindChild("Text").GetComponent<Text>().text = "Adhesive\n" + playerResources.GetNumberOfAdhesives + " / " + boatAdhesivesNeeded;
        option3.transform.FindChild("Text").GetComponent<Text>().text = "Fabric\n" + playerResources.GetNumberOfFabric + " / " + boatFabric;

        
        Text buttonText = mActionOptions.transform.Find("Action3").gameObject.transform.Find("Text").GetComponent<Text>();
        // update action bar button with count for resource selected to be focused
        switch (optionSelected)
        {
            case OptionSelected.Option31:
                buttonText.text = option1.transform.FindChild("Text").GetComponent<Text>().text;
                break;
            case OptionSelected.Option32:
                buttonText.text = option2.transform.FindChild("Text").GetComponent<Text>().text;
                break;
            case OptionSelected.Option33:
                buttonText.text = option3.transform.FindChild("Text").GetComponent<Text>().text;
                break;
        }
    }

    /*Author: Ka Ming Li*/
    // updates the fourth panel on the action bar with the current game phase and time remaining for that phase
    private void UpdatePhaseInfo()
    {
        Text phaseInfo = mPhase.GetComponent<Text>();

        phaseInfo.text = "Phase:\n" + GameManager.mCurGamePhase + "\n-  " + (int)mWatch.TimeRemaining() + "  s";
    }

	/* Method Author: Alex DS */		
	// keys for the minimap only active when inventory is not	
	private void MinimapKeys(){
		if( Input.GetKeyDown("m") ){
            ToggleMiniMap();
			Debug.Log("minimap being toggled off");
			// toggle minimap on/off
		}
	}

	/* Method Author: Alex DS */	
    // find player and save him for future reference
    private GameObject localPlayer = null;
	public void CheckAndSetPlayer(){
		localPlayer = GameInfo.GetPlayerObject();
		if( localPlayer ){}
	}

    /* Author: Ka Ming Li*/
    static private int woodOffer = 0, adhesiveOffer = 0, fabricOffer = 0,
            berriesOffer = 0, fishOffer = 0;
    // increments resource up when add button pressed and updates display
    private void Add(string resource)
    {
        PlayerResources playerResources = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetPlayerResourcesInfo;

        // prevents player from offering more resources than they hold
        switch(resource)
        {
            case "Wood":
                if(woodOffer <= playerResources.GetNumberOfWood - 1)
                    woodOffer += 1;
                break;
            case "Adhesive":
                if (adhesiveOffer <= playerResources.GetNumberOfAdhesives - 1)
                    adhesiveOffer += 1;
                break;
            case "Fabric":
                if (fabricOffer <= playerResources.GetNumberOfFabric - 1)
                    fabricOffer += 1;
                break;
            case "Berries":
                if (berriesOffer <= playerResources.GetNumberOfBerries - 1)
                    berriesOffer += 1;
                break;
            case "Fish":
                if (fishOffer <= playerResources.GetNumberOfFish - 1)
                    fishOffer += 1;
                break;
        }

        Text resourceOffer = GameObject.Find("PlayerOffer").GetComponent<Text>();
        resourceOffer.text = woodOffer + "\n\n\n" + adhesiveOffer + "\n\n\n" + fabricOffer + 
                        "\n\n\n" + berriesOffer + "\n\n\n" + fishOffer;
    }
    /* Author: Ka Ming Li*/
    // decrements resource when minus button pressed and updates display
    private void Minus(string resource)
    {
        switch (resource)
        {
            // prevents player from offering zero or a negative number of resources
            case "Wood":
                if(woodOffer > 0)
                    woodOffer -= 1;
                break;
            case "Adhesive":
                if (adhesiveOffer > 0)
                    adhesiveOffer -= 1;
                break;
            case "Fabric":
                if (fabricOffer > 0)
                    fabricOffer -= 1;
                break;
            case "Berries":
                if (berriesOffer > 0)
                    berriesOffer -= 1;
                break;
            case "Fish":
                if (fishOffer > 0)
                    fishOffer -= 1;
                break;
        }
        Text resourceOffer = GameObject.Find("PlayerOffer").GetComponent<Text>();
        resourceOffer.text = woodOffer + "\n\n\n" + adhesiveOffer + "\n\n\n" + fabricOffer +
                         "\n\n\n" + berriesOffer + "\n\n\n" + fishOffer;
    }

    /* Author: Ka Ming Li*/
    // reads incoming message from Main and carries out the appropriate action
    public void ButtonClicked(string message)
    {
        switch (message)
        {
            case "AccepTrade":
                break;
            case "DeclineTrade":                
                break;
            case "StealTrade":
                break;
            default:
                if(message.Contains("Action"))
                {
                    int action = int.Parse(message.Remove(0,6));
                    if(!optionsShowing)
                    {
                        ShowOptions(action);
                    }
                    else
                    {
                        optionsShowing = !optionsShowing;
                        for (int i = 1; i < 4; i++)
                        {
                            if (VisibleOptions[i])
                            {
                                VisibleOptions[i] = !VisibleOptions[i];
                                mActionOptions.transform.Find("Action" + i + " Options").gameObject.SetActive(VisibleOptions[i]);
                            }
                            else if(i == action)
                            {
                                ShowOptions(action);
                            }
                        }
                    }
                }
                else if (message.Contains("Option"))
                {
                    int option = int.Parse(message.Remove(0, 6));
                    Actions(option);
                }
                else if (message.Contains("Add"))
                {
                    string resource = message.Remove(0, 3);
                    Add(resource);
                }
                else if (message.Contains("Minus"))
                {
                    string resource = message.Remove(0, 5);
                    Minus(resource);
                }
                break;
        }

    }

    /*Author: Ka Ming Li*/
    static private Dictionary<string, int> BerriesFarmConstructionValues()
    {
        Dictionary<string, int> tempDictionary = new Dictionary<string, int>();
        tempDictionary.Add("Wood", 15);
        tempDictionary.Add("Adhesive", 8);
        tempDictionary.Add("Fabric", 8);
        tempDictionary.Add("Berries", 8);

        return tempDictionary;
    }
    /*Author: Ka Ming Li*/
    static private Dictionary<string, int> WoodFarmConstructionValues()
    {
        Dictionary<string, int> tempDictionary = new Dictionary<string, int>();
        tempDictionary.Add("Wood", 35);
        tempDictionary.Add("Adhesive", 8);
        tempDictionary.Add("Fabric", 8);
        tempDictionary.Add("Berries", 0);

        return tempDictionary;
    }
    /*Author: Ka Ming Li*/
    static private Dictionary<string, int> AdhesivesFarmConstructionValues()
    {
        Dictionary<string, int> tempDictionary = new Dictionary<string, int>();
        tempDictionary.Add("Wood", 15);
        tempDictionary.Add("Adhesive", 35);
        tempDictionary.Add("Fabric", 8);
        tempDictionary.Add("Berries", 0);

        return tempDictionary;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    static public Dictionary<string,int> GetOfferedValues()
    {
        Dictionary<string, int> tempDictionary = new Dictionary<string, int>();
        tempDictionary.Add("Wood", woodOffer);
        tempDictionary.Add("Adhesive", adhesiveOffer);
        tempDictionary.Add("Fabric", fabricOffer);
        tempDictionary.Add("Berries", berriesOffer);
        tempDictionary.Add("Fish", fishOffer);

        return tempDictionary;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    static public void ResetOfferedValues()
    {
        woodOffer = 0;
        adhesiveOffer = 0;
        fabricOffer = 0;
        berriesOffer = 0;
        fishOffer = 0;

        Text resourceOffer = GameObject.Find("PlayerOffer").GetComponent<Text>();
        resourceOffer.text = woodOffer + "\n\n\n" + adhesiveOffer + "\n\n\n" + fabricOffer +
                        "\n\n\n" + berriesOffer + "\n\n\n" + fishOffer;
    }
}
