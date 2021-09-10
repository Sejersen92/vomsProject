using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vomsProject.Editor
{
    public enum BlockType
    {
        Container,
        Text
    }
    public class TransferBlock
    {
        public BlockType type;
        public string tagType;
        public Dictionary<string, string> properties;
        public IEnumerable<TransferBlock> blocks;
        public string text;
        private static bool IsSelfClosing(string tagType) 
        {
            switch (tagType)
            {
                case "area":
                case "base":
                case "br":
                case "col":
                case "command":
                case "embed":
                case "hr":
                case "img":
                case "input":
                case "keygen":
                case "link":
                case "menuitem":
                case "meta":
                case "param":
                case "source":
                case "track":
                case "wbr":
                    return true;
            }
            return false;
        }
        private void ToHtmlDriver(StringBuilder builder)
        {
            builder.Append("<");
            builder.Append(tagType);
            foreach (var prop in properties)
            {
                builder.Append(" ");
                builder.Append(prop.Key);
                builder.Append("=\"");
                builder.Append(prop.Value);
                builder.Append("\"");
            }
            builder.Append(">");
            if (!IsSelfClosing(tagType))
            {
                if (type == BlockType.Container)
                {
                    foreach (var block in blocks)
                    {
                        block.ToHtmlDriver(builder);
                    }
                }
                else
                {
                    builder.Append(text);
                }
                builder.Append("</");
                builder.Append(tagType);
                builder.Append(">");
            }
        }
        public string ToHtml()
        {
            var builder = new StringBuilder();
            ToHtmlDriver(builder);
            return builder.ToString();
        }
    }
}
