using UnityEngine;
using System.Collections;

// game phase enum
public enum GamePhase{
	Exploration = 0,
	Construction = 1,
	Invasion = 2,
}
	
/* Class Author: Alex DS  */
public class GameManager{
	
	/* Method Author: Alex DS */
	public GameManager(){
	}
	
	/* Method Author: Alex DS */	
	// constructor which starts the watch and initial phase benefit
	public void Start(){
		mGameWatch.Start(GameInfo.GamePhaseInterval); // start phase watch
		PhaseAction();		
	}

	public static GamePhase mCurGamePhase = GamePhase.Exploration;
	public static Watch mGameWatch = new Watch();

	/* Method Author: Alex DS */
	// Update is called once per frame
	public void Update() {
		if( GameInfo.GamePlaying ){ // if game is playing
			PhaseHandler(); // call phase handler
		}
	}
	
	/* Method Author: Alex DS */
	// handles the phase transitions using a timer
	private void PhaseHandler(){
		if( mGameWatch.Done() ){ // if watch is done
			if( (int)mCurGamePhase < 2 )// if less then 2(upper limit of enum)
				mCurGamePhase += 1; // increment to next state
			else // otherwise current phase is the last phase possible
				mCurGamePhase = 0; // reset to first phase
			
			Debug.Log("Phase cycling: "+mCurGamePhase);
			PhaseAction(); // perform phase action relative to the current phase
			mGameWatch.Start(GameInfo.GamePhaseInterval); // re-start the watch
		}
		mGameWatch.Update(); // updates watch
	}

    float SpeedBonus = 25; //<--- as %
	/* Method Author: Alex DS */
	// method called to enable/disable phase-specific properties.
	private void PhaseAction(){
		switch( mCurGamePhase ){
			case GamePhase.Exploration:
                // set player movement speed
                // enable/disable invasion
                {
                    PlayerManager player = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
                    player.SetSpeedBonus(SpeedBonus);                    
                }				
			break;
			case GamePhase.Construction:
				// set player construction cost reduction
				// disable invasion
			break;
			case GamePhase.Invasion:
				// remove player construction cost reduction
				// enable invasion
			break;				
		}		
	}
}