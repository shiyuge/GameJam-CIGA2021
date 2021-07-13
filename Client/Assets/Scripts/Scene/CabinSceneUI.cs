using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CabinSceneUI : MonoBehaviour
{
    public Text MoneyGain;
    public Text FoodGain;
    public Text MoneyConsume;
    public Text FoodConsume;
    public Text MoneyTotal;
    public Text FoodTotal;
    public Image MerchantSatFill;
    public Image NobelSatFill;
    public Image WorkerSatFill;
    public Image CivilSatFill;
    public Image MerchantSatChange;
    public Image NobelSatChange;
    public Image WorkerSatChange;
    public Image CivilSatChange;
    public Text OptionTipsText;
    public Text ProgressText;
    public QuestionCardUI questionCardUI;
    [HideInInspector]
    public CabinScene cabinScene;
    Vector3 defaultTipsPosition;
    // Start is called before the first frame update
    void Start()
    {
        defaultTipsPosition = OptionTipsText.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateResources()
    {
        FoodGain.text = GameState.Instance.FoodProductionPerRound.ToString();
        MoneyGain.text = GameState.Instance.MoneyProductionPerRound.ToString();
        FoodConsume.text = GameState.Instance.FoodBaseConsumptionPerRound.ToString();
        MoneyConsume.text = GameState.Instance.MoneyConsumptionPerRound.ToString();
        MoneyTotal.text = GameState.Instance.Money.ToString();
        FoodTotal.text = GameState.Instance.Food.ToString();

        MerchantSatFill.fillAmount = (float)GameState.Instance.BusinessmanSatisfaction / 100;
        NobelSatFill.fillAmount = (float)GameState.Instance.AristocratSatisfaction / 100;
        WorkerSatFill.fillAmount = (float)GameState.Instance.WorkerSatisfaction / 100;
        CivilSatFill.fillAmount = (float)GameState.Instance.CivilianSatisfaction / 100;

        SetArrowSprite(MerchantSatChange, GameState.Instance.BusinessmanSatisfactionTrend);
        SetArrowSprite(NobelSatChange, GameState.Instance.AristocratSatisfactionTrend);
        SetArrowSprite(WorkerSatChange, GameState.Instance.WorkerSatisfactionTrend);
        SetArrowSprite(CivilSatChange, GameState.Instance.CivilianSatisfactionTrend);

        ProgressText.text = GameState.Instance.Progress.ToString();
    }

    public void PreNextRoundDisplay(int optionIndex)
    {
        DOTween.Kill(OptionTipsText);
        OptionTipsText.text = GameState.Instance.CurrentQuestion().Options[optionIndex].Tip;
        OptionTipsText.gameObject.SetActive(true);
        OptionTipsText.transform.localPosition = defaultTipsPosition;
        OptionTipsText.transform.DOLocalMoveY(defaultTipsPosition.y + 100, 1.5f).OnComplete(()=> {
            OptionTipsText.gameObject.SetActive(false);
        }).SetEase(Ease.InOutSine);
    }
    void SetArrowSprite(Image image, SatisfactionTrend trend)
    {
        if (trend == SatisfactionTrend.Decrease)
        {
            image.gameObject.SetActive(true);
            image.sprite = Resources.Load<Sprite>("UI/greenArrow");
        }
        else if (trend == SatisfactionTrend.Increase)
        {
            image.gameObject.SetActive(true);
            image.sprite = Resources.Load<Sprite>("UI/redArrow");
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }

}
