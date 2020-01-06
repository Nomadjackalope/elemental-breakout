using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleSelection : MonoBehaviour {

	private int paddleId = 3;

	public bool updatedId;
	

	// Use this for initialization
	void Start () {
	}

	public List<DataTypes.BiomeType> getBranchesPurchased() {
		return MasterPlayerData.instance.getPaddles()[paddleId].branchesPurchased;
	}

	public List<PowerUps> getRuneIds() {
		return MasterPlayerData.instance.getPaddles()[paddleId].runeIds;
	}

	public List<PowerUps> getSkillAvailable() {
		return MasterPlayerData.instance.getPaddles()[paddleId].skillAvailable;
	}

	public void setPaddleId(int id) {
		if(id != paddleId) {
			updatedId = true;
		}
		paddleId = id;
	}
}
