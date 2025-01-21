using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Sortable : MonoBehaviour
{
    SpriteRenderer sorted;
    public bool sortingActive = true;
    public const float MIN_DISTANCE = 0.2f;
    int lastSortOrder = 0;

    protected virtual void Start()
    {
        sorted = GetComponent<SpriteRenderer>();
    }
    protected virtual void LateUpdate()
    {
        if(!sorted) return;
        int newSortOrder = (int)(-transform.position.y / MIN_DISTANCE);
        if(lastSortOrder != newSortOrder)
        {
            lastSortOrder = sorted.sortingOrder;
            sorted.sortingOrder = newSortOrder;
        }
    }
}
