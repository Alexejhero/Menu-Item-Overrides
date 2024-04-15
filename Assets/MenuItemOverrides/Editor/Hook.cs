using UnityEditor;
using UnityEngine;

namespace MenuItemOverrides
{
    [InitializeOnLoad]
    internal sealed class Hook : ScriptableSingleton<Hook>
    {
        static Hook()
        {
            Patches.Patch();

            AssemblyReloadEvents.beforeAssemblyReload += () =>
            {
                PersistenceWindow window = CreateInstance<PersistenceWindow>();
                window.minSize = window.maxSize = Vector2.zero;
                window.ShowTab();
                window.position = new Rect(100000, 100000, 0, 0);
                window.Target = instance;
            };

            AssemblyReloadEvents.afterAssemblyReload += () =>
            {
                PersistenceWindow window = EditorWindow.GetWindow<PersistenceWindow>();
                if (window) window.Close();
            };
        }
    }
}
