using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PopupSystem.Instance.ShowPopup(PopupType.PopupGameStart);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            PopupSystem.Instance.ShowPopup(PopupType.PopupGameLose);
        }
    }
}
