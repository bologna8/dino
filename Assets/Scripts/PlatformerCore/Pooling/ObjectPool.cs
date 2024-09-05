using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public int maxSize = 99; //max size of list, with default starting size
    public string name; //Of the prefab to pool
    public List<GameObject> activeQ = new List<GameObject>();
    public List<GameObject> inactiveQ = new List<GameObject>();
}
