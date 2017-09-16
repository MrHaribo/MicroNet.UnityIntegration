using Jurassic;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScriptExecutor {

    public static object invoke(string functionName, params object[] arguments)
    {
        TextAsset scriptLocationAsset = Resources.Load("shared_dir_location") as TextAsset;

        string sharedDir = scriptLocationAsset.text;
        string codeString = File.ReadAllText(string.Format("{0}/scripts/{1}.js", sharedDir, functionName));

        ScriptEngine engine = new ScriptEngine();
        engine.SetGlobalFunction("print", new System.Action<string>(Debug.Log));
        engine.Execute(codeString);

        return engine.CallGlobalFunction(functionName, arguments);
    }
}
