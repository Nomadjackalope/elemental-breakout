using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillUI : FlexibleUI {

    Text countDown;
    Image icon;

    Coroutine updatingCount;
    PowerUpState currentState;

    void Start() {
        // watchHitsCounter
    }


    protected override void OnSkinUI() {
        base.OnSkinUI();

        foreach (Transform t in transform)
        {
            if(t.name == "Icon") {
                icon = t.GetComponent<Image>();
            } else if (t.name == "countDown") {
                countDown = t.GetComponent<Text>();
            }

        }

        if(icon != null && skillData != null) {
            Vector3 prevScale = icon.transform.localScale;
            icon.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            icon.color = new Color(1,1,1,0);
            icon.sprite = skillData.icon;
            Sequence popSprite = DOTween.Sequence();
            popSprite.AppendInterval(0.5f)
                .Append(icon.DOFade(1, 0.1f))
                .Join(icon.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutBounce));
        }
    }

    public void setSkillData(Skill data) {
        skillData = data;
        OnSkinUI();
    }

    public void updateState(PowerUpState state, float count) {
        if(state == PowerUpState.Active) {
            icon.color = Color.white;
        } else if(state == PowerUpState.CoolDown) {
            icon.color = new Color(0.8f, 0.8f, 0.8f);
        } else if(state == PowerUpState.Available) {
            icon.color = Color.green;
        }

        if(count >= 0) {
            if(state == PowerUpState.CoolDown) {
                if(skillData.whileCoolDown == CoolDownType.TimeBased) {
                    StartCoroutine(updateTime(count));
                } else if(skillData.whileCoolDown == CoolDownType.XHits) {
                    countDown.text = count.ToString("0");
                } else {
                    countDown.text = "";
                }
            } else if(state == PowerUpState.Active) {
                if(skillData.whileActive == CoolDownType.TimeBased) {
                    StartCoroutine(updateTime(count));                    
                } else if(skillData.whileActive == CoolDownType.XHits) {
                    countDown.text = count.ToString("0");
                } else {
                    countDown.text = "";
                }
            }
            
        }

    }

    // public void updateState(PowerUpState state, float count) {
    //     if(state == currentState) return;

    //     if(updatingCount != null) {
    //         StopCoroutine(updatingCount);
    //     }

    //     if(state == PowerUpState.Active) {
    //         icon.color = Color.green;

    //         if( isTime && count >= 0) {
    //             StartCoroutine(updateTime(count));
    //         } else if(!isTime) {
                
    //         }

            
    //     } else if(state == PowerUpState.CoolDown) {
    //         icon.color = Color.black;

    //         if(isTime) {
    //             StartCoroutine(updateTime(count));            
    //         }
    //     }
    // }

    // public void updateCount(PowerUpState state, float count) {

    // }

    IEnumerator updateTime(float time) {
        
        for (float i = time; i >= 0; i -= Time.deltaTime)
        {
            if(i > 1) {
                countDown.text = i.ToString("0");
            } else {
                countDown.text = i.ToString("0.00");
            }
            yield return null; 
        }

        countDown.text = "";
    }


}