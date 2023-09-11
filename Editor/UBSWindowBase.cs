using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace UBS
{
	public class UBSWindowBase : EditorWindow
	{
		static Texture2D _bigLogo;
		protected static Texture2D BigLogo
		{
			get
			{
				if(_bigLogo == null)
				{

                    var assets = AssetDatabase.FindAssets("ubs_logo_256");
                    if(assets.Length > 0)
                        _bigLogo = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]), typeof(Texture2D));
				    //mBigLogo = (Texture2D) AssetDatabase.LoadAssetAtPath(Styles._imagePath + "ubs_logo_256.png", typeof(Texture2D));
				}
				return _bigLogo;
			}
		}
		

		protected virtual void OnDisable()
		{
			_bigLogo = null;
		}
		
		protected virtual void OnGUI()
		{
            if (BigLogo != null)
            {
                GUI.color = new Color(1, 1, 1, 0.3f);
                GUI.DrawTexture(new Rect(position.width - 160, position.height - 160, 256, 256), BigLogo);
                GUI.color = Color.white;
            }
		}

        private static Vector2 _mouseStartPosition;
        public static void DrawBuildProcessEntry(BuildCollection data, BuildProcess e, bool odd, ref int selectedCount, bool editable, ref bool selected)
        {
            var mainstyle = selected ? Styles.SelectedListEntry : odd
                ? UBS.Styles.SelectableListEntryOdd
                : UBS.Styles.SelectableListEntry; 
            
            GUILayout.BeginHorizontal(mainstyle);
            {
                Texture2D platformIcon = GetPlatformIcon(e.Platform);
                GUILayout.Box(platformIcon, UBS.Styles.Icon);
                
                var sel = GUILayout.Toggle(e.Selected,"", Styles.Toggle);
                if (editable && sel != e.Selected)
                {
                    e.Selected = sel;
                    EditorUtility.SetDirty(data);
                }
                selectedCount += e.Selected ? 1 : 0;
                
                GUILayout.Label(e.Name, odd ? UBS.Styles.SelectableListEntryTextOdd : UBS.Styles.SelectableListEntryText);
                GUILayout.FlexibleSpace();
                
            }
            GUILayout.EndHorizontal();
            var rect = GUILayoutUtility.GetLastRect();
            
            if (Event.current.isMouse &&
                Event.current.type == EventType.MouseDown)
            {
                _mouseStartPosition = Event.current.mousePosition;
            }
            if (rect.Contains(Event.current.mousePosition) && rect.Contains(_mouseStartPosition) &&
                Event.current.isMouse && Event.current.type == EventType.MouseUp)
            {
                selected = true;
            }


            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                Rect r = GUILayoutUtility.GetLastRect();
                if (r.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(e.Name), false, null);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Open target folder"), false, () => {

                        DirectoryInfo di = new DirectoryInfo(UBS.Helpers.GetAbsolutePathRelativeToProject(e.OutputPath));

                        string path;
                        if ((di.Attributes & FileAttributes.Directory) != 0)
                            path = di.FullName;
                        else
                            path = di.Parent.FullName;

                        EditorUtility.RevealInFinder(path);
                    });
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Build and run"), false, BuildAndRun, new BuildAndRunUserData(data, e));
                    menu.AddItem(new GUIContent("Build"), false, Build, new BuildAndRunUserData(data, e));

                    menu.ShowAsContext();
                }
            }
        }

        static Texture2D GetPlatformIcon(BuildTarget platform)
        {
            switch (platform)
            {

                case BuildTarget.iOS:
                    return UBS.Styles.IconIOS;
                case BuildTarget.Android:
                    return UBS.Styles.IconAndroid;
                case BuildTarget.StandaloneWindows:
                    return UBS.Styles.IconWindows;
                case BuildTarget.StandaloneWindows64:
                    return UBS.Styles.IconWindows;
                case BuildTarget.StandaloneLinux64:
                    return UBS.Styles.IconLinux;
                case BuildTarget.StandaloneOSX:
                    return UBS.Styles.IconMacOs;
                default:
                    return new Texture2D(0, 0);
            }
        }

        static void Build(object process)
        {
            BuildAndRunUserData data = process as BuildAndRunUserData;
            UBSBuildWindow.Create(data.Collection, data.Process as BuildProcess, false);
        }
        static void BuildAndRun(object process)
        {
            BuildAndRunUserData data = process as BuildAndRunUserData;
            UBSBuildWindow.Create(data.Collection, data.Process, true);
        }

        internal class BuildAndRunUserData
        {
            internal BuildCollection Collection
            {
                get;
                private set;
            }
            internal BuildProcess Process
            {
                get;
                private set;
            }

            internal BuildAndRunUserData(BuildCollection collection, BuildProcess process)
            {
                Process = process;
                Collection = collection;
            }
        }
    }
}
