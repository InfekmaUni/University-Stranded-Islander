using UnityEngine;
using System.Collections;

//Class Author: Callum Milner
//Manages transparency of the boat and triggers the game over
public class BoatScript : MonoBehaviour {

    //Variables for total amount of resourcces required to complete the boat
    private int woodRequired = 210;
    private int adhesiveRequired = 50;
    private int fabricRequired = 100;

    //Variabled for the current amount of each boat resource possessed
    public int currentWood = 0;
    public int currentAdhesive = 0;
    public int currentFabric = 0;

    //Has an owner assigned so that the current amounts can be based on the player's resources and for triggering game over
    public GameObject owner = null;

    //Starting transparency as 0.1 so it's not invisible to begin with
    public float transparency = 0.1f;

    //Color used for setting the transparency
    private Color color;

    // Method Author: Callum Milner
    //Changes material settings so that the boat doesn't appear glitched. This is necesssary when using transparency for the boat
    void Start () {
        this.gameObject.GetComponent<Renderer>().material.SetInt("_ZWrite", 1); //Fixes rendering issues with transparency
        color = this.gameObject.GetComponent<Renderer>().material.color; //Gets the boat color
        color.a = transparency;
        this.gameObject.GetComponent<Renderer>().material.color = color; //Sets the transparency
	}
	
	//Method Author: Callum Milner
    //Used for setting the transparency based on the number of resources the player has
	void Update () {
	    if(owner != null)
        {
        }
	}
	
	/* Original: Callum, Modified: Alex DS*/
	// gets called from playerResource if interacted with
	public void BuildBoat(GameObject player){
		if( player == owner ){
			PlayerResources playerResource = owner.GetComponent<PlayerResources>();
			if( playerResource != null ){
				//Clamping the amounts so that the player having over the amount of one required resource wont affect the transparency
				currentWood = Mathf.Clamp(playerResource.GetNumberOfWood, 0, woodRequired);
				currentAdhesive = Mathf.Clamp(playerResource.GetNumberOfAdhesives, 0, adhesiveRequired);
				currentFabric = Mathf.Clamp(playerResource.GetNumberOfFabric, 0, fabricRequired);

				if(currentWood == woodRequired && currentAdhesive == adhesiveRequired && currentFabric == fabricRequired)
				{
					GameObject.Find("Game Main Logic").GetComponent<Main>().EndGame(true);
				}

				transparency = (0.9f / 360) * (currentFabric + currentWood + currentAdhesive) + 0.1f; //Means that harvesting resources lessens transparency
				color.a = transparency;
				this.gameObject.GetComponent<Renderer>().material.color = color;
			}			
		}else
			GameObject.Find("Feedback-Text").GetComponent<FeedbackText>().SetText("This boat is not yours!", true);
		
	}
}