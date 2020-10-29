using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VBomb : Chip
{
    public override void Initialize()
    {
        base.Initialize();

        type = ChipType.VBomb;
    }

    public override void Explode(BoardController board)
    {
        health -= 1;

        if (health == 0)
        {
            Destroy(gameObject);
            board.StartCoroutine(TriggerVertical(board));
        }    
    }

    private IEnumerator TriggerVertical(BoardController board)
    {
        int i = 1;

        while (true)
        {
            bool validUp = board.IsValidLocation(x, y + i);
            bool validDown = board.IsValidLocation(x, y - i);
            
            if (validUp) board.TriggerExplosion(x, y + i);
            if (validDown) board.TriggerExplosion(x, y - i);

            i++;

            if (!validUp && !validDown) break;

            yield return new WaitForSeconds(0.05f);
        }
    }
}
