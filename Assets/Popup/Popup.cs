using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Popup
{
    public class Popup : MonoBehaviour
    {
        public Action PreAnimateShowEvent;

        public Action PostAnimateShowEvent;

        public Action PreAnimateHideEvent;

        public Action PostAnimateHideEvent;

        protected Transform cachedTransform;

        protected Action backBlockerEvent;

        void Awake()
        {
            cachedTransform = transform;
        }

        protected internal virtual void Show()
        {
            PreAnimateShowEvent?.Invoke();

            cachedTransform.DOKill();
            cachedTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
            cachedTransform.DOScale(1f, 0.35f).OnComplete(() =>
            {
                PostAnimateShowEvent?.Invoke();
            });
        }

        protected internal void Close(bool forceDestroying = true)
        {
            PreAnimateHideEvent?.Invoke();

            cachedTransform.DOKill();
            cachedTransform.localScale = new Vector3(1f, 1f, 1f);
            cachedTransform.DOScale(0f, 0.35f).
                OnComplete(() =>
                {
                    PostAnimateHideEvent?.Invoke();

                    if (forceDestroying)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                });
        }

        public void CloseInternal()
        {
            PopupSystem.Instance.ClosePopup();
        }
    }
}
