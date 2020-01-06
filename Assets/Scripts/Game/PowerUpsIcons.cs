using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PowerUpState {
	CoolDown,
	Active,
	Available,
}

public class PowerUpsIcons : MonoBehaviour {

	public GameObject PowerUpsPrefab;

	public SkillDataList skillDataList;

	private List<SkillUI> skills = new List<SkillUI>();

	private Dictionary<string, UnityAction<PowerUps, PowerUpState, float>> listeners = new Dictionary<string, UnityAction<PowerUps, PowerUpState, float>>();


	// Use this for initialization
	void Start () {

		


		
	}

	public IEnumerator popIcons() {
		List<PowerUps> powerUps = MasterPlayerData.instance.getActivePaddle().runeIds;

		foreach (PowerUps item in powerUps)
		{
			Skill skill = skillDataList.getSkillData(item);
			if(skill != null) {
				print(skill.skillName);
				SkillUI ui = Instantiate(PowerUpsPrefab, transform).GetComponent<SkillUI>();
				ui.setSkillData(skill);	
				skills.Add(ui);
				UnityAction<PowerUps, PowerUpState, float> listener = new UnityAction<PowerUps, PowerUpState, float>(updateState);
				listeners.Add(skill.powerUp.ToString(), listener);
				GameManage.StartListening(skill.powerUp.ToString(), listener);
				//StartCoroutine(turnOnOff(item));
			}

			yield return new WaitForSeconds(0.1f);
		}
	}

	void OnEnable() {
		foreach (string key in listeners.Keys)
		{
			GameManage.StartListening(key, listeners[key]);
		}
	}

	void OnDisable() {
		foreach (string key in listeners.Keys)
		{
			GameManage.StopListening(key, listeners[key]);
		}
	}

	// IEnumerator turnOnOff(PowerUps powerUp) {
	// 	PowerUpState originState = PowerUpState.Active;
	// 	while (true) {
	// 		originState = originState == PowerUpState.Active ? PowerUpState.CoolDown : PowerUpState.Active;
	// 		updateState(powerUp, originState, 5f);
	// 		yield return new WaitForSeconds(5f);
	// 	}
	// }

	void updateState(PowerUps powerup, PowerUpState state, float time) {
		skills.Find(x => x.skillData.powerUp == powerup).updateState(state, time);
	}



	// void updateTime(PowerUps powerup, float time) {
	// 	skills.Find(x => x.skillData.powerUp == powerup).updateTime( time);
	// }


}
