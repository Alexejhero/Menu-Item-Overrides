using UnityEditor;
using UnityEngine;

namespace MenuItemOverrides
{
    internal sealed class PersistenceWindow : EditorWindow
    {
        public Object Target { get; set; }

        private void OnGUI()
        {
            Hook target = Target as Hook;
            if (!target) return;
            target!.hideFlags = target.hideFlags;
        }
    }
}
