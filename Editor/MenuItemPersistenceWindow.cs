using UnityEditor;
using UnityEngine;

namespace MenuItemOverrides
{
    public class MenuItemPersistenceWindow : EditorWindow
    {
        public Object Target { get; set; }

        private void OnGUI()
        {
            MenuItemHook target = Target as MenuItemHook;
            if (!target) return;
            target!.hideFlags = target.hideFlags;
        }
    }
}