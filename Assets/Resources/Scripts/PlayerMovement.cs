using UnityEngine;
using System.Collections;

/// <summary>
/// Class Author: Albert Dulian
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private PlayerManager mPlayerManager;
    private Rigidbody mPlayerRigidBody;
    private Transform mPlayerTransform;
    private Vector3 mMovement;
	public bool IsRooted = false; // called by territory to stop character from being able to move

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Awake()
    {
        mPlayerRigidBody = GetComponent<Rigidbody>();
        mPlayerTransform = GetComponent<Transform>();
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Start ()
    {
        mPlayerManager = GetComponentInParent<PlayerManager>();        
    }

    //------------------------------------------------------------
	//Method Author: Albert Dulian
    //Edited by: Callum Milner
    //Edited to increase the player's speed if they have sturdy boots
    void FixedUpdate()
    {
        if (mPlayerManager.IsPlayerDead) // set player movement speed to half if dead
        {
            if (!mPlayerManager.GetPlayerResourcesInfo.mSturdyBoots)
                mPlayerManager.mPlayerVelocity = 10;
            else
                mPlayerManager.mPlayerVelocity = 12;
        }
        else
        {           
            if (!mPlayerManager.GetPlayerResourcesInfo.mSturdyBoots)
                mPlayerManager.mPlayerVelocity = 20;
            else
                mPlayerManager.mPlayerVelocity = 24;
        }

        //UpdateMovement
        if (GameInfo.GamePlaying && !mPlayerManager.mTradingSystem.mIsTrading && !IsRooted)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            Move(horizontalAxis, verticalAxis);
        }
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Update ()
    {
	
	}

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Move(float h, float v)
    {

        if (h > 0 || h < 0)
        { // rotate 
            float mRotation = h * mPlayerManager.GerRotationSpeed;
            mPlayerTransform.Rotate(Vector3.up * mRotation); // rotate transform changing its forward vector
        }
        if (v > 0 || v < 0)
        { // move forward

            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit rayhit;
            LayerMask layer = 1 << LayerMask.NameToLayer("Water");

            if (Physics.Raycast(ray, out rayhit, layer))
            {
                mMovement = transform.forward; // movement is a forward direction movement
                mMovement = mMovement.normalized * mPlayerManager.GetPlayerVelocity * Time.deltaTime;

                if (v < 0) // invert movement
                    mMovement *= -1;

                Vector3 originalPos = transform.position;
                originalPos.y =  rayhit.point.y;

                mPlayerRigidBody.MovePosition(originalPos + mMovement);
            }           
        }        
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public Vector3 GetPlayerPosition
    {
        get { return mPlayerRigidBody.position; }
    }   
}
