using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameMenuUI : MonoBehaviour
{
    public Button StartButton;
    public GameMenu gameMenu;
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(()=>{
            gameMenu.EnterCabin();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
