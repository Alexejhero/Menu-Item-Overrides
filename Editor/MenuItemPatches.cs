using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace MenuItemOverrides
{
    public static class MenuItemPatches
    {
        private static List<MenuItemOverride> _overrides;
        
        public static List<string> report;
        
        public static void Patch()
        {
            _overrides = MenuItemPersistentData.LoadPrefs();
            report = new List<string>();

            Harmony harmony = new(nameof(MenuItemPatches));
            
            ConstructorInfo original = AccessTools.GetDeclaredConstructors(typeof(UnityEditor.MenuItem)).OrderByDescending(c => c.GetParameters().Length).First();
            MethodInfo postfix = AccessTools.Method(typeof(MenuItemPatches), nameof(MenuItemPostfix));
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }
        
        // ReSharper disable once InconsistentNaming
        private static void MenuItemPostfix(UnityEditor.MenuItem __instance)
        {
            report.Add($"{__instance.menuItem}, {__instance.priority}");
            
            foreach (MenuItemOverride o in _overrides)
            {
                if (o.IsRealCategory)
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