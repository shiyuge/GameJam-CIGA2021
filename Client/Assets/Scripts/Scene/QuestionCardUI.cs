using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestionCardUI : MonoBehaviour
{
    public Text Description;
    public Text OptionText;
    public Image OoptionShadow;
    public Image RedCircle;
    public Question curQuestion;
    public CabinSceneUI cabinSceneUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateQuestion()
    {
        ///刷新数据
        curQuestion = GameState.Instance.CurrentQuestion();
        Description.text = curQuestion.Text;
        RedCircle.GetComponent<RectTransform>().anchoredPosition = new Vector2(62 * (GameState.Instance.Round % 12) - 62 * 7, 0);
        ///表现
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().DOFade(1,0.5f).SetEase(Ease.InOutCubic).SetDelay(1.6f);
    }

    public void PreNextRoundDisplay(int optionIndex)
    {

    }
}
