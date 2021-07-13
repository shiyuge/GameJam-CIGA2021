using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QContent : MonoBehaviour
{
    [HideInInspector]
    public RectTransform rect;
    
    [HideInInspector]
    public QScroll qScroll;
    public GameObject qCellEntity;
    public QEntityPool pool;
    public QCell[] qCells;
    public delegate void UpdateChildrenCallbackDelegate(int index, Transform trans);
    public UpdateChildrenCallbackDelegate updateChildrenCallback = null;
    public delegate void InitDelegate();
    public InitDelegate init;
    bool inited = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetScrollAmount(Vector2 data)
    {
        if (!inited)
        {
            InitDefault();
            inited = true;
        }

        if (!qScroll) {return;}

        if (rect.sizeDelta.x <=  qScroll.rect.rect.width) {return;}
        Vector2 pos = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(Mathf.Lerp(0, qScroll.rect.rect.width - rect.sizeDelta.x, data.x), pos.y);

        UpdateCells();
    }

    void UpdateCells()
    {
        if (!inited)
        {
            InitDefault();
            inited = true;
        }

        if (qCells != null)
        {
            foreach (QCell qCell in qCells)
            {
                qCell.OnScroll(-rect.anchoredPosition.x);
            }
        }
    }

    void InitDefault()
    {
        string[] defaultText = {"需","要","设","置","初","使","化","函","数"};
        qCellEntity = Resources.Load<GameObject>("Prefabs/QCellEntity");
        qCells = new QCell[defaultText.Length];
        pool = new QEntityPool(qCellEntity);
        for (int i = 0; i < defaultText.Length; i++)
        {
            qCells[i] = new QCell(this, i, new Vector2(i * 100, 0), i *100 - 50, i * 100 + 50);
        }
        rect.sizeDelta = new Vector2(900, 200);

        updateChildrenCallback = (index, transform) => {
            transform.GetChild(0).GetComponent<Text>().text = index.ToString();
        };
    }

    public void SetAmount(int amount)
    {
        if (!inited)
        {
            InitDefault();
            inited = true;
        }
    }
}
