using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();
        for (int i = 0; i < 1000; i++) Print();
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        stopwatch.Restart();
        for (int i = 0; i < 1000; i++) Print2("Hi");
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        stopwatch.Restart();
        for (int i = 0; i < 1000; i++) List();
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Print()
    {
        Debug.Log("hello world");
    }

    void Print2(string a)
    {
        Debug.Log(a);
    }

    void List()
    {
        var l = new List<string>();
        l.Add("asd");
    }
}
