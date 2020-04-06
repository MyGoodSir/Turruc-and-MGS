using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest : MonoBehaviour
{

    public Action<int, int> useNums;

    
    void Start()
    {
        useNums += AddNums;
        useNums += (a, b) => Debug.Log("Product: " + (a * b));

        useNums(5, 2);
    }

    void AddNums(int a, int b)
    {
        Debug.Log("Sum: " + (a + b));
    }

    
}
