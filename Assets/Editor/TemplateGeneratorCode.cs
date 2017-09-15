using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelGeneration
{
    public partial class TemplateGenerator
    {
        private string templateName;
        private string parentName;
        private string[] fieldNames;
        private string[] fieldTypes;

        public TemplateGenerator(string templateName, string parentName, string[] fieldNames, string[] fieldTypes)
        {
            this.templateName = templateName;
            this.fieldNames = fieldNames;
            this.fieldTypes = fieldTypes;
            this.parentName = parentName;
        }
    }

}
