using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vomsProject.Helpers
{
    public class StylesheetOptions
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public static List<StylesheetOptions> GetFromString(string options)
        {
            var stylesheetList = new List<StylesheetOptions>();
            var optionsList = options.Split('\n');
            foreach (var option in optionsList)
            {
                var values = option.Trim().Split(',');
                if (values.Length == 3)
                {
                    stylesheetList.Add(new StylesheetOptions
                    {
                        Name = values[0],
                        Description = values[1],
                        Type = values[2]                        
                    });
                }
            }
            return stylesheetList;
        }
    }
}
