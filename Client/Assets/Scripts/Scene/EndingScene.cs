using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScene : MonoBehaviour
{
    [HideInInspector]
    public EndingSceneUI UI;
    
    void Start()
    {
        UI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/EndingUI")).GetComponent<EndingSceneUI>(); 
        UI.transform.parent = CIGAGameLoop.canvas;
        UI.GetComponent<RectTransform>().sizeDelta = Vector2.one;
        UI.transform.localPosition = Vector3.zero;
        UI.endingScene = this;
    }
    
    public void ResetGame()
    {
        GameState.Instance.Reset();
        SceneManager.LoadScene("Cabin");
        Destroy(UI.gameObject);
    }
}
