using System;
using UnityEngine;

/* Class Author: Alex DS */
public class Watch
{
	public Watch(){}
	// variables
	float STOP_TIME = -1; // time at which to stop the watch, this is defined by the Start method
	float mLastTime, mCurrentTime; // time variables
	
	/* Method Author: Alex DS */
	// method which is used primarly to check if the time set on start(time) has been surpassed by the current time
	public bool Done(){
		return (STOP_TIME - GetElapsedSeconds() <= 0);
	}
 
 	/* Method Author: Alex DS */
	// updates the current time relative to game speed
	public void Update(){
		mCurrentTime += (Time.deltaTime * GameInfo.GameSpeed);
	}
	
	/* Method Author: Alex DS */
	public float TimeRemaining(){
		return STOP_TIME - GetElapsedSeconds();
	}
	
	/* Method Author: Alex DS */
	// method to check whether the stop_time was defined
	// default: -1 = not init
	public bool isInit(){ return STOP_TIME >= 0; }
	
	/* Method Author: Alex DS */
	// method which returns the total amount of seconds that have elapsed
	public float GetElapsedSeconds(){
		return mCurrentTime - mLastTime;
	}
	
	/* Method Author: Alex DS */
	// method which starts the watch and stops based on specified time
	public void Start(float stop_time){
		STOP_TIME = Time.deltaTime + stop_time;
		Start();
	}
	
	/* Method Author: Alex DS */
	// sets initial time variables to the method call
	public void Start(){
		mLastTime = Time.deltaTime;
		mCurrentTime = Time.deltaTime;
	}
}