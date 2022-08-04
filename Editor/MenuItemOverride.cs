using System.Collections.Generic;
using System.Text;

namespace MenuItemOverrides
{
    public class MenuItemOverride
    {
        public string originalPath = "";
        public bool category;
        public bool IsRealCategory => category && originalPath.EndsWith("/") && !originalPath.Contains("^^^");
        
        public bool hide;

        public bool overridePath;
        public string newPath = "";

        public bool overridePriority;
        public int newPriority;
        public bool relativeOffset = true;

        public bool ShouldBeRemoved { get; private set; }
        public void Remove() => ShouldBeRemoved = true;
        
        public int MoveDirection { get; private set; }
        public void Move(int direction) => MoveDirection = direction;

        public void Serialize(StringBuilder sb)
        {
            sb.AppendLine(originalPath);
            sb.AppendLine(newPath);
            sb.AppendLine($"{category},{hide},{overridePath},{overridePriority},{newPriority},{relativeOffset}");
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
                category = bool.Parse(splits[0]),
                hide = bool.Parse(splits[1]),
                overridePath = bool.Parse(splits[2]),
                overridePriority = bool.Parse(splits[3]),
                newPriority = int.Parse(splits[4])
            };
        }
    }
}
