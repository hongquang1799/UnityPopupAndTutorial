using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HBomb : Chip
{
    public override void Initialize()
    {
        base.Initialize();

        type = ChipType.HBomb;
    }
}
