using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEditor;

namespace MenuItemOverrides
{
    internal static class Patches
    {
        private static List<MenuItemOverride> _overrides;

        public static HashSet<string> Report;

        public static void Patch()
        {
            _overrides = Config.LoadPrefs();
            Report = new HashSet<string>();

            Harmony harmony = new(nameof(MenuItemOverrides));

            MethodInfo original = AccessTools.Method(AccessTools.TypeByName("System.Reflection.RuntimeMethodInfo"), nameof(MethodInfo.GetCustomAttributes), new[] {typeof(Type), typeof(bool)});
            MethodInfo postfix = AccessTools.Method(typeof(Patches), nameof(Postfix));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }

        private static void Postfix(object[] __result, [HarmonyArgument(0)] Type attributeType)
        {
            if (attributeType != typeof(MenuItem)) return;

            foreach (object obj in __result)
            {
                MenuItem item = (MenuItem) obj;
                UpdateMenuItem(item);
            }
        }

        private static void UpdateMenuItem(MenuItem item)
        {
            Report.Add($"{item.menuItem}, {item.priority}");

            foreach (MenuItemOverride o in _overrides)
            {
                if (o.IsCategory)
                {
                    if (!item.menuItem.StartsWith(o.OriginalPath)) continue;

                    if (o.Hide) item.menuItem = "CONTEXT/MenuItemHidden/" + item.menuItem;
                    else if (o.OverridePath && o.NewPath.EndsWith("/"))
                    {
                        int index = item.menuItem.IndexOf(o.OriginalPath, StringComparison.Ordinal);
                        item.menuItem = item.menuItem[..index] + o.NewPath + item.menuItem[(index + o.OriginalPath.Length)..];
                    }

                    if (o.OverridePriority) item.priority += o.NewPriority;
                }
                else
                {
                    if (item.menuItem != o.OriginalPath) continue;

                    if (o.Hide) item.menuItem = "CONTEXT/MenuItemHidden/" + item.menuItem;
                    else if (o.OverridePath) item.menuItem = o.NewPath;

                    if (o.OverridePriority) item.priority = (o.RelativeOffset ? item.priority : 0) + o.NewPriority;
                }
            }

            if (EditorConfig.DebugEnabled) item.menuItem += $" ({item.priority})";
        }
    }
}
