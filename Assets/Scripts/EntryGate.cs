using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryGate 
{
    private Queue<Chip> chipQueue = new Queue<Chip>();

    public Vector3 position;

    public float queueDistance;

    private Chip lastChip;

    public void Add(Chip chip)
    {
        if (chipQueue.Count > 0)
        {
            chip.transform.localPosition = lastChip.transform.position + new Vector3(0f, queueDistance, 0f);
        }
        else
        {
            chip.transform.localPosition = position;
        }

        lastChip = chip;
        chipQueue.Enqueue(chip);
        chip.gameObject.SetActive(false);
    }

    public void Update()
    {
        while (chipQueue.Count > 0)
        {
            Chip chip = chipQueue.Peek();

            if (chip.transform.localPosition.y < position.y)
            {
                chip = chipQueue.Dequeue();
                chip.gameObject.SetActive(true);

                if (chipQueue.Count == 0)
                {
                    lastChip = null;
                }
            }
            else
            {
                break;
            }
        }        
    }
}
