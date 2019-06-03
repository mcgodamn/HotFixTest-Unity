using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;


public delegate void TestDelegateMethod(int a);
public delegate string TestDelegateFunction(int a);

public class ILRTTest : MonoBehaviour
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
        // HelloWorld();
        Invocation();
        // Delegate();
    }

    void HelloWorld()
    {
        appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest", null, null);
    }

    void Invocation()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        Debug.Log("调用无参数静态方法");
        stopwatch.Start();
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        for (int i = 0; i < 1000; i++) appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest", null, null);

        stopwatch.Stop();
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        //调用带参数的静态方法
        stopwatch.Restart();
        Debug.Log("调用带参数的静态方法");
        for (int i = 0; i < 1000; i++) appdomain.Invoke("HotFix_Project.InstanceClass", "StaticFunTest2", null, 123);

        stopwatch.Stop();
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        Debug.Log("通过IMethod调用方法");
        stopwatch.Restart();
        //预先获得IMethod，可以减低每次调用查找方法耗用的时间
        IType type = appdomain.LoadedTypes["HotFix_Project.InstanceClass"];
        //根据方法名称和参数个数获取方法
        IMethod method = type.GetMethod("StaticFunTest", 0);

        for (int i = 0; i < 1000; i++) appdomain.Invoke(method, null, null);

        stopwatch.Stop();
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        Debug.Log("指定参数类型来获得IMethod");
        stopwatch.Restart();
        IType intType = appdomain.GetType(typeof(int));
        //参数类型列表
        List<IType> paramList = new List<ILRuntime.CLR.TypeSystem.IType>();
        paramList.Add(intType);
        //根据方法名称和参数类型列表获取方法
        method = type.GetMethod("StaticFunTest2", paramList, null);
        for (int i = 0; i < 1000; i++) appdomain.Invoke(method, null, 456);

        stopwatch.Stop();
        Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        // Debug.Log("实例化热更里的类");
        // stopwatch.Restart();
        // object obj = appdomain.Instantiate("HotFix_Project.InstanceClass", new object[] { 233 });
        // //第二种方式
        // object obj2 = ((ILType)type).Instantiate();

        // stopwatch.Stop();
        // Debug.Log($"耗時 {stopwatch.Elapsed}");

        // Debug.Log("调用成员方法");
        // stopwatch.Restart();
        // int id = (int)appdomain.Invoke("HotFix_Project.InstanceClass", "get_ID", obj, null);
        // Debug.Log("!! HotFix_Project.InstanceClass.ID = id");
        // id = (int)appdomain.Invoke("HotFix_Project.InstanceClass", "get_ID", obj2, null);
        // Debug.Log("!! HotFix_Project.InstanceClass.ID = id");

        // stopwatch.Stop();
        // Debug.Log($"耗時 {stopwatch.Elapsed}");

        // Debug.Log("调用泛型方法");
        // stopwatch.Restart();
        // IType stringType = appdomain.GetType(typeof(string));
        // IType[] genericArguments = new IType[] { stringType };
        // appdomain.InvokeGenericMethod("HotFix_Project.InstanceClass", "GenericMethod", genericArguments, null, "TestString");

        // stopwatch.Stop();
        // Debug.Log($"耗時 {stopwatch.Elapsed}");

        // Debug.Log("获取泛型方法的IMethod");
        // stopwatch.Restart();
        // paramList.Clear();
        // paramList.Add(intType);
        // genericArguments = new IType[] { intType };
        // method = type.GetMethod("GenericMethod", paramList, genericArguments);
        // appdomain.Invoke(method, null, 33333);

        // stopwatch.Stop();
        // Debug.Log($"耗時 {stopwatch.Elapsed}");
    }


    //这个方法仅仅是为了演示，强制删除缓存的委托适配器，实际项目不要这么调用
    void ClearDelegateCache()
    {
        var type = appdomain.LoadedTypes["HotFix_Project.TestDelegate"];
        ILMethod m = type.GetMethod("Method", 1) as ILMethod;
        m.DelegateAdapter = null;

        m = type.GetMethod("Function", 1) as ILMethod;
        m.DelegateAdapter = null;

        m = type.GetMethod("Action", 1) as ILMethod;
        m.DelegateAdapter = null;
    }


    public static TestDelegateMethod TestMethodDelegate;
    public static TestDelegateFunction TestFunctionDelegate;
    public static System.Action<string> TestActionDelegate;

    void Delegate()
    {
        Debug.Log("完全在热更DLL内部使用的委托，直接可用，不需要做任何处理");
        appdomain.Invoke("HotFix_Project.TestDelegate", "Initialize", null, null);
        appdomain.Invoke("HotFix_Project.TestDelegate", "RunTest", null, null);

        Debug.Log("如果需要跨域调用委托（将热更DLL里面的委托实例传到Unity主工程用）, 就需要注册适配器，不然就会像下面这样");
        try
        {
            appdomain.Invoke("HotFix_Project.TestDelegate", "Initialize2", null, null);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
        //为了演示，清除适配器缓存，实际使用中不要这么做
        ClearDelegateCache();
        Debug.Log("这是因为iOS的IL2CPP模式下，不能动态生成类型，为了避免出现不可预知的问题，我们没有通过反射的方式创建委托实例，因此需要手动进行一些注册");
        Debug.Log("首先需要注册委托适配器,刚刚的报错的错误提示中，有提示需要的注册代码");
        //下面这些注册代码，正式使用的时候，应该写在InitializeILRuntime中
        //TestDelegateMethod, 这个委托类型为有个参数为int的方法，注册仅需要注册不同的参数搭配即可
        appdomain.DelegateManager.RegisterMethodDelegate<int>();
        //带返回值的委托的话需要用RegisterFunctionDelegate，返回类型为最后一个
        appdomain.DelegateManager.RegisterFunctionDelegate<int, string>();
        //Action<string> 的参数为一个string
        appdomain.DelegateManager.RegisterMethodDelegate<string>();


        Debug.Log("注册完毕后再次运行会发现这次会报另外的错误");
        try
        {
            appdomain.Invoke("HotFix_Project.TestDelegate", "Initialize2", null, null);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        Debug.Log("ILRuntime内部是用Action和Func这两个系统内置的委托类型来创建实例的，所以其他的委托类型都需要写转换器");
        Debug.Log("将Action或者Func转换成目标委托类型");

        appdomain.DelegateManager.RegisterDelegateConvertor<TestDelegateMethod>((action) =>
        {
            //转换器的目的是把Action或者Func转换成正确的类型，这里则是把Action<int>转换成TestDelegateMethod
            return new TestDelegateMethod((a) =>
            {
                //调用委托实例
                ((System.Action<int>)action)(a);
            });
        });
        //对于TestDelegateFunction同理，只是是将Func<int, string>转换成TestDelegateFunction
        appdomain.DelegateManager.RegisterDelegateConvertor<TestDelegateFunction>((action) =>
        {
            return new TestDelegateFunction((a) =>
            {
                return ((System.Func<int, string>)action)(a);
            });
        });

        //下面再举一个这个Demo中没有用到，但是UGUI经常遇到的一个委托，例如UnityAction<float>
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<float>>((action) =>
        {
            return new UnityEngine.Events.UnityAction<float>((a) =>
            {
                ((System.Action<float>)action)(a);
            });
        });


        Debug.Log("现在我们再来运行一次");
        appdomain.Invoke("HotFix_Project.TestDelegate", "Initialize2", null, null);
        appdomain.Invoke("HotFix_Project.TestDelegate", "RunTest2", null, null);
        Debug.Log("运行成功，我们可以看见，用Action或者Func当作委托类型的话，可以避免写转换器，所以项目中在不必要的情况下尽量只用Action和Func");
        Debug.Log("另外应该尽量减少不必要的跨域委托调用，如果委托只在热更DLL中用，是不需要进行任何注册的");
        Debug.Log("---------");
        Debug.Log("我们再来在Unity主工程中调用一下刚刚的委托试试");
        TestMethodDelegate(789);
        var str = TestFunctionDelegate(098);
        Debug.Log("!! OnHotFixLoaded str = str");
        TestActionDelegate("Hello From Unity Main Project");
    }
}
