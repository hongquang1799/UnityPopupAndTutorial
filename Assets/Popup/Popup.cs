using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    public Action ShowEvent;

    public Action HideEvent;

    protected Transform cachedTransform;

    [NonSerialized] public bool preInstantiated;

    [NonSerialized] public bool exitByBackBlocker;

    void Awake()
    {
        cachedTransform = transform;
    }

    public virtual void Show()
    {
        cachedTransform.DOKill();
        cachedTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
        cachedTransform.DOScale(1f, 0.35f);
    }

    public virtual void Hide()
    {
        cachedTransform.DOKill();
        cachedTransform.localScale = new Vector3(1f, 1f, 1f);
        cachedTransform.DOScale(0f, 0.35f).
            OnComplete(() =>
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            });
    }
}
