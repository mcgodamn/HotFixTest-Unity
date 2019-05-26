using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;

public class Test : MonoBehaviour
{
    AppDomain appdomain;

    void Start()
    {
        LoadILRuntime();
    }

    void LoadILRuntime()
    {
        appdomain = new AppDomain();

        AssetBundle bundle = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundles/hotfix.fuckyeah");

        TextAsset txt = bundle.LoadAsset("dll.bytes") as TextAsset;
        byte[] dll = txt.bytes;
        txt = bundle.LoadAsset("pdb.bytes") as TextAsset;
        byte[] pdb = txt.bytes;

        using (System.IO.MemoryStream fs = new System.IO.MemoryStream(dll))
        {
            using (System.IO.MemoryStream p = new System.IO.MemoryStream(pdb))
            {
                appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
            }
        }
        OnHotFixLoaded();
    }



    void OnHotFixLoaded()
    {
        appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest", null, null);
    }
}
