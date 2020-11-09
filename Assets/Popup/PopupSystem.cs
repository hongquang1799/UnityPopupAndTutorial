using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Popup
{
    public enum CurrentPopupBehaviour
    {
        KeepShowing,
        HideTemporary,
        Close
    }

    public class PopupSystem : MonoSingleton<PopupSystem>
    {
        [Serializable]
        public struct PopupTypePrefabPair
        {
            public PopupType type;
            public GameObject prefab;
            public bool cachedAndPreInstantiate;
        }

        public class PopupInfo
        {
            public Popup instance;
            public Action backBlockerPressedEvent;
        }

        public BlurEffect blurEffect;

        public BlockingImage backBlocker;

        public Transform rootTransform;

        public Stack<PopupInfo> popupInfoStack = new Stack<PopupInfo>();

        public List<PopupTypePrefabPair> popupList;

        private Dictionary<PopupType, GameObject> popupTable = new Dictionary<PopupType, GameObject>();

        public override void Awake()
        {
            base.Awake();

            if (backBlocker.transform.parent != rootTransform)
                backBlocker.transform.parent = rootTransform;

            backBlocker.gameObject.SetActive(false);

            for (int i = 0; i < popupList.Count; i++)
            {
                popupTable.Add(popupList[i].type, popupList[i].prefab);
            }
        }

        void Start()
        {
        }

        private void HandleCurrentPopup(CurrentPopupBehaviour currentPopupBehaviour)
        {
            var currentPopupInfo = popupInfoStack.Peek();
            var currentPopup = currentPopupInfo.instance;
            if (currentPopup != null) // not necessary
            {
                switch (currentPopupBehaviour)
                {
                    case CurrentPopupBehaviour.Close:
                        {
                            popupInfoStack.Pop().instance.Close();
                            break;
                        }
                    case CurrentPopupBehaviour.HideTemporary:
                        {
                            currentPopup.Close(false);
                            break;
                        }
                    case CurrentPopupBehaviour.KeepShowing:
                    default:
                        {
                            break;
                        }
                }
            }
        }

        public Popup ShowPopup(PopupType popupType, CurrentPopupBehaviour currentPopupBehaviour, Action BackBlockerPressedEvent = null)
        {
            if (popupTable.ContainsKey(popupType) == false)
                return null;

            GameObject prefab = popupTable[popupType];
            Popup popup = Instantiate(prefab, rootTransform).GetComponent<Popup>();

            if (popup == null)
                return null;

            blurEffect.enabled = true;
            backBlocker.GetComponent<RawImage>().texture = blurEffect.GetTexture();

            if (popupInfoStack.Count > 0)
            {
                HandleCurrentPopup(currentPopupBehaviour);
            }

            PopupInfo popupInfo = new PopupInfo();
            popupInfo.instance = popup;
            popupInfo.backBlockerPressedEvent = BackBlockerPressedEvent;

            backBlocker.transform.SetSiblingIndex(rootTransform.childCount - 2);
            backBlocker.gameObject.SetActive(true);

            backBlocker.PointerDownAction = BackBlockerPressedEvent;

            popup.Show();
            popupInfoStack.Push(popupInfo);

            return popup;
        }

        public void ClosePopup()
        {
            if (popupInfoStack.Count > 0)
            {
                PopupInfo popupInfo = popupInfoStack.Pop();
                Popup popup = popupInfo.instance;
                ClosePopup(popup);

                if (popupInfoStack.Count == 0)
                {
                    backBlocker.gameObject.SetActive(false);
                    backBlocker.PointerDownAction = null;

                    blurEffect.enabled = false;
                }
                else
                {
                    popupInfo = popupInfoStack.Peek();
                    if (popupInfo.instance.gameObject.activeSelf == false)
                    {
                        popupInfo.instance.gameObject.SetActive(true);
                        popupInfo.instance.Show();
                    }

                    backBlocker.transform.SetSiblingIndex(Mathf.Max(0, rootTransform.childCount - 3));
                    backBlocker.gameObject.SetActive(true);
                    backBlocker.PointerDownAction = popupInfo.backBlockerPressedEvent;
                }
            }
        }

        public void CloseAllPopups()
        {
            foreach (PopupInfo popupInfo in popupInfoStack)
            {
                ClosePopup(popupInfo.instance);
            }

            backBlocker.gameObject.SetActive(false);
            backBlocker.PointerDownAction = null;

            blurEffect.enabled = false;
        }

        private void ClosePopup(Popup popup, bool forceDestroying = true)
        {
            popup.Close(forceDestroying);
        }
    }

}
