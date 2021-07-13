using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PreCabinUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Text DocumentText;
    [HideInInspector]
    public CabinScene cabinScene;
    public Button ClickButton;
    void Start()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        DocumentText.text = Config.WelcomeText;
        cg.alpha = 0;
        cg.DOFade(1,8);
        ClickButton.onClick.AddListener(()=>{
            cabinScene.LoadUI();
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
