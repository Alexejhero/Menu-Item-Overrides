using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MenuItemOverrides
{
    internal static class Config
    {
        private static string ConfigParentPath => Path.Combine("ProjectSettings", "Packages");
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
            string configParentPath = ConfigParentPath;
            string configPath = ConfigPath;

            if (File.Exists(configParentPath)) throw new Exception("ProjectSettings/Packages must be a directory but it is a file");
            if (!Directory.Exists(configParentPath)) Directory.CreateDirectory(configParentPath);

            if (!File.Exists(configPath)) File.WriteAllText(configPath, "");

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
