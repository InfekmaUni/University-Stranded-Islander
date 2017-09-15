using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Class Author: Albert Dulian
/// ##################################################
/// Player - NPC Trading:
/// 1. Number of resources offered by NPC to a player is generated randomly
/// 2. Accepting the offer will +/- number of resources offered by both Player and NPC
/// 3. Steal - 50% chances for successful steal
///  - If steal is successful a random number of resources from NPC will be taken and given to the player
///  - If steal is unsuccessful player's health will be reduced by 25%
/// ##################################################
/// </summary>
public class TradingSystem 
{
    public bool mIsTrading;
    private Transform mPlayer;
    private Transform mNPCTarget;
    private PlayerResources mPlayerResources;
    private int tradeDistance;
    private GameObject mTradeWindow;
    private Dictionary<string, int> mOfferedByPlayer;
    private Dictionary<string, int> mOfferedByNPC;

    //TRADE WINDOWS
    private GameObject mPlayerResourcesWindow;
    private GameObject mNPCResourcesWindow;
    private GameObject mPlayerOfferWindow;
    private GameObject mNPCOfferWindow;
	private Logger mLogger;

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public TradingSystem(GameObject _tradeWindow)
    {
        mPlayer = GameObject.Find("PlayerManager").transform.GetChild(0);        
        mPlayerResources = mPlayer.GetComponent<PlayerResources>();
        mTradeWindow = _tradeWindow;
        mOfferedByPlayer = new Dictionary<string, int>();
        mOfferedByNPC = new Dictionary<string, int>();
        mNPCTarget = null;

        mPlayerResourcesWindow = mTradeWindow.transform.FindChild("CurrentResources").gameObject;
        mNPCResourcesWindow = mTradeWindow.transform.FindChild("OtherPlayerHiddenResources").gameObject;
        mPlayerOfferWindow = mTradeWindow.transform.FindChild("OfferWindows").transform.FindChild("You").gameObject;
        mNPCOfferWindow = mTradeWindow.transform.FindChild("OfferWindows").transform.FindChild("OtherPlayer").gameObject;

		mLogger = GameObject.Find("Game Main Logic").GetComponent<Logger>();
        mIsTrading = false;
        tradeDistance = 5;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void Update()
    {
        if(Input.GetKeyDown("t"))
        {            
            if(!mIsTrading)
            {                       
                Trade();
            }          
        }    
            
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Trade()
    {        
        if(IsInRange(out mNPCTarget)) //TRADE IF IN RANGE AND FACING NPC
        {
            UpdatePlayersListOfResources();
            UpdateNPCListOfResources();
            NPCOfferResources();

            mIsTrading = true;
            mTradeWindow.SetActive(true);
           
        }
    }

    //Check if there is a potential NPC in front of a player
    //------------------------------------------------------------
    //Method Author: Albert Dulian
    bool IsInRange(out Transform NPC)
    {
        NPC = null;
        Ray ray = new Ray(mPlayer.position, mPlayer.forward);
        RaycastHit hitRay;       
    
        if (Physics.Raycast(ray, out hitRay, tradeDistance))
        {
            if (hitRay.collider.tag == "NPC")
            {
                NPC = hitRay.transform;
                return true;
            }
        }

        return false;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void TradeButtonClicked(string button)
    {
        switch(button)
        {
            case "AcceptTrade":
                {
                    AcceptTrade();
                    break;
                }
            case "DeclineTrade":
                {
                    DeclineTrade();
                    break;
                }
            case "StealTrade":
                {
                    StealTrade();
                    break;
                }
            default:
                return;
        }
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void AcceptTrade()
    {
        mOfferedByPlayer = HudManager.GetOfferedValues();

		mLogger.mLoggerTrade.PlayerSuccesfullTrade.Add(mOfferedByPlayer);
		mLogger.mLoggerTrade.AiSuccesfullTrade.Add(mOfferedByNPC);
		mLogger.mLoggerTrade.accepted++;
		mLogger.mLoggerTrade.totalSuccesfullTrade++;
		
        mPlayerResources.AddTradingResources(mOfferedByNPC);
        mPlayerResources.SubtractTradingResources(mOfferedByPlayer);

        mNPCTarget.GetComponent<NPCResources>().AddTradingResources(mOfferedByPlayer);
        mNPCTarget.GetComponent<NPCResources>().SubtractTradingResources(mOfferedByNPC);

        HudManager.ResetOfferedValues();

        mIsTrading = false;
        mTradeWindow.SetActive(false);
        mNPCTarget = null;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void DeclineTrade()
    {
		mLogger.mLoggerTrade.PlayerUnSuccesfullTrade.Add(HudManager.GetOfferedValues());
		mLogger.mLoggerTrade.AiUnSuccesfullTrade.Add(mOfferedByNPC);
		mLogger.mLoggerTrade.declined++;
		mLogger.mLoggerTrade.totalUnsuccesfullTrade++;
		
        HudManager.ResetOfferedValues();

        mIsTrading = false;
        mTradeWindow.SetActive(false);
        mNPCTarget = null;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void StealTrade()
    {
        // > .5 - Successful 
        // <= .5 - Unsuccessful

        float steal = Random.Range(0.0f, 1.0f);
        if(steal > 0.5f)
        {
            Dictionary<string, int> stealResources = new Dictionary<string, int>();
            NPCResources npcRes = mNPCTarget.GetComponent<NPCResources>();
            System.Random rand = new System.Random();

            stealResources.Add("Wood", rand.Next(0, npcRes.GetNumberOfWood));
            stealResources.Add("Adhesive", rand.Next(0, npcRes.GetNumberOfAdhesives));
            stealResources.Add("Fabric", rand.Next(0, npcRes.GetNumberOfFabric));
            stealResources.Add("Berries", rand.Next(0, npcRes.GetNumberOfBerries));
            stealResources.Add("Fish", rand.Next(0, npcRes.GetNumberOfFish));

            mPlayerResources.AddTradingResources(stealResources);             
            mNPCTarget.GetComponent<NPCResources>().SubtractTradingResources(stealResources);
			
			mLogger.mLoggerTrade.PlayerStolenTrade.Add(HudManager.GetOfferedValues());
			mLogger.mLoggerTrade.AIStolenTrade.Add(mOfferedByNPC);
			mLogger.mLoggerTrade.totalSuccesfullTrade++;
        }
        else
        {
			
			mLogger.mLoggerTrade.totalUnsuccesfullTrade++;	
			mLogger.mLoggerTrade.PlayerUnsuccesfullStolenTrade.Add(HudManager.GetOfferedValues());
			mLogger.mLoggerTrade.AiUnsuccesfullStolenTrade.Add(mOfferedByNPC);
            mPlayerResources.SubtractHealth(25); // 1/4 of health
        }

		mLogger.mLoggerTrade.stolen++;
		
        HudManager.ResetOfferedValues();

        mIsTrading = false;
        mTradeWindow.SetActive(false);
        mNPCTarget = null;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void UpdatePlayersListOfResources()
    {
        Text resourceText = mPlayerResourcesWindow.transform.Find("You").transform.Find("ResourceNumbers").gameObject.GetComponent<Text>();

        resourceText.text = mPlayerResources.GetNumberOfWood.ToString() + "\n\n";
        resourceText.text += mPlayerResources.GetNumberOfAdhesives.ToString() + "\n\n";
        resourceText.text += mPlayerResources.GetNumberOfFabric.ToString() + "\n\n";
        resourceText.text += mPlayerResources.GetNumberOfBerries.ToString() + "\n\n";
        resourceText.text += mPlayerResources.GetNumberOfFish.ToString() + "\n\n";
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void UpdateNPCListOfResources()
    {
        Text resourceText = mNPCResourcesWindow.transform.Find("ResourceNumbers").gameObject.GetComponent<Text>();
        NPCResources npcRes = mNPCTarget.GetComponent<NPCResources>();

        resourceText.text = npcRes.GetNumberOfWood.ToString() + "\n\n";
        resourceText.text += npcRes.GetNumberOfAdhesives.ToString() + "\n\n";
        resourceText.text += npcRes.GetNumberOfFabric.ToString() + "\n\n";
        resourceText.text += npcRes.GetNumberOfBerries.ToString() + "\n\n";
        resourceText.text += npcRes.GetNumberOfFish.ToString() + "\n\n";
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void NPCOfferResources( )
    {
        Text resourceText = mNPCOfferWindow.transform.Find("ListOfResources2").transform.Find("OtherOffer").gameObject.GetComponent<Text>();
        NPCResources npcRes = mNPCTarget.GetComponent<NPCResources>();
        System.Random rand = new System.Random();

        int woodOffer = rand.Next(0, npcRes.GetNumberOfWood);
        int adhesiveOffer = rand.Next(0, npcRes.GetNumberOfAdhesives);
        int fabricOffer= rand.Next(0, npcRes.GetNumberOfFabric);
        int berriesOffer= rand.Next(0, npcRes.GetNumberOfBerries);
        int fishOffer= rand.Next(0, npcRes.GetNumberOfFish);

        mOfferedByNPC.Clear();
        mOfferedByNPC.Add("Wood", woodOffer);
        mOfferedByNPC.Add("Adhesive", adhesiveOffer);
        mOfferedByNPC.Add("Fabric", fabricOffer);
        mOfferedByNPC.Add("Berries", berriesOffer);
        mOfferedByNPC.Add("Fish", fishOffer);

        resourceText.text =  woodOffer.ToString() + "\n\n\n";
        resourceText.text += adhesiveOffer.ToString() + "\n\n\n";
        resourceText.text += fabricOffer.ToString() + "\n\n\n";
        resourceText.text += berriesOffer.ToString() + "\n\n\n";
        resourceText.text += fishOffer.ToString() + "\n\n\n";

    }

}
