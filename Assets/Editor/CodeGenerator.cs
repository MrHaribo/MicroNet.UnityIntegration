using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ModelGeneration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using Jurassic;

namespace AssemblyCSharpEditor
{
	public class CodeGenerator
	{
        private static void generateModelClass(JObject templateObject, Dictionary<string, JObject> allTemplates)
        {
            List<string> methods = new List<string>();
            List<string> fieldNames = new List<string>();
            List<string> fieldTypes = new List<string>();

            List<string> constantNames = new List<string>();
            List<string> constantTypes = new List<string>();
            List<string> constantValues = new List<string>();

            var scripts = new Dictionary<string, KeyValuePair<string, string[]>>();

            string templateName = (string)templateObject["name"];

            foreach (JToken variableToken in templateObject["variables"])
            {
                string variableName = (string)variableToken["name"];

                JObject variableTypeObject = (JObject)variableToken["type"];
                string variableType = (string)variableTypeObject["type"];
                string variableTypename = getVariableTypename(variableTypeObject);

                if (variableToken["constVar"] != null)
                {
                    string constValueString = getConstValue(variableTypeObject, variableToken["constVar"]);

                    constantNames.Add(variableName);
                    constantTypes.Add(variableTypename);
                    constantValues.Add(constValueString);
                }
                else
                {
                    if (variableType == "SCRIPT")
                    {
                        string scriptName = (string)variableTypeObject["scriptName"];
                        List<string> scriptArgs = new List<string>(variableTypeObject["memberArgs"].Values<string>());
                        var scriptDesc = new KeyValuePair<string, string[]>(scriptName, scriptArgs.ToArray());
                        scripts.Add(variableName, scriptDesc);
                    }
                    else if (variableTypename != null)
                    {
                        fieldNames.Add(variableName);
                        fieldTypes.Add(variableTypename);
                    }
                }
            }

            List<JToken> parentCtorArgTokens = new List<JToken>();
            string parentName = (string)templateObject["parent"];
            string parentNameRef = parentName;
            while (parentNameRef != null)
            {
                JObject parentObject = allTemplates[parentNameRef];
                List<JToken> parentCtorArgs = getCtorArgs(parentObject);

                parentCtorArgs.Reverse();
                parentCtorArgTokens.AddRange(parentCtorArgs);

                parentNameRef = (string)parentObject["parent"];
            }
            parentCtorArgTokens.Reverse();

            List<JToken> ctorArgTokens = getCtorArgs(templateObject);

            List<string> ctorArgs = new List<string>();
            StringBuilder ctorArgString = new StringBuilder();
            StringBuilder superCtorArgString = new StringBuilder();

            foreach (JToken parentCtorArg in parentCtorArgTokens) 
            {
                string variableName = (string)parentCtorArg["name"];
                string variableTypename = getVariableTypename((JObject)parentCtorArg["type"]);

                superCtorArgString.AppendFormat("{0},", variableName);
                ctorArgString.AppendFormat("{0} {1},", variableTypename, variableName);
            }

            foreach (JToken parentCtorArg in ctorArgTokens)
            {
                string variableName = (string)parentCtorArg["name"];
                string variableTypename = getVariableTypename((JObject)parentCtorArg["type"]);

                ctorArgs.Add(variableName);
                ctorArgString.AppendFormat("{0} {1},", variableTypename, variableName);
            }

            TemplateGenerator generator = new TemplateGenerator(templateName, parentName);
            generator.fieldNames = fieldNames.ToArray();
            generator.fieldTypes = fieldTypes.ToArray();
            generator.constantNames = constantNames.ToArray();
            generator.constantTypes = constantTypes.ToArray();
            generator.constantValues = constantValues.ToArray();
            generator.ctorArgs = ctorArgs.ToArray();
            generator.ctorArgString = ctorArgString.ToString().TrimEnd(',');
            generator.superCtorArgString = superCtorArgString.ToString().TrimEnd(',');
            generator.scripts = scripts;

            var classDefintion = generator.TransformText();

            GenerateCodeFile(templateName, classDefintion);
        }

        private static List<JToken> getCtorArgs(JObject templateObject)
        {
            List<JToken> ctorArgs = new List<JToken>();
            foreach (JToken variableToken in templateObject["variables"])
            {
                if (variableToken["constVar"] != null)
                    continue;
                
                if (variableToken["ctorArg"] == null || !variableToken["ctorArg"].Value<bool>())
                    continue;

                JObject variableTypeObject = (JObject)variableToken["type"];
                string variableTypename = getVariableTypename(variableTypeObject);

                if (variableTypename != null)
                    ctorArgs.Add(variableToken);
            }
            return ctorArgs;
        }

        private static string getConstValue(JObject variableTypeObject, JToken valueObj)
        {
            string variableType = (string)variableTypeObject["type"];
            switch (variableType)
            {
                case "STRING":
                    return String.Format("\"{0}\"", valueObj.Value<string>());
                case "NUMBER":
                    string numberType = (string)variableTypeObject["numberType"];
                    switch (numberType)
                    {
                        case "BYTE":
                            return valueObj.Value<byte>().ToString();
                        case "SHORT":
                            return valueObj.Value<short>().ToString();
                        case "INT":
                            return valueObj.Value<int>().ToString();
                        case "LONG":
                            return valueObj.Value<long>().ToString();
                        case "FLOAT":
                            return valueObj.Value<float>().ToString();
                        case "DOUBLE":
                            return valueObj.Value<double>().ToString();
                    }
                    return null;
                case "BOOLEAN":
                    return valueObj.Value<bool>().ToString();
                case "CHAR":
                    return String.Format("'{0}'", valueObj.Value<char>().ToString());
                case "ENUM":
                    return valueObj.Value<string>();
                case "LIST":
                case "SET":
                case "MAP":
                case "COMPONENT":
                case "SCRIPT":
                    return null;
                case "GEOMETRY":
                    string geometryType = (string)variableTypeObject["geometryType"];
                    switch (geometryType)
                    {
                        case "VECTOR3":
                            return "Vector3";
                        case "VECTOR2":
                            return "Vector2";
                    }
                    return null;
                default:
                    return null;
            }
        }

        private static string getVariableTypename(JObject variableTypeObject)
        {
            string variableType = (string)variableTypeObject["type"];


            switch (variableType)
            {
                case "STRING":
                    return "string";
                case "NUMBER":
                    string numberType = (string)variableTypeObject["numberType"];
                    switch (numberType)
                    {
                        case "BYTE":
                            return "byte";
                        case "SHORT":
                            return "short";
                        case "INT":
                            return "int";
                        case "LONG":
                            return "long";
                        case "FLOAT":
                            return "float";
                        case "DOUBLE":
                            return "double";
                    }
                    return null;
                case "BOOLEAN":
                    return "bool";
                case "CHAR":
                    return "char";
                case "ENUM":
                    return (string)variableTypeObject["enumType"];
                case "LIST":
                    JObject listEntryTypeObject = (JObject)variableTypeObject["entryType"];
                    string listEntryTypename = getVariableTypename(listEntryTypeObject);
                    return string.Format("List<{0}>", listEntryTypename);
                case "SET":
                    JObject setEntryTypeObject = (JObject)variableTypeObject["entryType"];
                    string setEntryTypename = getVariableTypename(setEntryTypeObject);
                    return string.Format("HashSet<{0}>", setEntryTypename);
                case "MAP":
                    JObject mapEntryTypeObject = (JObject)variableTypeObject["entryType"];
                    string mapEntryTypename = getVariableTypename(mapEntryTypeObject);
                    JObject mapKeyTypeObject = (JObject)variableTypeObject["keyType"];
                    string mapKeyTypename = getVariableTypename(mapKeyTypeObject);
                    return string.Format("Dictionary<{0},{1}>", mapKeyTypename, mapEntryTypename);
                case "COMPONENT":
                    return (string)variableTypeObject["componentType"];
                case "SCRIPT":
                    return null;
                case "GEOMETRY":
                    string geometryType = (string)variableTypeObject["geometryType"];
                    switch (geometryType)
                    {
                        case "VECTOR3":
                            return "Vector3";
                        case "VECTOR2":
                            return "Vector2";
                    }
                    return null;
                default:
                    return null;
            }
            
        }

        [MenuItem("MicroNet/Generate Model")]
        public static void GenerateModel()
        {
            string sharedDir = EditorPrefs.GetString("SharedDirLocation", null);
            if (sharedDir == null)
            {
                Debug.Log("SharedDir Location not specified");
                return;
            }

            Dictionary<string, JObject> allTemplates = new Dictionary<string, JObject>();

            DirectoryInfo templateDir = new DirectoryInfo(sharedDir + "/model/templates");
            foreach (FileInfo file in templateDir.GetFiles())
            {
                String jsonString = File.ReadAllText(file.FullName);

                JObject templateObject = JObject.Parse(jsonString);
                string templateName = (string)templateObject["name"];

                allTemplates.Add(templateName, templateObject);
            }

            foreach (KeyValuePair<string, JObject> template in allTemplates)
            {
                generateModelClass(template.Value, allTemplates);
            }

        }

        [MenuItem("MicroNet/Generate Enums")]
        public static void GenerateEnum()
        {
            string sharedDir = EditorPrefs.GetString("SharedDirLocation", null);
            if (sharedDir == null)
            {
                Debug.Log("SharedDir Location not specified");
                return;
            }

            DirectoryInfo enumDir = new DirectoryInfo(sharedDir + "/model/enums");
            foreach (FileInfo file in enumDir.GetFiles())
            {
                String jsonString = File.ReadAllText(file.FullName);
                JObject enumDescription = JObject.Parse(jsonString);
                string enumName = (string)enumDescription["name"];
                JArray enumEntryArray = (JArray)enumDescription["variables"];

                List<string> enumEntries = new List<string>();
                foreach (JToken entry in enumEntryArray)
                    enumEntries.Add(entry.Value<string>());

                EnumGenerator generator = new EnumGenerator(enumName, enumEntries.ToArray());
                var classDefintion = generator.TransformText();
                GenerateCodeFile(enumName, classDefintion);
            }
        }

        [MenuItem("MicroNet/Generate ParameterCodes")]
        public static void GenerateParameterCodes()
        {
            string sharedDir = EditorPrefs.GetString("SharedDirLocation", null);
            if (sharedDir == null)
            {
                Debug.Log("SharedDir Location not specified");
                return;
            }

            String jsonString = File.ReadAllText(sharedDir + "/ParameterCode");

            JArray parameterCodeEntries = JArray.Parse(jsonString);
            List<string> parameterCodes = new List<string>();

            foreach (JToken code in parameterCodeEntries)
            {
                parameterCodes.Add(code.Value<string>());
            }

            EnumGenerator generator = new EnumGenerator("ParameterCode", parameterCodes.ToArray());
            var classDefintion = generator.TransformText();
            GenerateCodeFile("ParameterCode", classDefintion);
        }

        [MenuItem("MicroNet/Run Script")]
        public static void RunScript()
        {
            object result = ScriptExecutor.invoke("myScript", 2, 90);
            Debug.Log("From Script :): " + Convert.ToInt32(result));

            //string sharedDir = EditorPrefs.GetString("SharedDirLocation", null);
            //String codeString = File.ReadAllText(sharedDir + "/scripts/myScript.js");

            //ScriptEngine engine = new ScriptEngine();
            //engine.SetGlobalFunction("print", new System.Action<string>(Debug.Log));


            //engine.Execute(codeString);


            //int result = engine.CallGlobalFunction<int>("test", 3, 5);

            //Debug.Log("Script Result: " + result);
        }

        private static void GenerateCodeFile(string name, string data)
		{
			var outputPath = Path.Combine(Application.dataPath, name + ".cs");
			File.WriteAllText(outputPath, data);
			AssetDatabase.Refresh();
		}
	}
}