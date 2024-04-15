using System.Collections.Generic;
using System.Text;

namespace MenuItemOverrides
{
    internal sealed class MenuItemOverride
    {
        public string OriginalPath { get; set; } = "";

        public bool Hide { get; set; }

        public bool OverridePath { get; set; }
        public string NewPath { get; set; } = "";

        public bool OverridePriority { get; set; }
        public int NewPriority { get; set; }
        public bool RelativeOffset { get; set; } = true;

        public OperationFlag flag;

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
