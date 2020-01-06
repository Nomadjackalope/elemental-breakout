using UnityEngine;
using UnityEngine.UI;

// [RequireComponent(typeof(Image))]
// [RequireComponent(typeof(Toggle))]
// // public class ToggleSkill : FlexibleUI {

//     Text skillName;
//     Text skillDescription;
//     Image image;
//     Image greyScaleImage;

//     Toggle toggle;

//     void Start() {
//         toggle = GetComponent<Toggle>();
//         if(Application.isPlaying) {
//             if(!MasterPlayerData.instance.branchEnabled(skillData.biome)
//                 || !MasterPlayerData.instance.checkPowerupUnlocked(skillData.powerUp)) {
//                 toggle.interactable = false;
//             }

//             toggle.onValueChanged.AddListener(delegate {
//                 ToggleValueChanged(toggle);
//             });

//             if(MasterPlayerData.instance.getActivePaddle().runeIds.Contains(skillData.powerUp)) {
//                 toggle.isOn = true;
//                 greyScaleImage.enabled = false;
//             }
//         }
//     }

//     void Update() {
//         if(Application.isPlaying) {
//             bool currentState = MasterPlayerData.instance.branchEnabled(skillData.biome)
//                 && MasterPlayerData.instance.checkPowerupUnlocked(skillData.powerUp);
//             if(currentState != toggle.interactable) {
//                 toggle.interactable = currentState;
//             }

        
//             if(MasterPlayerData.instance.getActivePaddle().runeIds.Contains(skillData.powerUp)) {
//                 toggle.isOn = true;
//                 greyScaleImage.enabled = false;
//             } else {
//                 toggle.isOn = false;
//                 greyScaleImage.enabled = true;
//             }
            
//         }
//     }

//     // void ToggleValueChanged(Toggle change) {
//     //     SkillTree.instance.OnPowerUpClicked(skillData);
//     //     if(change.isOn && !MasterPlayerData.instance.getActivePaddle().runeIds.Contains(skillData.powerUp)) {
//     //         if(skillData.isAnActive && MasterPlayerData.instance.activePaddleHasActive()) {
//     //             Debug.LogWarning("You have multiple actives enabled");
//     //             //SkillTree.instance.activatePowerUp(skillData.powerUp);
//     //             SkillTree.instance.showActiveCheckPanel(skillData.powerUp); //, enableActive);
//     //         } else {
//     //             SkillTree.instance.activatePowerUp(skillData.powerUp);
//     //             greyScaleImage.enabled = false;
//     //         }
//     //     } else if(!change.isOn && MasterPlayerData.instance.getActivePaddle().runeIds.Contains(skillData.powerUp)) {
//     //         SkillTree.instance.deactivatePowerUp(skillData.powerUp);
//     //         greyScaleImage.enabled = true;
//     //     }
//     // }

//     // void enableActive(PowerUps powerUp) {
//     //     greyScaleImage.enabled = false;
//     // }

//     // public delegate void callback(PowerUps powerUp);

//     protected override void OnSkinUI() {
//         base.OnSkinUI();

//         foreach (Transform t in transform)
//         {
//             if(t.name == "Background") {
//                 image = t.GetComponent<Image>();
//             } else if (t.name == "Greyscale") {
//                 greyScaleImage = t.GetComponent<Image>();
//             }

//         }

//         if(skillDescription != null)
//             skillDescription.text = skillData.description;

//         if(skillName != null)
//             skillName.text = skillData.skillName;

//         if(image != null) {
//             image.sprite = skillData.icon;
//         }

//         if(greyScaleImage != null)
//             greyScaleImage.sprite = skillData.icon;
        
//     }

//     public void OnPowerUpClicked() {
//         SkillTree.instance.OnPowerUpClicked(skillData);
//     }
// }