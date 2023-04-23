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

        public static HashSet<string> report;

        public static void Patch()
        {
            _overrides = Config.LoadPrefs();
            report = new HashSet<string>();

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
            report.Add($"{item.menuItem}, {item.priority}");

            foreach (MenuItemOverride o in _overrides)
            {
                if (o.IsCategory)
                {
                    if (!item.menuItem.StartsWith(o.originalPath)) continue;

                    if (o.hide) item.menuItem = "CONTEXT/MenuItemHidden/" + item.menuItem;
                    else if (o.overridePath && o.newPath.EndsWith("/"))
                    {
                        int index = item.menuItem.IndexOf(o.originalPath, StringComparison.Ordinal);
                        item.menuItem = item.menuItem[..index] + o.newPath + item.menuItem[(index + o.originalPath.Length)..];
                    }

                    if (o.overridePriority) item.priority += o.newPriority;
                }
                else
                {
                    if (item.menuItem != o.originalPath) continue;

                    if (o.hide) item.menuItem = "CONTEXT/MenuItemHidden/" + item.menuItem;
                    else if (o.overridePath) item.menuItem = o.newPath;

                    if (o.overridePriority) item.priority = (o.relativeOffset ? item.priority : 0) + o.newPriority;
                }
            }
        }
    }
}
