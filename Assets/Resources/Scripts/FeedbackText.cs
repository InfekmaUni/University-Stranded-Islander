using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* class author: Alex DS */
public class FeedbackText : MonoBehaviour {

	private Watch mWatch = new Watch();
	public int mDisplayTime = 3;
	private Text mText;
	// Use this for initialization
	void Start () {
		mText = this.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {	
		if( mWatch.Done() ){ // if done, reset the text
			mText.text = "";
		}		
		mWatch.Update();
	}

	/* Method author : Alex DS */
	// this function is called by other components to set its properties
	public void SetText(string message, bool error = false){
		mWatch.Start(mDisplayTime);
		mText.text = message;

		if( error ) // if error
			mText.color = Color.red; // set text color to red
		else
			mText.color = Color.yellow; // set text color to yellow
	}
}
