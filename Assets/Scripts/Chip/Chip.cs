using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChipType
{
    None,
    SimpleChip,
    BringDown,
    SimpleBomb,
    RainbowBomb,
    HBomb,
    VBomb,
}

public class Chip : MonoBehaviour
{
    [NonSerialized]
    public int x, y;

    public int rtx, rty;

    [NonSerialized]
    public int colorType;

    public ChipType type;

    public List<Vector3> path = new List<Vector3>();

    protected Animator animator;
 
    protected int currentTargetIndex;

    protected bool canMatch;

    protected int health;

    protected float speed;

    public virtual void Initialize()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;

        canMatch = true;
        health = 1;
    }

    public virtual void Explode(BoardController board)
    {
        health -= 1;

        if (health == 0)
        {
            Destroy(gameObject);
        }        
    }

    public void AddPathPosition(Vector3 position)
    {
        if (path.Count == 0)
        {
            speed = 0f;
            currentTargetIndex = 0;
        }

        path.Add(position);
    }

    public void MoveToTarget(float dt)
    {
        if (path.Count <= 0) return;

        speed += 0.15f;
        float speedDt = Mathf.Clamp(speed * dt, 0f, 0.15f);

        Vector3 localPosition = transform.localPosition;
        Vector3 targetPosition = path[currentTargetIndex];

        Vector3 delta = targetPosition - localPosition;

        if (delta.x != 0f) speedDt *= 1.4147f;

        float subMagnitude = delta.magnitude - speedDt;

        if (subMagnitude > 0f)
        {
            localPosition += delta.normalized * speedDt;
        }
        else
        {
            if (currentTargetIndex + 1 == path.Count) // reached the target
            {
                localPosition = targetPosition;
                path.Clear();
                speed = 0f;
                currentTargetIndex = 0;
            }
            else
            {
                localPosition = targetPosition - (path[currentTargetIndex + 1] - targetPosition) * subMagnitude;
                currentTargetIndex++;
            }
        }

        transform.localPosition = localPosition;
    }

    public bool IsFalling()
    {
        return path.Count > 0;
    }

    public bool CanMatch()
    {
        return canMatch;
    }
}
