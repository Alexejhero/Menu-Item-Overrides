using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEditor;

namespace MenuItemOverrides
{
    internal static class Patches
    {
        private static List<MenuItemOverride> _overrides;

        public static List<string> report;

        public static void Patch()
        {
            _overrides = Config.LoadPrefs();
            report = new List<string>();

            Harmony harmony = new(nameof(MenuItemOverrides));

            ConstructorInfo original = AccessTools.GetDeclaredConstructors(typeof(MenuItem))
                .OrderByDescending(c => c.GetParameters().Length).First();
            MethodInfo postfix = AccessTools.Method(typeof(Patches), nameof(MenuItemPostfix));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }

        private static void MenuItemPostfix(MenuItem __instance)
        {
            report.Add($"{__instance.menuItem}, {__instance.priority}");

            foreach (MenuItemOverride o in _overrides)
            {
                if (o.IsCategory)
                {
                    if (__instance.menuItem.StartsWith(o.originalPath))
                    {
                        if (o.hide) __instance.menuItem = "CONTEXT/MenuItemHidden/" + __instance.menuItem;
                        else if (o.overridePath && o.newPath.EndsWith("/"))
                            __instance.menuItem = ("^^^" + __instance.menuItem).Replace("^^^" + o.originalPath, o.newPath);

                        if (o.overridePriority)
                        {
                            __instance.priority += o.newPriority;
                        }
                    }
                }
                else
                {
                    if (__instance.menuItem == o.originalPath)
                    {
                        if (o.hide) __instance.menuItem = "CONTEXT/MenuItemHidden/" + __instance.menuItem;
                        else if (o.overridePath) __instance.menuItem = o.newPath;

                        if (o.overridePriority)
                        {
                            if (o.relativeOffset)
                            {
                                __instance.priority += o.newPriority;
                            }
                            else
                            {
                                __instance.priority = o.newPriority;
                            }
                        }
                    }
                }
            }
        }
    }
}
