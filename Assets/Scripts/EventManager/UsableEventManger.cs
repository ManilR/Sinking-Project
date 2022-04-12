using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableEventManger : MonoBehaviour
{
    public static UsableEventManger current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onAction;
    public void TriggerAction(int id)
    {
        if (onAction != null)
            onAction(id);
    }

    public event Action<int> onUp;
    public void TriggerUp(int id)
    {
        if (onUp != null)
            onUp(id);
    }

    public event Action<int> onDown;
    public void TriggerDown(int id)
    {
        if (onDown != null)
            onDown(id);
    }

    public event Action<int> onLeft;
    public void TriggerLeft(int id)
    {
        if (onLeft != null)
            onLeft(id);
    }

    public event Action<int> onRight;
    public void TriggerRight(int id)
    {
        if (onRight != null)
            onRight(id);
    }
}
