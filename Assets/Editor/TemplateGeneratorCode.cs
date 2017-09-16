using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelGeneration
{
    public partial class TemplateGenerator
    {
        public string templateName;
        public string parentName;

        public string[] fieldNames;
        public string[] fieldTypes;

        public string[] constantNames;
        public string[] constantTypes;
        public string[] constantValues;

        public string ctorArgString;
        public string superCtorArgString;
        public string[] ctorArgs;

        public Dictionary<string, KeyValuePair<string, string[]>> scripts = new Dictionary<string, KeyValuePair<string, string[]>>();

        public TemplateGenerator(string templateName, string parentName)
        {
            this.templateName = templateName;
            this.parentName = parentName;
        }
    }

}
