using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	public static Manager m;
	public bool canRun;
	List<playerGUI> guiObjs;

	struct playerGUI{
		public GUIText lives;
	};

	void Awake(){
		m = this;
		canRun = false;

		guiObjs = new List<playerGUI> ();
		createGUIObjects ();
	}

	public void setPlayerLives(int playerNum_, int lives_){
		guiObjs[playerNum_].lives.text = "P" + (playerNum_ + 1) + " Lives: " + lives_;

		//player died
		if(lives_ <= 0){
			int loadedLevel = Application.loadedLevel;

			//loop around
			if((loadedLevel + 1) >= Application.levelCount){
				Application.LoadLevel(0);
			}
			else{
				Application.LoadLevel(loadedLevel + 1);
			}
		}
	}

	void createGUIObjects(){
		playerGUI p1 = new playerGUI();
		playerGUI p2 = new playerGUI();
		playerGUI p3 = new playerGUI();
		playerGUI p4 = new playerGUI();

		Vector3 pos = new Vector3 (0, 0, 0);

		//make the four player lives gui texts
		GameObject obj = new GameObject ("P1LivesText");
		obj.AddComponent<GUIText> ();
		obj.guiText.text = "P1 Lives: 3";
		pos.x = 0.01f;
		pos.y = 0.99f;
		obj.transform.position = pos;
		p1.lives = obj.GetComponent<GUIText>();

		obj = new GameObject ("P2LivesText");
		obj.AddComponent<GUIText> ();
		obj.guiText.text = "P2 Lives: 3";
		pos.x = 0.90f;
		pos.y = 0.99f;
		obj.transform.position = pos;
		p2.lives = obj.GetComponent<GUIText>();


		obj = new GameObject ("P3LivesText");
		obj.AddComponent<GUIText> ();
		obj.guiText.text = "P3 Lives: 3";
		pos.x = 0.01f;
		pos.y = 0.05f;
		obj.transform.position = pos;
		p3.lives = obj.GetComponent<GUIText>();

		obj = new GameObject ("P4LivesText");
		obj.AddComponent<GUIText> ();
		obj.guiText.text = "P4 Lives: 3";
		pos.x = 0.90f;
		pos.y = 0.05f;
		obj.transform.position = pos;
		p4.lives = obj.GetComponent<GUIText>();


		guiObjs.Add (p1);
		guiObjs.Add (p2);
		guiObjs.Add (p3);
		guiObjs.Add (p4);
	}

	//make a function to hide a specific players GUI.  can be called from
	//where the players dissapear if they're not plugged in.
	public void removePlayerGUIs(int playerNum_){

		Destroy (guiObjs [playerNum_].lives);
		guiObjs.RemoveAt (playerNum_);
	}


	IEnumerator show321Yield(){
		Vector3 pos = new Vector3(0,0,0);
		GameObject obj = new GameObject ("321");
		obj.AddComponent<GUIText> ();
		obj.guiText.text = "3";
		obj.guiText.fontSize = 24;
		pos.x = 0.50f;
		pos.y = 0.50f;
		obj.transform.position = pos;
		yield return new WaitForSeconds(1);

		obj.guiText.text = "2";
		yield return new WaitForSeconds(1);
	
		obj.guiText.text = "1";
		yield return new WaitForSeconds(1);

		canRun = true;
		Destroy (obj);
	}

	// Use this for initialization
	void Start () {
		//This does it in the background but does not
		//stop the stream of execution.
		StartCoroutine (show321Yield());
	}


	// Update is called once per frame
	void Update () {
	
	}
}
