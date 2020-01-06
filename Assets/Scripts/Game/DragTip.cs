using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragTip : MonoBehaviour
{

    public CanvasGroup allGroup;
    public CanvasGroup arrowsGroup;
    public CanvasGroup dotGroup;
    // Start is called before the first frame update
    void Start()
    {

        arrowsGroup.alpha = 0;
        dotGroup.alpha = 0;

        Sequence seq = DOTween.Sequence();
        //touch dot
        // seq.Append(dotGroup.DOFade(1, 0.3f))
        //     .Append(dotGroup.DOFade(0, 0.3f))
        //     .Append(dotGroup.DOFade(1, 0.15f))
        //     .Append(dotGroup.DOFade(0, 0.3f))
        //     // move arrows
        //     .Append(arrowsGroup.DOFade(1, 0.2f))
        //     .Append(transform.DOLocalMoveX(100, 1f))
        //     .Append(transform.DOLocalMoveX(-100, 1f))
        //     .Append(transform.DOLocalMoveX(100, 1f))
        //     .Append(transform.DOLocalMoveX(-50, 0.5f))
        //     .Append(arrowsGroup.DOFade(0, 0.2f))
        //     .AppendCallback(() => { print("destroying this"); Destroy(this.gameObject); });

        seq.Append(dotGroup.DOFade(1, 0.3f))
            .Append(dotGroup.DOFade(0, 0.3f))
            .Append(dotGroup.DOFade(1, 0.15f))
            .Append(dotGroup.DOFade(0, 0.3f))
            // move arrows
            .Append(arrowsGroup.DOFade(1, 0.2f))
            .Append(transform.DOLocalMoveX(10, 1f))
            .Append(transform.DOLocalMoveX(-10, 1f))
            .Append(transform.DOLocalMoveX(10, 1f))
            .Append(transform.DOLocalMoveX(-5, 0.5f))
            .Append(arrowsGroup.DOFade(0, 0.2f))
            .AppendCallback(() => { print("destroying this"); Destroy(this.gameObject); });
    }
}