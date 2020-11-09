using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            var popup = Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameStart,
                Popup.CurrentPopupBehaviour.KeepShowing, () => Debug.Log("Dit me may"));
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGameLose,
                Popup.CurrentPopupBehaviour.HideTemporary, Popup.PopupSystem.Instance.ClosePopup);
        }
    }
}
