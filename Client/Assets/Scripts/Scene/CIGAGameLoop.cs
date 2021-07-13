using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CIGAGameLoop : MonoBehaviour
{
    public static Transform canvas;
    public static CabinSceneUI canbinUI;
    // Start is called before the first frame update
    void Start()
    { 
        DontDestroyOnLoad(gameObject);
       canvas = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Canvas")).transform;
       canvas.localPosition = Vector3.zero;
       GameState.Instance.SetGameEndCallback(this.OnGameEnd);
       GameObject.DontDestroyOnLoad(canvas.gameObject);
       SceneManager.LoadScene("GameMenu");
    }

    void OnGameEnd(Ending ending)
    {
        Destroy(canbinUI.gameObject);
        SceneManager.LoadScene("Ending");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
