using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XluaTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        XLua.LuaEnv luaenv = new XLua.LuaEnv();
        var main = System.IO.File.ReadAllText(Application.dataPath + "\\main.lua");
        luaenv.DoString(main);
        stopwatch.Start();
        for(int i = 0; i < 1000;i++) luaenv.DoString("StaticFunTest()");
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        stopwatch.Restart();
        for (int i = 0; i < 1000; i++) luaenv.DoString("StaticFunTest2('123')");
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        stopwatch.Restart();
        for (int i = 0; i < 1000; i++) luaenv.DoString("AddGenericList()");
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        stopwatch.Restart();
        luaenv.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
