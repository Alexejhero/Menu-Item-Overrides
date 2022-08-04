using UnityEditor;
using UnityEngine;

namespace MenuItemOverrides
{
    [InitializeOnLoad]
    [FilePath("Packages/MenuItemOverrides/Editor/Hook.asset", FilePathAttribute.Location.ProjectFolder)]
    public class MenuItemHook : ScriptableSingleton<MenuItemHook>
    {
        static MenuItemHook()
        {
            MenuItemPatches.Patch();
            
            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                MenuItemPersistenceWindow window = CreateInstance<MenuItemPersistenceWindow>();
                window.minSize = window.maxSize = Vector2.zero;
                window.ShowTab();
                window.position = new Rect(100000, 100000, 0, 0);
                window.Target = instance;
            };

            AssemblyReloadEvents.afterAssemblyReload += () =>
            {
                MenuItemPersistenceWindow window = EditorWindow.GetWindow<MenuItemPersistenceWindow>();
                if (window) window.Close();
            };
        }
    }
}
