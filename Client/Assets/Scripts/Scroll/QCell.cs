using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QCell : MonoBehaviour
{
    public QContent qContent;
    public GameObject entity;

    int index;

    public Vector2 pos;

    float leftBound;
    float rightBound;

    bool wasIn;

    public QCell(QContent qContent, int index, Vector2 pos, float leftBound, float rightBound)
    {
        this.qContent = qContent;
        this.index = index;
        this.pos = pos;
        this.leftBound = leftBound;
        this.rightBound = rightBound;
        wasIn = false;
        OnScroll(0);
    }

    public void OnScroll(float scrollAmount)
    {
        if (!wasIn && IsIn(scrollAmount))
        {
            wasIn = true;
            entity = qContent.pool.Get();
            entity.transform.SetParent(qContent.transform);
            entity.GetComponent<RectTransform>().anchoredPosition = pos;
            if (qContent.updateChildrenCallback != null)
            {
                qContent.updateChildrenCallback(index, entity.transform);
            }
        }

        else if (wasIn && !IsIn(scrollAmount))
        {
            wasIn = false;
            if (entity)
            {
                qContent.pool.Recycle(entity);
                entity = null;
            }
        }
    }

    bool IsIn(float scrollAmount)
    {
        if (rightBound > scrollAmount && leftBound < scrollAmount + qContent.qScroll.rect.rect.width)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Dispose()
    {
        qContent.pool.Recycle(entity);
        entity = null;
    }
}
