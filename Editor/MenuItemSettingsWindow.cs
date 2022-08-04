using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MenuItemOverrides
{
    public class MenuItemSettingsWindow : EditorWindow
    {
        private static string ReportPath => Path.Combine("Library", "MenuItemReport.txt");

        private List<MenuItemOverride> _saved;
        private List<MenuItemOverride> _edited;
        
        private Vector2 _scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            _saved = MenuItemPersistentData.LoadPrefs();
            _edited = new List<MenuItemOverride>(_saved);
        }

        [UnityEditor.MenuItem("Tools/Menu Item Overrides/Configuration", priority = -1000)]
        public static void ShowWindow()
        {
            GetWindow<MenuItemSettingsWindow>("Menu Item Overrides");
        }

        [UnityEditor.MenuItem("Tools/Menu Item Overrides/See Report...", priority = -999)]
        public static void SeeReport()
        {
            File.WriteAllLines(ReportPath, MenuItemPatches.report.OrderBy(a => a));
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

                if (o.ShouldBeRemoved)
                {
                    _edited.RemoveAt(i);
                    i--;
                }

                if (o.MoveDirection < 0)
                {
                    _edited.RemoveAt(i);
                    _edited.Insert(i - 1, o);
                    o.Move(0);
                }
                else if (o.MoveDirection > 0)
                {
                    _edited.RemoveAt(i);
                    _edited.Insert(i + 1, o);
                    o.Move(0);
                }
            }
            
            float oldHeight = EditorStyles.helpBox.fixedHeight;
            FontStyle oldFontStyle = EditorStyles.helpBox.fontStyle;
            EditorStyles.helpBox.fixedHeight = 30;
            EditorStyles.helpBox.fontStyle = FontStyle.Bold;
            EditorGUILayout.HelpBox("Modifying priority offsets for categories will only\ntake effect once the editor has been restarted.", MessageType.Warning);
            EditorStyles.helpBox.fixedHeight = oldHeight;
            EditorStyles.helpBox.fontStyle = oldFontStyle;       
            GUILayout.Space(15);

            
            foreach (MenuItemOverride o in _edited)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                
                if (_edited.IndexOf(o) == 0) EditorGUI.BeginDisabledGroup(true);
                if (GUILayout.Button("↑", GUILayout.MaxWidth(25)))
                {
                    GUI.FocusControl(null);
                    o.Move(-1);
                }
                if (_edited.IndexOf(o) == 0) EditorGUI.EndDisabledGroup();
                
                if (_edited.IndexOf(o) == _edited.Count - 1) EditorGUI.BeginDisabledGroup(true);
                if (GUILayout.Button("↓", GUILayout.MaxWidth(25)))
                {
                    GUI.FocusControl(null);
                    o.Move(1);
                }
                if (_edited.IndexOf(o) == _edited.Count - 1) EditorGUI.EndDisabledGroup();
                
                o.originalPath = EditorGUILayout.TextField(o.originalPath, GUILayout.MinWidth(50));

                if (!o.originalPath.EndsWith("/"))
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUIContent content = new("C", "Path must end with '/' in order to match an entire category");
                    GUILayout.Toggle(false, content, GUI.skin.button, GUILayout.MaxWidth(25));
                    EditorGUI.EndDisabledGroup();
                }
                else if (o.originalPath.Contains("^^^"))
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUIContent content = new("C", "Path must not contain '^^^' in order to match an entire category");
                    GUILayout.Toggle(false, content, GUI.skin.button, GUILayout.MaxWidth(25));
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    GUIContent content = new("C", "Match an entire category of menu items, instead of a single one");
                    o.category = GUILayout.Toggle(o.category, content, GUI.skin.button, GUILayout.MaxWidth(25));
                }

                if (GUILayout.Button(new GUIContent("X", "Remove override"), GUILayout.MaxWidth(25)))
                {
                    GUI.FocusControl(null);
                    o.Remove();
                }
                EditorGUILayout.EndHorizontal();

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.BeginHorizontal();
                    o.hide = EditorGUILayout.ToggleLeft("Hide", o.hide, GUILayout.MaxWidth(125));
                    EditorGUILayout.EndHorizontal();

                    if (!o.hide)
                    {
                        EditorGUILayout.BeginHorizontal();
                        o.overridePath = EditorGUILayout.ToggleLeft("Override Path", o.overridePath, GUILayout.MaxWidth(125));
                        if (!o.overridePath) EditorGUI.BeginDisabledGroup(true);
                        o.newPath = EditorGUILayout.TextField(o.newPath);
                        if (o.IsRealCategory && o.overridePath && !o.newPath.EndsWith("/"))
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            GUIContent content = new("!", "New path must end with '/' in order to be applied to a category");
                            Color backgroundColor = GUI.backgroundColor;
                            GUI.backgroundColor = Color.red;
                            GUILayout.Button(content, new GUIStyle(GUI.skin.button)
                            {
                                normal =
                                {
                                    textColor = Color.white
                                },
                                fontStyle = FontStyle.Bold
                            }, GUILayout.MaxWidth(25));
                            GUI.backgroundColor = backgroundColor;
                            EditorGUI.EndDisabledGroup();
                        }
                        if (!o.overridePath) EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();

                        string strContent = !o.IsRealCategory 
                            ? !o.relativeOffset 
                                ? "Override Priority"
                                : "Offset Priority"
                            : "Offset Priorities";
                        EditorGUILayout.BeginHorizontal();
                        o.overridePriority = EditorGUILayout.ToggleLeft(strContent, o.overridePriority, GUILayout.MaxWidth(125));
                        if (!o.overridePriority) EditorGUI.BeginDisabledGroup(true);
                        o.newPriority = EditorGUILayout.IntField(o.newPriority);
                        if (!o.overridePriority) EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();

                        if (!o.IsRealCategory)
                        {
                            if (!o.overridePriority) EditorGUI.BeginDisabledGroup(true);
                            o.relativeOffset = EditorGUILayout.ToggleLeft("Relative Offset", o.relativeOffset, GUILayout.MaxWidth(125));
                            if (!o.overridePriority) EditorGUI.EndDisabledGroup();
                        }
                    }
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(15);
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
                _saved = MenuItemPersistentData.LoadPrefs();
                _edited = new List<MenuItemOverride>(_saved);
            }
            if (GUILayout.Button("Save"))
            {
                GUI.FocusControl(null);
                _saved = new List<MenuItemOverride>(_edited);
                MenuItemPersistentData.SavePrefs(_saved);
            }
            if (GUILayout.Button("Save and Reload"))
            {
                GUI.FocusControl(null);
                _saved = new List<MenuItemOverride>(_edited);
                MenuItemPersistentData.SavePrefs(_saved);
                
                EditorUtility.RequestScriptReload();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }
    }
}
