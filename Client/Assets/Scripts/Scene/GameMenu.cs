using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    GameMenuUI gameMenuUI;
    // Start is called before the first frame update
    void Start()
    {
        gameMenuUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GameMenuUI")).GetComponent<GameMenuUI>();
        gameMenuUI.transform.parent = CIGAGameLoop.canvas;
        gameMenuUI.GetComponent<RectTransform>().sizeDelta = Vector2.one;
        gameMenuUI.transform.localPosition = Vector3.zero;
        gameMenuUI.gameMenu = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterCabin()
    {
        SceneManager.LoadScene("Cabin");
        GameObject.Destroy(gameMenuUI.gameObject);   
    }
}
