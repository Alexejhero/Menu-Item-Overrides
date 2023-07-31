using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MenuItemOverrides
{
    internal static class Config
    {
        private static string ConfigPath => Path.Combine("ProjectSettings", "Packages", "MenuItemOverrides.txt");

        public static void SavePrefs(IEnumerable<MenuItemOverride> overrides)
        {
            StringBuilder sb = new();

            foreach (MenuItemOverride o in overrides)
            {
                o.Serialize(sb);
            }

            File.WriteAllText(ConfigPath, sb.ToString());
        }

        public static List<MenuItemOverride> LoadPrefs()
        {
            if (!File.Exists(ConfigPath)) File.WriteAllText(ConfigPath, "");

            List<MenuItemOverride> overrides = new();

            try
            {
                List<string> lines = File.ReadAllLines(ConfigPath).ToList();

                while (lines.Count >= 5)
                {
                    overrides.Add(MenuItemOverride.Deserialize(lines));
                    lines.RemoveRange(0, 5);
                }
            }
            catch
            {
                // ignored
            }

            return overrides;
        }
    }
}
