using UnityEngine;
using System.Collections;

// camera state
public enum CameraState{
	DebugLook,
	ThirdPerson,
	FirstPerson
}
	
/* Class Author: Alex DS  */
public class PlayerCamera : MonoBehaviour {
	public PlayerCamera(){}
	
	private CameraState mCameraState = CameraState.ThirdPerson;
	private float mScale = 0; // scale value of the camera, copied from player to scale offset variables
	Transform mCameraTransform;
	Vector3 mPosition;
	
	/* Method Author: Alex DS  */
	// initialisation method
	void Start(){
		mCameraTransform = this.gameObject.GetComponent<Transform>(); // save camera transform
		mPosition = mCameraTransform.position;
	}
	
	/* Method Author: Alex DS  */
	// method to keep scale synced to player scale
	private void UpdateScale(){
		mScale = mPlayerTransform.transform.localScale.x;
		mThirdPersonOffSet *= mScale / 2;
		mFirstPersonHeightOffset *= mScale / 2;
	}

	/* Method Author: Alex DS  */	
	// method update looper
	public bool lockedMouse = true;
	public void UpdateCamera(){
		// locked mouse state
		if(lockedMouse){
			Cursor.lockState = CursorLockMode.Locked;
		}else{
			Cursor.lockState = CursorLockMode.Confined;
		}
			
		if( GameInfo.GamePlaying ){ // if game is playing
			mCameraTransform.position = mPosition; // sync transform pos with position variable
			if( mPlayerTransform == null ) // if transform does not exist
				CheckAndSetPlayer(); // check and set player
			else{
				if( mPlayerTransform.localScale.x != mScale ) // use player scale to scale camera offset
					UpdateScale();	// update all offsets relative to scale
			}
			CheckForKey(); // to toggle debuglook
			switch(mCameraState){
				case CameraState.DebugLook:
					CheckKeyboard(); // check for keyboard input
					AddToPosition(mUpdateMove); // move the camera based on keyboard input
					mUpdateMove = new Vector3(); // reset the movement update for this update
				break;
				case CameraState.ThirdPerson:
				case CameraState.FirstPerson:
					UpdateFollow(); // update call
				break;			
			}
		}
	}

	/* Method Author: Alex DS  */	
	// method which saves the player transform
	private Transform mPlayerTransform = null;
	public void CheckAndSetPlayer(){
		GameObject player = GameInfo.GetPlayerObject(); // get player object
		if( player ) // if player is a valid object
			mPlayerTransform = player.GetComponent<Transform>(); // save its transform
	}

	/* Method Author: Alex DS  */	
	// method which handles mouse movements for mouse offsets
	private Vector3 mAngleClamp = new Vector3(0,90,0);
	private Watch ResetDelay = new Watch(); // delay before the camera can be reset to origin by using movement keys
	private void CheckForMouse(){
		if( lockedMouse ){
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");
			mMouseOffset.x +=  x * mMouseSpeed; // add x to the mouseoffset
			mMouseOffset.y +=  y * mMouseSpeed; // add y to the mouseoffset
			if( !( x > 0 || x < 0 ) || !( y > 0 || y < 0 ) ){ // only reset if x and y are 0		
				if( ResetDelay.Done() ){ // only allow reset if the reset delay is done
					if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d") ) // check movement keys
						mMouseOffset = mMouseOffset / 1.1f; // divide to reset camera to origin while moving
				};
			}else
				ResetDelay.Start(1); // reset delay timer			
			ResetDelay.Update(); // update current time
		};
	}

	/* Method Author: Alex DS  */
	// method which does all the updates needed by the camera, only called for third or first person
	private Vector3 mThirdPersonOffSet = new Vector3(0.5f,2,-5); // third person offset
	private Vector3 mMouseOffset = new Vector3(0,0,0); // mouse offset which is applied to offset current perspective rotation
	private float mMouseSpeed = 2; // mouse speed modifier
	private Vector3 mFirstPersonHeightOffset = new Vector3(0,1,0); // first person modifier
	public void UpdateFollow(){
		if( mPlayerTransform != null ){
			mCameraTransform.position = mPlayerTransform.position; // sync player and camera position
			CheckForMouse(); // apply mouse offset
			if(mCameraState == CameraState.ThirdPerson){ // third person only updates
				float desiredAngle = mPlayerTransform.transform.eulerAngles.y; // get y rotation angle
				Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0); // create quaternion rotation
				mCameraTransform.position += (rotation * new Vector3(0,mThirdPersonOffSet.y, mThirdPersonOffSet.z) ); // apply player rotation to camera with third person offset
				
				// apply rotation
				mCameraTransform.RotateAround(mPlayerTransform.position, Vector3.left, mMouseOffset.y); // apply mouse y offset to rotate around the x point			
				mCameraTransform.RotateAround(mPlayerTransform.position, Vector3.up, mMouseOffset.x); // apply mouse x offset to rotate around the y point
					
				mCameraTransform.LookAt(mPlayerTransform.position + new Vector3(0,mThirdPersonOffSet.y/2, 0));	// lookat player	
			}else{ // first person only updates
				mCameraTransform.position += mFirstPersonHeightOffset; // apply first person height offset
				mCameraTransform.rotation = mPlayerTransform.rotation; // copy character rotation
				
				// apply rotation
				mCameraTransform.Rotate(Vector3.up * mMouseOffset.x);  // apply mouse x offset to rotate around the y point
				mCameraTransform.Rotate(Vector3.left * mMouseOffset.y); // apply mouse y offset to rotate around the x point
			}
		}else
			Debug.Log("Not a valid target to follow");
	}

	/* Method Author: Alex DS  */	
	// method for camera keys, called by all camera states.
	private CameraState mPrevCameraState = CameraState.ThirdPerson;
	public void CheckForKey(){
		// toggle mouse lock state
		if( Input.GetKeyDown("escape") ){ 
			lockedMouse = !lockedMouse;
			Debug.Log(lockedMouse);
			Debug.Log("escape down");
		}
		
		// toggle for debuglook
		if( Input.GetKeyDown("f12") ){
			if( mCameraState != CameraState.DebugLook ){
				mPrevCameraState = mCameraState;
				mCameraState = CameraState.DebugLook;
				GameInfo.DebugCamera = true;
			}else{		
				mCameraState = mPrevCameraState;
				mPrevCameraState = CameraState.DebugLook;
				GameInfo.DebugCamera = false;
			}
		}
		
		// toggle between first and third person
		if( Input.GetKeyDown("f5") && mCameraState != CameraState.DebugLook ){
			mMouseOffset = new Vector3(0,0,0);
			if( mCameraState == CameraState.FirstPerson )
				mCameraState = CameraState.ThirdPerson;
			else
				mCameraState = CameraState.FirstPerson;
		};
	}

	/* Method Author: Alex DS  */
	// method which checks for keyboard input, is only called if debuglook camera state is active
	public void CheckKeyboard(){
		if( Input.GetKey("a") || Input.GetKeyDown("a") ) // sideways movement
			MoveLeft();		
		if( Input.GetKey("d") || Input.GetKeyDown("d") )
			MoveRight();
		
		if( Input.GetKey("w") || Input.GetKeyDown("w") ) // up/downwards movement
			MoveUp();		
		if( Input.GetKey("s") || Input.GetKeyDown("s") )
			MoveDown();
		
		float delta = Input.GetAxis("Mouse ScrollWheel"); // uses unity input manager to determine scrollwheel
		if( delta != 0 ){ // zooming movement
			if( delta>0 )
				ZoomIn();
			if( delta<0 )
				ZoomOut();
		};
	}

	// movement variables
	private const float MOVEMENT_INCREMENT = 0.5f;
	private Vector3 mUpdateMove = new Vector3();
	
	/* Method Author: Alex DS  */
	// method to set position
	private void SetPostion(Vector3 pos){
		mPosition = pos;
	}

	/* Method Author: Alex DS  */	
	// method to add to position
	private void AddToPosition(Vector3 addPos){
		mPosition += addPos;
	}
	
	/* Method Author: Alex DS  */
	public void MoveLeft(){
		mUpdateMove.x -= MOVEMENT_INCREMENT;
	}
	
	/* Method Author: Alex DS  */	
	public void MoveRight(){
		mUpdateMove.x += MOVEMENT_INCREMENT;		
	}
	
	/* Method Author: Alex DS  */	
	public void MoveUp(){
		mUpdateMove.z += MOVEMENT_INCREMENT;
	}
	
	/* Method Author: Alex DS  */	
	public void MoveDown(){
		mUpdateMove.z -= MOVEMENT_INCREMENT;
	}
	
	/* Method Author: Alex DS  */	
	public void ZoomOut(){			
		mUpdateMove.y += MOVEMENT_INCREMENT;
	}
	
	/* Method Author: Alex DS  */	
	public void ZoomIn(){
		mUpdateMove.y -= MOVEMENT_INCREMENT;
	}

    //Method Author: Albert Dulian
    public bool IsLocked()
    {
        return lockedMouse;
    }
}
