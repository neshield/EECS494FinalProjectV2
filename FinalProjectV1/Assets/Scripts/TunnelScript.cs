using UnityEngine;
using System.Collections;

public enum TunnelType {vertical, horizontal};

public class TunnelScript : MonoBehaviour {
	static bool beenThroughTunnel;
	public TunnelScript pairedTunnel;
	public Vector3 spawnLocation;
	public TunnelType tType;

	// Use this for initialization
	void Start () {
		beenThroughTunnel = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		PlayerObject pon = other.GetComponent<PlayerObject> ();
		BulletScript bs = other.GetComponent<BulletScript> ();

		if(!pon && !bs){
			return;
		}

		Vector3 pos = other.transform.position;
		if(!beenThroughTunnel){
			//then the y matters where the player goes through
			if(pairedTunnel.tType == TunnelType.horizontal){
				pairedTunnel.setSpawnLocation(
					new Vector3(pairedTunnel.spawnLocation.x, 
				            	//pon.transform.position.y, 
				            	pos.y,
				            	pairedTunnel.spawnLocation.z)
					);
			}
			//if it's a vertical then the x direction matters
			else{
				float leftSide = this.transform.position.x - (this.transform.localScale.x / 2);
				//float diff = pon.transform.position.x - Mathf.Abs(leftSide);
				float diff = pos.x - Mathf.Abs(leftSide);
				float otherLeft = pairedTunnel.transform.position.x - (pairedTunnel.transform.localScale.x / 2);

				float newX = otherLeft + diff;


				pairedTunnel.setSpawnLocation(
					new Vector3(newX, 
				            pairedTunnel.spawnLocation.y, 
				            pairedTunnel.spawnLocation.z)
					);
			}

			//pon.transform.position = pairedTunnel.spawnLocation;
			other.transform.position = pairedTunnel.spawnLocation;
			beenThroughTunnel = true;
		}
		else{
			beenThroughTunnel = false;
		}



	}

	void setSpawnLocation(Vector3 location_){
		spawnLocation = location_;
	}
}
