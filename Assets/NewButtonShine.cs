using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewButtonShine : MonoBehaviour
{

    //public float minDelay, maxDelay;
    //public GameObject shine;

    // Start is called before the first frame update
    void Start()
    {
        animate();      
    }

    void animate() {
       // transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        Sequence sequence = DOTween.Sequence();   
        sequence
            .AppendInterval(0.5f)
            //.Append(transform.DOScale(1f, 1f))
            .Append(transform.DOLocalMoveX(transform.localPosition.x * -1, 1.0f))
            //.AppendInterval(Random.Range(minDelay, maxDelay))
            .AppendCallback(() => {
                transform.DOLocalMoveX(transform.localPosition.x * -1, 0);
                //animate();
            });
    }
}
