using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DOTweenManager : MonoBehaviour
{
    public enum MoveType
    {
        //None,
        LocalMove,Move, 
        Rotate, LocalRotate,
        Scale,
        // Color, Fade,
        // Text,
        CanvasGroup
    }
    [System.Serializable]
    public struct TweenManagerInfo
    {
        public DOTweenManager.MoveType moveType;
        public float delayTime;
        public float duration;
        public Transform targetTrans;
        public Vector3 offset;
        public float targetNum;
        public DG.Tweening.Ease easeType;
    }

    public List<TweenManagerInfo> tweenInfoList;

    
    
    private void Start()
    {
        for (int i = 0; i < tweenInfoList.Count; i++)
        {
            BehaveTween(tweenInfoList[i]);
        }
    }
    
    private void OnEnable()
    {
        for (int i = 0; i < tweenInfoList.Count; i++)
        {
            BehaveTween(tweenInfoList[i]);
        }
    }

    void BehaveTween(TweenManagerInfo info)
    {
        if (info.targetTrans == null)
        {
            info.targetTrans = this.transform;
        }
        if (info.moveType == MoveType.LocalMove)
        {
            this.transform.DOLocalMove(info.targetTrans.localPosition + info.offset, info.duration)
                .SetDelay(info.delayTime).SetEase(info.easeType);
        }
        else if (info.moveType == MoveType.Move)
        {
            this.transform.DOMove(info.targetTrans.position + info.offset, info.duration)
                .SetDelay(info.delayTime).SetEase(info.easeType);
        }
        else if (info.moveType == MoveType.Rotate)
        {
            this.transform.DORotate(info.targetTrans.rotation.eulerAngles + info.offset, info.duration)
                .SetDelay(info.delayTime).SetEase(info.easeType);
        }
        else if (info.moveType == MoveType.LocalRotate)
        {
            this.transform.DOLocalRotate(info.targetTrans.localRotation.eulerAngles + info.offset, info.duration)
                .SetDelay(info.delayTime).SetEase(info.easeType);
        }
        else if (info.moveType == MoveType.Scale)
        {
            this.transform.DOScale(info.targetTrans.localScale + info.offset, info.duration)
                .SetDelay(info.delayTime).SetEase(info.easeType);
        }
        else if (info.moveType == MoveType.CanvasGroup)
        {
            CanvasGroup group = this.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = this.gameObject.AddComponent<CanvasGroup>();
            }
            group.DOFade(info.targetNum, info.duration).SetDelay(info.delayTime).SetEase(info.easeType);
        }
        
        
    }
    // private void OnEnable()
    // {
    //     
    // }
    //
    // private void Update()
    // {
    //     
    //     
    //     
    // }

}
