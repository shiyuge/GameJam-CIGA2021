using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CabinScene : MonoBehaviour
{
    CabinSceneUI cabinSceneUI;
    PreCabinUI preCabinUI;
    Question curQuestion;
    public GameObject SceneObjects;

    public GameObject IndoorMerchant;
    public GameObject IndoorWorker;
    public GameObject IndoorNoel;
    public GameObject IndoorCivil;
    public GameObject Me;
    public GameObject Merchant;
    public GameObject Worker;
    public GameObject Nobel;
    public GameObject Civil;
    // Start is called before the first frame update
    void Start()
    {
        LoadPreUI();
    }

    public void LoadPreUI()
    {
        SceneObjects.SetActive(false);
        preCabinUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PreCabinUI")).GetComponent<PreCabinUI>();
        preCabinUI.transform.parent = CIGAGameLoop.canvas;
        preCabinUI.GetComponent<RectTransform>().sizeDelta = Vector2.one;
        preCabinUI.transform.localPosition = Vector3.zero;
        preCabinUI.cabinScene = this;

    }

    public void LoadUI()
    {
        SceneObjects.SetActive(true);
        cabinSceneUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CabinUI")).GetComponent<CabinSceneUI>();
        cabinSceneUI.transform.parent = CIGAGameLoop.canvas;
        cabinSceneUI.GetComponent<RectTransform>().sizeDelta = Vector2.one;
        cabinSceneUI.transform.localPosition = Vector3.zero;
        cabinSceneUI.cabinScene = this;

        CIGAGameLoop.canbinUI = cabinSceneUI;
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextRound(int optionIndex)
    {
        PreNextRoundDisplay(optionIndex);
        GameState.Instance.NextRound(GameState.Instance.CurrentQuestion().Options[optionIndex]);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        cabinSceneUI.UpdateResources();
        cabinSceneUI.questionCardUI.UpdateQuestion();

        int rand = Random.Range(0,4);

        Me.gameObject.SetActive(rand != 2);
        Merchant.SetActive(rand == 0);
        IndoorMerchant.SetActive(rand == 0);
        Worker.SetActive(rand == 1);
        IndoorWorker.SetActive(rand == 1);
        Nobel.SetActive(rand == 2);
        IndoorNoel.SetActive(rand == 2);
        Civil.SetActive(rand == 3);
        IndoorCivil.SetActive(rand == 3);
    }

    public void PreNextRoundDisplay(int optionIndex)
    {
        cabinSceneUI.questionCardUI.PreNextRoundDisplay(optionIndex);
        cabinSceneUI.PreNextRoundDisplay(optionIndex);
    }
}
