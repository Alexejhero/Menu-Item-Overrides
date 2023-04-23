using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MenuItemOverrides
{
    internal class SettingsWindow : EditorWindow
    {
        private static string ReportPath => Path.Combine("Library", "MenuItemReport.txt");

        private List<MenuItemOverride> _saved;
        private List<MenuItemOverride> _edited;

        private Vector2 _scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            _saved = Config.LoadPrefs();
            _edited = new List<MenuItemOverride>(_saved);
        }

        [MenuItem("Tools/Menu Item Overrides/Configuration", priority = -1000)]
        public static void ShowWindow()
        {
            GetWindow<SettingsWindow>("Menu Item Overrides");
        }

        [MenuItem("Tools/Menu Item Overrides/See Report...", priority = -999)]
        public static void SeeReport()
        {
            File.WriteAllLines(ReportPath, Patches.report.OrderBy(a => a));
            Process.Start(ReportPath);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();

            GUILayout.Label(" Menu Item Overrides", new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fixedHeight = 22,
                fontSize = 14
            });

            GUILayout.Space(10);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            for (int i = 0; i < _edited.Count; i++)
            {
                MenuItemOverride o = _edited[i];

                switch (o.flag)
                {
                    case MenuItemOverride.OperationFlag.MoveUp:
                        _edited.RemoveAt(i);
                        _edited.Insert(i - 1, o);
                        break;
                    case MenuItemOverride.OperationFlag.MoveDown:
                        _edited.RemoveAt(i);
                        _edited.Insert(i + 1, o);
                        break;
                    case MenuItemOverride.OperationFlag.Remove:
                        _edited.RemoveAt(i);
                        i--;
                        break;
                }

                o.flag = MenuItemOverride.OperationFlag.None;
            }

            for (int index = 0; index < _edited.Count; index++)
            {
                MenuItemOverride o = _edited[index];
                DrawItem(o, index);
            }

            if (GUILayout.Button("Add"))
            {
                GUI.FocusControl(null);
                _edited.Add(new MenuItemOverride());
            }

            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset"))
            {
                GUI.FocusControl(null);
                _saved = Config.LoadPrefs();
                _edited = new List<MenuItemOverride>(_saved);
            }
            if (GUILayout.Button("Save"))
            {
                GUI.FocusControl(null);
                _saved = new List<MenuItemOverride>(_edited);
                Config.SavePrefs(_saved);
            }
            if (GUILayout.Button("Save and Reload"))
            {
                GUI.FocusControl(null);
                _saved = new List<MenuItemOverride>(_edited);
                Config.SavePrefs(_saved);

                EditorUtility.RequestScriptReload();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawItem(MenuItemOverride item, int index)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                DrawItemHeader(item, index);

                if (!item.hide)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        DrawItemPath(item);
                        DrawItemPriority(item);
                    }
                }
            }

            EditorGUILayout.Space();
        }

        private void DrawItemHeader(MenuItemOverride item, int index)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(index == 0))
                {
                    if (GUILayout.Button("↑", GUILayout.MaxWidth(25)))
                    {
                        GUI.FocusControl(null);
                        item.flag = MenuItemOverride.OperationFlag.MoveUp;
                    }
                }

                using (new EditorGUI.DisabledScope(index == _edited.Count - 1))
                {
                    if (GUILayout.Button("↓", GUILayout.MaxWidth(25)))
                    {
                        GUI.FocusControl(null);
                        item.flag = MenuItemOverride.OperationFlag.MoveDown;
                    }
                }

                item.originalPath = TextFieldWithPlaceholder(item.originalPath, "AnnoyingPackage/", item.GetHashCode() + "_old", GUILayout.MinWidth(50)).Trim();

                using (new EditorGUI.DisabledScope(true))
                {
                    GUIContent itemContent = new("/*", "This override matches a single item. To match all items in a submenu, add a trailing '/' to the path.");
                    GUIContent categoryContent = new("/*", "This override matches an entire submenu of items instead of a single one. To disable this behaviour, remove the trailing '/' from the path.");
                    GUILayout.Toggle(item.IsCategory, item.IsCategory ? categoryContent : itemContent, GUI.skin.button, GUILayout.MaxWidth(25));
                }

                GUIContent hideContent = new("H", "Completely hide this menu item");
                item.hide = GUILayout.Toggle(item.hide, hideContent, GUI.skin.button, GUILayout.MaxWidth(25));

                GUILayout.Label("|", GUILayout.MaxWidth(7.2f));

                if (GUILayout.Button(new GUIContent("X", "Remove override"), GUILayout.MaxWidth(25)))
                {
                    GUI.FocusControl(null);
                    item.flag = MenuItemOverride.OperationFlag.Remove;
                }
            }
        }

        private void DrawItemPath(MenuItemOverride item)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                item.overridePath = EditorGUILayout.ToggleLeft("Override Path", item.overridePath, GUILayout.MaxWidth(150));
                using (new EditorGUI.DisabledScope(!item.overridePath))
                {
                    item.newPath = TextFieldWithPlaceholder(item.newPath, "Tools/LessAnnoyingPackage/", item.GetHashCode() + "_new").Trim();
                    if (!item.overridePath || item.IsCategory == item.newPath.EndsWith("/")) return;

                    using (new EditorGUI.DisabledScope(true))
                    {
                        GUIContent originalCategoryContent = new("!", "New path must end with '/' in order to be applied to an entire submenu.");
                        GUIContent originalItemContent = new("!", "New path must not end with '/' in order to be applied to a single item.");

                        Color backgroundColor = GUI.backgroundColor;
                        GUI.backgroundColor = Color.red;
                        GUILayout.Button(item.IsCategory ? originalCategoryContent : originalItemContent, new GUIStyle(GUI.skin.button)
                        {
                            normal =
                            {
                                textColor = Color.white
                            },
                            fontStyle = FontStyle.Bold
                        }, GUILayout.MaxWidth(25));
                        GUI.backgroundColor = backgroundColor;
                    }
                }
            }
        }

        private void DrawItemPriority(MenuItemOverride item)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                item.overridePriority = EditorGUILayout.ToggleLeft(item.IsCategory ? "Override Priorities" : "Override Priority", item.overridePriority, GUILayout.MaxWidth(150));
                using (new EditorGUI.DisabledScope(!item.overridePriority))
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (item.IsCategory)
                    {
                        GUILayout.Space(15);
                        using (new EditorGUI.DisabledScope(true))
                        {
                            GUILayout.Button(new GUIContent("+=", "Update submenu item priorities by adding an offset to their current values."), GUILayout.MaxWidth(25));
                        }

                        GUILayout.Space(-20);

                        item.newPriority = EditorGUILayout.IntField(item.newPriority);

                        using (new EditorGUI.DisabledScope(true))
                        {
                            GUIContent content = new("!", "Modifying priority offsets for submenus will only take effect once the editor has been restarted or the submenu has been moved, or hidden and unhidden.");

                            Color backgroundColor = GUI.backgroundColor;
                            GUI.backgroundColor = Color.yellow;
                            GUILayout.Button(content, new GUIStyle(GUI.skin.button)
                            {
                                normal =
                                {
                                    textColor = Color.white
                                },
                                fontStyle = FontStyle.Bold
                            }, GUILayout.MaxWidth(25));
                            GUI.backgroundColor = backgroundColor;
                        }
                    }
                    else
                    {
                        GUILayout.Space(15);
                        if (item.relativeOffset)
                        {
                            bool change = GUILayout.Button(new GUIContent("+=", "Update item priority by adding an offset to it. Click to change behaviour."), GUILayout.MaxWidth(25));
                            if (change) item.relativeOffset = false;
                        }
                        else
                        {
                            bool change = GUILayout.Button(new GUIContent("=", "Update item priority by overriding it entirely. Click to change behaviour."), GUILayout.MaxWidth(25));
                            if (change) item.relativeOffset = true;
                        }

                        GUILayout.Space(-20);

                        item.newPriority = EditorGUILayout.IntField(item.newPriority);
                    }
                }
            }
        }

        private string TextFieldWithPlaceholder(string text, string placeholder, string controlId, params GUILayoutOption[] options)
        {
            GUIStyle placeholderStyle = new GUIStyle(EditorStyles.textField);
            placeholderStyle.normal.textColor = Color.gray;
            placeholderStyle.hover.textColor = Color.gray;

            bool focused = EditorGUIUtility.editingTextField && GUI.GetNameOfFocusedControl().StartsWith(controlId);
            bool previouslyUnfocused = focused && GUI.GetNameOfFocusedControl().EndsWith("_prev");

            if (string.IsNullOrEmpty(text) && !focused)
            {
                GUI.SetNextControlName(controlId + "_prev");
                EditorGUILayout.TextField(placeholder, placeholderStyle, options);
            }
            else
            {
                if (previouslyUnfocused) EditorGUI.FocusTextInControl(controlId);
                GUI.SetNextControlName(controlId);
                text = EditorGUILayout.TextField(text, options);
                if (previouslyUnfocused) EditorGUI.FocusTextInControl(controlId);
            }

            return text;
        }
    }
}
