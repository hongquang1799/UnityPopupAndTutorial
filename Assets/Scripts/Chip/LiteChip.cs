using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiteChip : Chip
{
    public override void Initialize()
    {
        base.Initialize();

        type = ChipType.SimpleChip;
    }
}
