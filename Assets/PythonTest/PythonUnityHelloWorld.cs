using UnityEngine;

namespace Exodrifter.UnityPython.Examples
{
	public class PythonUnityHelloWorld : MonoBehaviour
	{
		void Start()
		{
            var stopwatch = new System.Diagnostics.Stopwatch();
			var engine = global::UnityPython.CreateEngine();
			var scope = engine.CreateScope();

			string code = "import UnityEngine\n";
			code += "UnityEngine.Debug.Log('Hello world!')";

			var source = engine.CreateScriptSourceFromString(code);
            stopwatch.Start();
            for (int i = 0; i < 1000; i++) source.Execute(scope);
            Debug.LogWarning($"耗時 {stopwatch.Elapsed}");

        }
	}
}