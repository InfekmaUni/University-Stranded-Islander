using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

/* Author: Alex DS */
public struct LoggerTradeInfo{
	public int accepted;
	public int declined;
	public int stolen;
	
	public int totalSuccesfullTrade;
	public int totalUnsuccesfullTrade;
	public void Init(){
		accepted = 0;
		declined = 0;
		stolen = 0;
		PlayerSuccesfullTrade  = new List<Dictionary<string,int>>();
		AiSuccesfullTrade = new List<Dictionary<string,int>>();
		
		PlayerUnSuccesfullTrade = new List<Dictionary<string,int>>();
		AiUnSuccesfullTrade = new List<Dictionary<string,int>>();
	
		PlayerStolenTrade = new List<Dictionary<string,int>>();
		AIStolenTrade = new List<Dictionary<string,int>>();

		PlayerUnsuccesfullStolenTrade = new List<Dictionary<string,int>>();
		AiUnsuccesfullStolenTrade = new List<Dictionary<string,int>>();
	}
	public List<Dictionary<string,int>> PlayerSuccesfullTrade;
	public List<Dictionary<string,int>> AiSuccesfullTrade;
		
	public List<Dictionary<string,int>> PlayerUnSuccesfullTrade;
	public List<Dictionary<string,int>> AiUnSuccesfullTrade;
	
	public List<Dictionary<string,int>> PlayerStolenTrade;
	public List<Dictionary<string,int>> AIStolenTrade;
	
	public List<Dictionary<string,int>> PlayerUnsuccesfullStolenTrade;
	public List<Dictionary<string,int>> AiUnsuccesfullStolenTrade;
}
	
/* Class Author: Alex DS */
public class Logger : MonoBehaviour {
	private string fileName = "StrandedIslander";
	private string extension = ".txt";
	private int fileID = 0;
	private float gameDuration = 0;
	public LoggerTradeInfo mLoggerTrade = new LoggerTradeInfo();
	
	// Use this for initialization
	void Start () {
		mLoggerTrade.Init();
	}
	/* Author: Alex DS */
	void OnApplicationQuit(){
		LogToFile();
	}
	
	/* Author: Alex DS */
	public void LogToFile(){
		while(true){ // while a file has not been found
			string file =fileName+"-"+fileID+extension;
	        if (File.Exists(file)) // if file exist, increment ID
				fileID++;
			else
				break;
		}
		
		string foundFile = fileName+"-"+fileID+extension;
		var sr = File.CreateText(foundFile); // create file
		sr.WriteLine ("############Stranded Islander Data Collection Logger############");
		sr.WriteLine ("################################################################");
		sr.WriteLine ();
		sr.WriteLine ("Date: "+DateTime.Now); // current date and time
        sr.WriteLine ("Duration: "+(int)gameDuration+"s"); // game length
		
		sr.WriteLine ();
		sr.WriteLine ("##Player Data Summary##");
		// summary of raw data
		sr.WriteLine ("Amount of times player in Trade: |Accepted: "+mLoggerTrade.accepted+"| Declined: "+mLoggerTrade.declined+"| Stole:"+mLoggerTrade.stolen+"|");
		sr.WriteLine ("Amount of |successful trades: "+mLoggerTrade.totalSuccesfullTrade+"| unsuccessful trades: "+mLoggerTrade.totalUnsuccesfullTrade+"|");
		
		sr.WriteLine();
		sr.WriteLine ("###RAW TRADING DATA###");

		#region accept trade
		// accepted trades
		sr.WriteLine("Succesfull Trades");
		int i = 0; // reset counter 
		foreach (var dictionary in mLoggerTrade.PlayerSuccesfullTrade){ // for all dictionaries in succesfull trade
			sr.Write("Player offered: ");
			foreach (var keyValue in dictionary){ // for each resource
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value)); // write resource on same lie
			}
			sr.WriteLine(); 
			sr.Write("AI offered:     ");
			foreach (var keyValue in mLoggerTrade.AiSuccesfullTrade[i]){ // for all resources that the AI offered
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value)); // write resources on same line
			}
			sr.WriteLine(); 
			
			sr.WriteLine();
			i++;
		}
		sr.WriteLine();	
	#endregion
	
		#region declined trade
		// declined trades
		sr.WriteLine("Declined Trades");
		i = 0; // reset counter
		foreach (var dictionary in mLoggerTrade.PlayerUnSuccesfullTrade){ // for all dictionaries containing resources
			sr.Write("Player offered: ");
			foreach (var keyValue in dictionary){ // for all resources
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value)); // write on same line
			}
			sr.WriteLine();
			sr.Write("AI offered:     ");
			foreach (var keyValue in mLoggerTrade.AiUnSuccesfullTrade[i]){// for all resources
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value));// write on same line
			}
			sr.WriteLine();
			
			sr.WriteLine();
			i++; // increment counter for next dictionary trade
		}
		sr.WriteLine();	
		#endregion
		
		#region Stolen Trades
		// stolen trades
		sr.WriteLine("Stolen Trades");
		i = 0; // reset counter
		foreach (var dictionary in mLoggerTrade.PlayerStolenTrade){ // for all stolen trades
			sr.Write("Player offered: ");
			foreach (var keyValue in dictionary){ // for all player resources
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value)); // write to line
			}
			sr.WriteLine();
			sr.Write("AI offered:     ");
			foreach (var keyValue in mLoggerTrade.AIStolenTrade[i]){ // for all ai resource offers
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value)); // write to line
			}
			sr.WriteLine();
			
			sr.WriteLine();
			i++;
		}
		sr.WriteLine();	
		#endregion
		
		#region both parties stole from eachother
		// unSuccesfullStolenTrade
		sr.WriteLine("Unsuccesfull Stolen Trade, this is where both parties stole from eachother");
		i = 0;
		foreach (var dictionary in mLoggerTrade.PlayerUnsuccesfullStolenTrade){ // for all trades
			sr.Write("Player offered: ");
			foreach (var keyValue in dictionary){ // for all player resources
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value)); // write to line
			}
			sr.WriteLine();
			sr.Write("AI offered:     ");
			foreach (var keyValue in mLoggerTrade.AiUnsuccesfullStolenTrade[i]){ // for all ai resources
				sr.Write(string.Format("| {0} {1} | ", keyValue.Key, keyValue.Value)); // write to line
			}
			sr.WriteLine();
			
			sr.WriteLine();
			i++;
		}
		sr.WriteLine();	
		#endregion
		
        sr.Close();	
	}
	
	/* Author: Alex DS */
	// Update is called once per frame
	void Update () {
		gameDuration += Time.deltaTime;
	}
}
