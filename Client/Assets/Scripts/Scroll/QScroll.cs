using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class QScroll : MonoBehaviour
{
    [HideInInspector]
    public ScrollRect scrollRect;
    [HideInInspector]
    public RectTransform rect;
    public QContent[] qContents;
    public float maxScroll = 1;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener((data) => { ScrollCallback(data); });
        rect = GetComponent<RectTransform>();

        foreach (QContent zContent in qContents)
        {
            zContent.qScroll = this;
        }
    }


    void ScrollCallback(Vector2 data)
    {
        foreach (QContent zContent in qContents)
        {
            Vector2 clampedScroll = new Vector2(Mathf.Min(data.x, maxScroll), data.y);
            zContent.SetScrollAmount(clampedScroll);
        }
    }
}
