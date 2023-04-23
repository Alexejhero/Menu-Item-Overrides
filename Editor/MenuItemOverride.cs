using System.Collections.Generic;
using System.Text;

namespace MenuItemOverrides
{
    internal class MenuItemOverride
    {
        public string originalPath = "";

        public bool hide;

        public bool overridePath;
        public string newPath = "";

        public bool overridePriority;
        public int newPriority;
        public bool relativeOffset = true;

        public OperationFlag flag;

        public bool IsCategory => originalPath.EndsWith("/");

        public void Serialize(StringBuilder sb)
        {
            sb.AppendLine(originalPath);
            sb.AppendLine(newPath);
            sb.AppendLine($",{hide},{overridePath},{overridePriority},{newPriority},{relativeOffset}");
            sb.AppendLine();
            sb.AppendLine();
        }

        public static MenuItemOverride Deserialize(IList<string> lines)
        {
            string[] splits = lines[2].Split(',');

            return new MenuItemOverride
            {
                originalPath = lines[0],
                newPath = lines[1],
                hide = bool.Parse(splits[1]),
                overridePath = bool.Parse(splits[2]),
                overridePriority = bool.Parse(splits[3]),
                newPriority = int.Parse(splits[4]),
                relativeOffset = bool.Parse(splits[5])
            };
        }

        public enum OperationFlag
        {
            None,
            MoveUp,
            MoveDown,
            Remove
        }
    }
}
