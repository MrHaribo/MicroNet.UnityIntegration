using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelGeneration
{
    public partial class EnumGenerator
    {
        private string enumName;
        private string[] enumConstants;

        public EnumGenerator(string enumName, string[] enumConstants)
        {
            if (string.IsNullOrEmpty(enumName))
            {
                throw new ArgumentException("generatedClassName");
            }

            if (enumConstants == null)
            {
                throw new ArgumentNullException("source cannot be null!");
            }

            this.enumName = enumName;
            this.enumConstants = enumConstants;
        }
    }

}
