using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    //Picks a Random Spawned Element
    public static T GetRandomElement<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0) return default;
        int i = Random.Range(0, list.Count);   // max is exclusive
        return list[i];
    }
}
