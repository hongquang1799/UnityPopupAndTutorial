using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSystem : MonoSingleton<PopupSystem>
{
    public enum CurrentPopupBehaviour
    {
        HideTemp,
        HideThenDestroy
    }

    [Serializable]
    public struct PopupTypePrefabPair
    {
        public PopupType type;
        public GameObject prefab;
    }

    public BlurEffect blurEffect;

    public BlockingImage backBlocker;

    public Transform rootTransform;

    public Stack<Popup> popups = new Stack<Popup>();

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

    public Popup ShowPopup(PopupType popupType, bool exitByBackBlocker = true)
    {
        if (popupTable.ContainsKey(popupType) == false)
            return null;
             
        GameObject prefab = popupTable[popupType];      
        Popup popup = Instantiate(prefab, rootTransform).GetComponent<Popup>();

        if (popup == null)
            return null;

        blurEffect.enabled = true;
        backBlocker.GetComponent<RawImage>().texture = blurEffect.GetTexture();
        backBlocker.transform.SetSiblingIndex(rootTransform.childCount - 2);
        backBlocker.gameObject.SetActive(true);

        if (exitByBackBlocker) 
            backBlocker.PointerDownAction = HidePopup;

        popup.Show();
        popup.exitByBackBlocker = exitByBackBlocker;

        popups.Push(popup);       

        return popup;
    }

    public void HidePopup()
    {
        if (popups.Count > 0)
        {
            Popup popup = popups.Pop();
            HidePopup(popup);

            if (popups.Count == 0)
            {
                backBlocker.gameObject.SetActive(false);
                blurEffect.enabled = false;
            }
            else
            {
                backBlocker.transform.SetSiblingIndex(rootTransform.childCount - 2);
            }
        }
    }

    public void HideAllPopup()
    {
        foreach (Popup popup in popups)
        {
            HidePopup(popup);
        }
    }

    private void HidePopup(Popup popup)
    {
        popup.Hide();
    }
}
