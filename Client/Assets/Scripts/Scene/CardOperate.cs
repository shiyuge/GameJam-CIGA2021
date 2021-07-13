using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardOperate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;
    public AnimationCurve rCurve;
    float shiftingAmount = 0;
    Vector3 curPos;
    float curRotateZ;

    public Vector3 destPos;
    public float destRotateZ;
    public float maxShift = 0.5f;
    float beginScreenPosX;
    float shiftAmount;

    public QuestionCardUI questionCardUI;
    void Start()
    {
        curPos = transform.localPosition;
        curRotateZ = transform.localEulerAngles.z;
        questionCardUI.OptionText.gameObject.SetActive(false);
        questionCardUI.OoptionShadow.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        beginScreenPosX = data.position.x;
    }

    public void OnDrag(PointerEventData data)
    {
        shiftAmount = (data.position.x - beginScreenPosX) / (Screen.width / 2);
        shiftAmount = Mathf.Max(-maxShift, shiftAmount);
        shiftAmount = Mathf.Min(maxShift, shiftAmount);
        if (shiftAmount > 0)
        {
            transform.localPosition = new Vector3(Mathf.Lerp(curPos.x, destPos.x, xCurve.Evaluate(shiftAmount / maxShift)) , 
            Mathf.Lerp(curPos.y, destPos.y, yCurve.Evaluate(shiftAmount / maxShift)), 0);
            transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(curRotateZ, destRotateZ, rCurve.Evaluate(shiftAmount / maxShift)));
        }
        else
        {
            transform.localPosition = new Vector3(-Mathf.Lerp(curPos.x, destPos.x, xCurve.Evaluate(-shiftAmount / maxShift)) , 
            Mathf.Lerp(curPos.y, destPos.y, yCurve.Evaluate(-shiftAmount / maxShift)), 0);
            transform.localEulerAngles = new Vector3(0, 0, -Mathf.Lerp(curRotateZ, destRotateZ, rCurve.Evaluate(-shiftAmount / maxShift)));
        }

        if (shiftAmount / maxShift < -0.8f)
        {
            questionCardUI.OptionText.gameObject.SetActive(true);
            questionCardUI.OoptionShadow.gameObject.SetActive(true);
            questionCardUI.OptionText.text = GameState.Instance.CurrentQuestion().Options[0].Text;

        }
        else if (shiftAmount / maxShift > 0.8f)
        {
            questionCardUI.OptionText.gameObject.SetActive(true);
            questionCardUI.OoptionShadow.gameObject.SetActive(true);
            questionCardUI.OptionText.text = GameState.Instance.CurrentQuestion().Options[1].Text;
        }
        else
        {
            questionCardUI.OptionText.gameObject.SetActive(false);
            questionCardUI.OoptionShadow.gameObject.SetActive(false);
        }

    }

    public void OnEndDrag(PointerEventData data)
    {
        transform.DOLocalMove(curPos, 0.1f);
        transform.DOLocalRotate(Vector3.zero, 0.1f);
        if (shiftAmount / maxShift < -0.8f)
        {
            questionCardUI.cabinSceneUI.cabinScene.NextRound(0);
        }
        else if (shiftAmount / maxShift > 0.8f)
        {
            questionCardUI.cabinSceneUI.cabinScene.NextRound(1);
        }
        questionCardUI.OptionText.gameObject.SetActive(false);
        questionCardUI.OoptionShadow.gameObject.SetActive(false);
    }
}
