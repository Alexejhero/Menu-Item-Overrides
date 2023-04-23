using UnityEditor;

namespace MenuItemOverrides
{
    internal static class EditorConfig
    {
        private const string DEBUG_ENABLED_EDITORPREFS_KEY = "MenuItemOverrides_DebugEnabled";
        private static bool? _debugEnabled;
        public static bool DebugEnabled
        {
            get => _debugEnabled ??= EditorPrefs.GetBool(DEBUG_ENABLED_EDITORPREFS_KEY, false);
            set => EditorPrefs.SetBool(DEBUG_ENABLED_EDITORPREFS_KEY, (_debugEnabled = value).Value);
        }
    }
}
