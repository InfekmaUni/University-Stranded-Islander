using UnityEngine;
using System.Collections;

/// <summary>
/// Class Author: Albert Dulian
/// </summary
public class FarmConstructor : MonoBehaviour
{
    public GameObject[] mFarmsObjects; //0 - Berry Farm, 1 - Wood Farm, 2 - Adhesives Farm 
    public GameObject mPlayerCamera;
    public PlayerManager mPlayerManager;
    public Material mInvalidZoneMaterial;
    public Material mValidZoneMaterial;

    private GameObject mFarm;
    private bool mFarmSelected;   
    private bool mIsInNPCTerritory;

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    void Start ()
    {      
        mFarm = null;        
        mFarmSelected = false;
        mIsInNPCTerritory = false;        
	}

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Move and rotate the farm with the player
    void FixedUpdate()
    {
        if(mFarm != null)
        {
            Vector3 moveTo = mPlayerCamera.transform.position + (mPlayerCamera.transform.forward * 15);
            Vector3 rayOrigin = mFarm.transform.position + Vector3.up;

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit rayhit;
            LayerMask layer = 1 << LayerMask.NameToLayer("Water");           

            //Keep the object on the ground
            if (Physics.Raycast(ray, out rayhit, layer))
            {
               moveTo.y = rayhit.point.y;
               mFarm.transform.position = moveTo;
            }
            
            float mRotation = 0f;

            //Set the rotation
            if (!mPlayerCamera.GetComponent<PlayerCamera>().IsLocked()) //Rotate With Player
            {
                float horizontalAxis = Input.GetAxis("Horizontal");
               
                mRotation = horizontalAxis * mPlayerManager.GerRotationSpeed;
            }
            else
            {
                float x = Input.GetAxis("Mouse X"); //Rotate using mouse 
                if (x == 0)
                    x = Input.GetAxis("Horizontal"); //Rotate with Player

                mRotation = x * mPlayerManager.GerRotationSpeed;
            }

            mFarm.transform.Rotate(0,0, mRotation);
        }
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Change materials from green -> red depending if the farm is in
    //NPCs territory
    void Update ()
    {
	    if(mFarm != null)
        {
            if (!mFarmSelected)
            {
                Material[] mats = new Material[2];
                mats[0] = mFarm.GetComponent<Renderer>().materials[0];               

                if (mIsInNPCTerritory) mats[1] = mInvalidZoneMaterial;
                else mats[1] = mValidZoneMaterial;

                mFarm.GetComponent<Renderer>().materials = mats;

                //Place the farm and remove green/red material
                if (Input.GetKeyDown("b"))
                {                    
                    Destroy(mFarm.GetComponent<Rigidbody>());

                    mats = new Material[1];
                    mats[0] = mFarm.GetComponent<Renderer>().materials[0];
                    mFarm.GetComponent<Renderer>().materials = mats;

                    mFarm = null;
                    mFarmSelected = false;
                    mIsInNPCTerritory = false;
                }
            }            
        }
	}

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    //Add the farm as a child to the FarmConstructor object
    public void ChooseFarm(int farm)
    {
        //Option:
        //1 - Berry Farm
        //2 - Wood Farm
        //3 - Adhesive Farm

        if (mFarm != null)
            return;

        if (farm == 1)
            mFarm = mFarmsObjects[0];
        else if (farm == 2)
            mFarm = mFarmsObjects[1];
        else if (farm == 3)
            mFarm = mFarmsObjects[2];
        else
            return;

        Vector3 initialPos = mPlayerManager.GetPlayerTransformInfo.transform.position;
        initialPos.z += 15;
        initialPos.y = 5;

        GameObject child = Instantiate(mFarm, initialPos, mFarm.transform.rotation) as GameObject;
        child.transform.parent = transform;

        mFarm = child;
    }

    //------------------------------------------------------------
    //Method Author: Albert Dulian
    public void IsInNPCTerritory(bool b)
    {
        mIsInNPCTerritory = b;
    }
}
