using System.Collections.Generic;
using System.Text;

namespace MenuItemOverrides
{
    internal class MenuItemOverride
    {
        public string OriginalPath = "";

        public bool Hide;

        public bool OverridePath;
        public string NewPath = "";

        public bool OverridePriority;
        public int NewPriority;
        public bool RelativeOffset = true;

        public OperationFlag Flag;

        public bool IsCategory => OriginalPath.EndsWith("/");

        public void Serialize(StringBuilder sb)
        {
            sb.AppendLine(OriginalPath);
            sb.AppendLine(NewPath);
            sb.AppendLine($",{Hide},{OverridePath},{OverridePriority},{NewPriority},{RelativeOffset}");
            sb.AppendLine();
            sb.AppendLine();
        }

        public static MenuItemOverride Deserialize(IList<string> lines)
        {
            string[] splits = lines[2].Split(',');

            return new MenuItemOverride
            {
                OriginalPath = lines[0],
                NewPath = lines[1],
                Hide = bool.Parse(splits[1]),
                OverridePath = bool.Parse(splits[2]),
                OverridePriority = bool.Parse(splits[3]),
                NewPriority = int.Parse(splits[4]),
                RelativeOffset = bool.Parse(splits[5])
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
