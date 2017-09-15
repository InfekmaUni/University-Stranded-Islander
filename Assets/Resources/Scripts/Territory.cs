using UnityEngine;
using System.Collections;

/* Class Author: Alex DS */
public class Territory : MonoBehaviour {

	private SphereCollider mSphere;
	public GameObject mAttachedPlayer;
	public bool mLocalPlayer = false;
	private GameObject mLastCollidedObject;
	private Material[] mMats = new Material[3];
	/* Method Author: Alex DS */
	void Start () {
		mSphere = this.gameObject.GetComponent<SphereCollider>();
	
		mMats[0] =  Resources.Load("Materials/Territory Player Mat") as Material;
		mMats[1] =  Resources.Load("Materials/Territory Valid Mat") as Material;
		mMats[2] = Resources.Load("Materials/Territory InValid Mat") as Material;
	}
	
	/* Method Author : Alex DS */
	void Update () {
		switch(GameManager.mCurGamePhase){
			case GamePhase.Exploration:
				mSphere.enabled = true;
				UpdateTerritoryMat(mLocalPlayer);
			break;
			case GamePhase.Construction:
				mSphere.enabled = true;
				UpdateTerritoryMat(mLocalPlayer);
			break;
			case GamePhase.Invasion:
				mSphere.enabled = false;
				UpdateTerritoryMat( GameManager.mCurGamePhase == GamePhase.Invasion );
				if( mLastCollidedObject != null )
					mLastCollidedObject.gameObject.GetComponent<PlayerMovement>().IsRooted = false; // to ensure when the phase switches over the last collided character gets freed
			break;
		}
	}
	
	/* Method Author: Alex DS */
	private void UpdateTerritoryMat(bool valid){
		if( mLocalPlayer ){ // if player
			this.GetComponent<Renderer>().material = mMats[0];
		}else if( valid ){ // valid depends on phase
			this.GetComponent<Renderer>().material = mMats[1];
		}else{ // invalid
			this.GetComponent<Renderer>().material = mMats[2];
		}
	}
	
	/* Method Author: Alex DS */
	// this methods lifespan is tied to the current enabled state of the sphere which is toggled on or off in the updater
	// this method also imitates a collider component but using the trigger method we are able to control what is and isnt allowed without using layers.
	public void OnTriggerStay(Collider other){
		if( other.gameObject != mAttachedPlayer ){ // if not true, its an player with no access	
			mLastCollidedObject = other.gameObject;	
			// pushes the colliding object normalized direction backwards.
			Vector3 direction = Vector3.Normalize(this.transform.position - other.transform.position);
			other.gameObject.transform.position = new Vector3(	other.gameObject.transform.position.x - direction.x,
																other.gameObject.transform.position.y,
																other.gameObject.transform.position.z - direction.z);
            if(other.name.Contains("Player"))
                other.gameObject.GetComponent<PlayerMovement>().IsRooted = true;
		}
	}
	
	/* Method Author: Alex DS */
	// method which releases player movement state flag when he exits the trigger
	public void OnTriggerExit(Collider other){

        if (other.name.Contains("Player"))
            other.gameObject.GetComponent<PlayerMovement>().IsRooted = false;		
	}
}