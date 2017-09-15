using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ModelGeneration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AssemblyCSharpEditor
{
	/// <summary>
	/// A helper class for code generation in the editor.
	/// </summary>
	public class CodeGenerator
	{
        private static void generateModelClass(string jsonString)
        {
            List<string> methods = new List<string>();
            List<string> fieldNames = new List<string>();
            List<string> fieldTypes = new List<string>();
            List<string> ctorArgs = new List<string>();

            JObject templateObject = JObject.Parse(jsonString);

            string templateName = (string)templateObject["name"];
            string parentName = (string)templateObject["parent"];

            foreach (JToken variableToken in templateObject["variables"])
            {
                string variableName = (string)variableToken["name"];

                JObject variableTypeObject = (JObject)variableToken["type"];
                string variableTypename = getVariableTypename(variableTypeObject);

                string variableType = (string)variableTypeObject["type"];

                if (variableTypename != null)
                {
                    fieldNames.Add(variableName);
                    fieldTypes.Add(variableTypename);
                }
            }

            TemplateGenerator generator = new TemplateGenerator(templateName, parentName, fieldNames.ToArray(), fieldTypes.ToArray());
            var classDefintion = generator.TransformText();

            GenerateCodeFile(templateName, classDefintion);
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

            DirectoryInfo templateDir = new DirectoryInfo(sharedDir + "/model/templates");
            foreach (FileInfo file in templateDir.GetFiles())
            {
                String jsonString = File.ReadAllText(file.FullName);
                generateModelClass(jsonString);
            }
        }

        [MenuItem("MicroNet/Generate Enum")]
        public static void GenerateEnum()
        {
            EnumGenerator generator = new EnumGenerator("MyEnum", new string[] { "Hans", "Ruedi" });
            var classDefintion = generator.TransformText();
            GenerateCodeFile("MyEnum", classDefintion);
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

		private static void GenerateCodeFile(string name, string data)
		{
			var outputPath = Path.Combine(Application.dataPath, name + ".cs");
			File.WriteAllText(outputPath, data);
			AssetDatabase.Refresh();
		}
	}
}