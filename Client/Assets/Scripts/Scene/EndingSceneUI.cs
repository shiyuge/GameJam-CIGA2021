using UnityEngine;
using UnityEngine.UI;

public class EndingSceneUI : MonoBehaviour
{
    [HideInInspector]
    public EndingScene endingScene;

    public Image ImageBg;
    public Text Round;
    
    public Text Reason;
    
    public Text Money;
    public Text Food;
    public Text Progress;

    public Text Civilian;
    public Text Worker;
    public Text Aristocrat;
    public Text Businessman;

    public Button ResetButton;
    
    void Start()
    {
        Round.text = GameState.Instance.Round.ToString();
        
        Ending ending = GameState.Instance.GameEnding;
        Reason.text = Config.Localization[ending];

        Money.text = GameState.Instance.Money.ToString();
        Food.text = GameState.Instance.Food.ToString();
        Progress.text = GameState.Instance.Progress.ToString();

        Civilian.text = GameState.Instance.CivilianSatisfaction.ToString();
        Worker.text = GameState.Instance.WorkerSatisfaction.ToString();
        Aristocrat.text = GameState.Instance.AristocratSatisfaction.ToString();
        Businessman.text = GameState.Instance.BusinessmanSatisfaction.ToString();
        
        ResetButton.onClick.AddListener(endingScene.ResetGame);

        if (GameState.Instance.Progress == 0)
        {
            ImageBg.sprite = Resources.Load<Sprite>("UI/1");
        }
        else if (GameState.Instance.Progress < 25)
        {
            ImageBg.sprite = Resources.Load<Sprite>("UI/1");
        }
        else if (GameState.Instance.Progress < 50)
        {
            ImageBg.sprite = Resources.Load<Sprite>("UI/2");
        }
        else if (GameState.Instance.Progress < 75)
        {
            ImageBg.sprite = Resources.Load<Sprite>("UI/3");
        }
        else if (GameState.Instance.Progress < 100)
        {
            ImageBg.sprite = Resources.Load<Sprite>("UI/4");
        }
        else
        {
            ImageBg.sprite = Resources.Load<Sprite>("UI/5");
        }
    }
}
