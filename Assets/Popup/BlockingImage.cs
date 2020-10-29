﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockingImage : MonoBehaviour, IPointerDownHandler
{
    public Action PointerDownAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownAction?.Invoke();
    }
}
