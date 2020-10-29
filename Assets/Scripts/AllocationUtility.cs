using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllocationUtility 
{
    public static T[,] Allocate2D<T>(int w, int h, T defaultValue)
    {
        T[,] array = new T[w, h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                array[x, y] = defaultValue;
            }
        }
        return array;
    }
}
