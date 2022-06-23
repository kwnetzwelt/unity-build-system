using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace UBS
{
	internal static class Styles
	{
		public const string _imagePath = "Packages/com.kwnetzwelt.ubs/Editor/images/";

		static Dictionary<long,Texture2D>_textures = new Dictionary<long, Texture2D>();
		
		static Texture2D GetTexture(long colorRGBA)
		{
			if (_textures.ContainsKey(colorRGBA) && _textures [colorRGBA] != null)
				return _textures [colorRGBA];
				
			Color32 c = GetColor(colorRGBA);
			
			var tmp = new Texture2D(4, 4);
			for (int x = 0; x < 4; x++)
				for (int y = 0; y < 4; y++)
					tmp.SetPixel(x, y, c);
			tmp.Apply();
			tmp.Compress(true);
			
			_textures [colorRGBA] = tmp;
			
			return tmp; 
		}
		
		static Color32 GetColor(long colorRGBA)
		{
			byte r = (byte)((colorRGBA & 0xff000000) >> 24);
			byte g = (byte)((colorRGBA & 0xff0000) >> 16);
			byte b = (byte)((colorRGBA & 0xff00) >> 8);
			byte a = (byte)((colorRGBA & 0xff));
			
			Color32 c = new Color32(r, g, b, a);
			return c;
		}
		
		static GUIStyle _list;
		public static GUIStyle List
		{
			get
			{
				if (_list == null)
				{
					_list = new GUIStyle();
					_list.normal.background = GetTexture(0x424646ff);
					_list.alignment = TextAnchor.UpperLeft;
					_list.stretchWidth = true;
					_list.stretchHeight = true;
				}
				return _list;
			}
		}

		static GUIStyle _boldKey;
		public static GUIStyle BoldKey
		{
			get
			{
				if (_boldKey == null)
				{
					_boldKey = new GUIStyle();
					if (EditorGUIUtility.isProSkin)
						_boldKey.normal.textColor = GetColor(0xffffffff);
					else
						_boldKey.normal.textColor = GetColor(0x000000ff);
					_boldKey.fontStyle = FontStyle.Bold;
					_boldKey.alignment = TextAnchor.MiddleLeft;
					
					
				}
				return _boldKey;
			}
		}

		static GUIStyle _buildProcessEditorBackground;
		public static GUIStyle BuildProcessEditorBackground
		{
			get
			{
				if (_buildProcessEditorBackground == null)
				{
					_buildProcessEditorBackground = new GUIStyle();
					if (EditorGUIUtility.isProSkin)
						_buildProcessEditorBackground.normal.background = GetTexture(0x444444aa);
					else
						_buildProcessEditorBackground.normal.background = GetTexture(0xeeeeeeaa);
				}
				return _buildProcessEditorBackground;
			}
		}

		static Texture2D _gear;
		public static Texture2D Gear
		{
			get
			{
				if (_gear == null)
				{
					_gear = (Texture2D)AssetDatabase.LoadAssetAtPath(Styles._imagePath + "gear.png", typeof(Texture2D));

				}
				return _gear;
			}
		}
		static Texture2D _iconIOS;
		public static Texture2D IconIOS
		{
			get
			{
				if (_iconIOS == null)
				{
					var path = Styles._imagePath + "icons/ico_ios.png";
					_iconIOS = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
				}
				return _iconIOS;
			}
		}
		static Texture2D _iconAndroid;
		public static Texture2D IconAndroid
		{
			get
			{
				if (_iconAndroid == null)
				{
					_iconAndroid = (Texture2D)AssetDatabase.LoadAssetAtPath(Styles._imagePath + "icons/ico_android.png", typeof(Texture2D));
					
				}
				return _iconAndroid;
			}
		}
		static Texture2D _iconWindows;
		public static Texture2D IconWindows
		{ 
			get
			{
				if (_iconWindows == null)
				{
					_iconWindows = (Texture2D)AssetDatabase.LoadAssetAtPath(Styles._imagePath + "icons/ico_windows.png", typeof(Texture2D));
					
				}
				return _iconWindows;
			}
		}
		static GUIStyle _icon;
		public static GUIStyle Icon
		{ 
			get
			{
				if (_icon == null)
				{
					_icon = new GUIStyle();
					_icon.fixedWidth = 20;
					_icon.fixedHeight = 20;
					_icon.contentOffset = new Vector2(-8f, -2f);
				}
				return _icon;
			}
		}

		static GUIStyle _normalValue;
		public static GUIStyle NormalValue
		{
			get
			{
				if (_normalValue == null)
				{
					_normalValue = new GUIStyle();
					if (EditorGUIUtility.isProSkin)
						_normalValue.normal.textColor = GetColor(0xffffffff);
					else
						_normalValue.normal.textColor = GetColor(0x000000ff);
					_normalValue.fontStyle = FontStyle.Normal;
					_normalValue.alignment = TextAnchor.MiddleLeft;
					
					
				}
				return _normalValue;
			}
		}

		static GUIStyle _selectableListEntryText;
		public static GUIStyle SelectableListEntryText
		{
			get
			{
				if (_selectableListEntryText == null)
				{
					_selectableListEntryText = new GUIStyle(List);
					_selectableListEntryText.normal.textColor = GetColor(0xb5b5b5ff);
                    _selectableListEntryText.normal.background = GetTexture(0x33353500);
					_selectableListEntryText.fontStyle = FontStyle.Bold;
                    _selectableListEntryText.stretchWidth = true;


                }
				return _selectableListEntryText;
			}
		}
		static GUIStyle _selectableListEntryTextOdd;
		public static GUIStyle SelectableListEntryTextOdd
		{
			get
			{
				if (_selectableListEntryTextOdd == null)
				{
					_selectableListEntryTextOdd = new GUIStyle(List);
					_selectableListEntryTextOdd.normal.textColor = GetColor(0xb5b5b5ff);
					_selectableListEntryTextOdd.normal.background = GetTexture(0x33353500);
					_selectableListEntryTextOdd.fontStyle = FontStyle.Bold;
                    _selectableListEntryTextOdd.stretchWidth = true;

                }
				return _selectableListEntryTextOdd;
			}
		}

        private static GUIStyle _toggle;

        public static GUIStyle Toggle
        {
            get
            {
                if (_toggle == null)
                {
                    _toggle = new GUIStyle(UnityEditor.EditorStyles.toggle);
                    _toggle.stretchWidth = false;
                    _toggle.fixedWidth = 17;
                    _toggle.contentOffset = Vector2.zero;
                }

                return _toggle;
            }
        }
        
		static GUIStyle _selectableListEntry;
		public static GUIStyle SelectableListEntry
		{
			get
			{
				if (_selectableListEntry == null)
				{
					_selectableListEntry = new GUIStyle(List);
					_selectableListEntry.stretchHeight = false;
					_selectableListEntry.normal.textColor = GetColor(0xb5b5b5ff);
					_selectableListEntry.fontStyle = FontStyle.Bold;
					_selectableListEntry.padding = new RectOffset(20, 20, 10, 9);
					_selectableListEntry.border = new RectOffset(0, 0, 0, 1);
					
				}
				return _selectableListEntry;
			}
		}
		
		static GUIStyle _selectableListEntryOdd;
		public static GUIStyle SelectableListEntryOdd
		{
			get
			{
				if (_selectableListEntryOdd == null)
				{
					_selectableListEntryOdd = new GUIStyle(SelectableListEntry);
					_selectableListEntryOdd.normal.background = GetTexture(0x333535ff);
					
				}
				return _selectableListEntryOdd;
			}
		}

		static GUIStyle _toolButton;
		public static GUIStyle ToolButton
		{
			get
			{
				if (_toolButton == null)
				{
					_toolButton = new GUIStyle("TE toolbarbutton");

					_toolButton.fontSize = 24;
					_toolButton.fontStyle = FontStyle.Bold;
					_toolButton.fixedHeight = 28;
					_toolButton.padding = new RectOffset(5, 5, 5, 5);
					_toolButton.contentOffset = new Vector2(-1, 0);
					_toolButton.clipping = TextClipping.Overflow;

				}
				return _toolButton;
			}

		}
		
		static GUIStyle _selectedListEntry;
		public static GUIStyle SelectedListEntry
		{
			get
			{
				if (_selectedListEntry == null)
				{
					_selectedListEntry = new GUIStyle();
					_selectedListEntry.normal.background = GetTexture(0x1b76d1ff);
					_selectedListEntry.normal.textColor = GetColor(0xffffffff);
					_selectedListEntry.fontStyle = FontStyle.Bold;
					_selectedListEntry.padding = new RectOffset(20, 20, 10, 9);
					_selectedListEntry.border = new RectOffset(0, 0, 0, 1);
				}
				return _selectedListEntry;
			}
		}

		static GUIStyle _dirtySetting;
		public static GUIStyle DirtySetting
		{
			get
			{
				if (_dirtySetting == null)
				{
					_dirtySetting = new GUIStyle();
					_dirtySetting.normal.background = GetTexture(0x1b76d1ff);

				}
				return _dirtySetting;
			}
		}
		
		static GUIStyle _bigHint;
		public static GUIStyle BigHint
		{
			get
			{
				if (_bigHint == null)
				{
					_bigHint = new GUIStyle();
					_bigHint.alignment = TextAnchor.MiddleCenter;
					_bigHint.stretchWidth = true;
					_bigHint.stretchHeight = true;
					_bigHint.fontSize = 32;
					_bigHint.fontStyle = FontStyle.Bold;
					_bigHint.normal.textColor = GetColor(0x00000066);
				}
				return _bigHint;
			}
		}
		static GUIStyle _mediumHint;
		public static GUIStyle MediumHint
		{
			get
			{
				if (_mediumHint == null)
				{
					_mediumHint = new GUIStyle();
					_mediumHint.alignment = TextAnchor.MiddleCenter;
					_mediumHint.stretchWidth = true;
					_mediumHint.stretchHeight = false;
					_mediumHint.fontSize = 24;
					_mediumHint.fontStyle = FontStyle.Bold;
					_mediumHint.normal.textColor = GetColor(0x00000066);
				}
				return _mediumHint;
			}
		}
		
		static GUIStyle _detailsGroup;
		public static GUIStyle DetailsGroup
		{
			get
			{
				if (_detailsGroup == null)
				{
					_detailsGroup = new GUIStyle();
					_detailsGroup.alignment = TextAnchor.UpperLeft;
					_detailsGroup.margin = new RectOffset(2, 2, 2, 2);
					
				}
				return _detailsGroup;
			}
		}
		
		static GUIStyle _infoGroup;
		public static GUIStyle InfoGroup
		{
			get
			{
				if (_infoGroup == null)
				{
					_infoGroup = new GUIStyle();
					_infoGroup.alignment = TextAnchor.UpperLeft;
					_infoGroup.margin = new RectOffset(20, 20, 2, 10);
					_infoGroup.padding = new RectOffset(10, 10, 5, 5);
					_infoGroup.normal.background = GetTexture(0x00000066);
					
				}
				return _infoGroup;
			}
		}

		static GUIStyle _detailsGizmo;
		public static GUIStyle DetailsGizmo
		{
			get
			{
				if (_detailsGizmo == null)
				{
					_detailsGizmo = new GUIStyle();
					_detailsGizmo.alignment = TextAnchor.MiddleLeft;
					_detailsGizmo.fixedWidth = 32;
					_detailsGizmo.fixedHeight = 32;
					_detailsGizmo.margin = new RectOffset(10, 10, 0, 0);
					_detailsGizmo.normal.textColor = Color.white;
					
				}
				return _detailsGizmo;
			}
		}
		
			
		static GUIStyle _detailsTitle;
		public static GUIStyle DetailsTitle
		{
			get
			{
				if (_detailsTitle == null)
				{
					_detailsTitle = new GUIStyle();
					_detailsTitle.alignment = TextAnchor.MiddleLeft;
					_detailsTitle.fontSize = 32;
					_detailsTitle.fontStyle = FontStyle.Bold;
					_detailsTitle.margin = new RectOffset(10, 10, 10, 10);
					_detailsTitle.normal.textColor = GetColor(0x00000066);
					
				}
				return _detailsTitle;
			}
		}
		
		static GUIStyle _detailsDescription;
		public static GUIStyle DetailsDescription
		{
			get
			{
				if (_detailsDescription == null)
				{
					_detailsDescription = new GUIStyle();
					_detailsDescription.alignment = TextAnchor.UpperLeft;
					_detailsDescription.margin = new RectOffset(65, 10, 10, 15);
					_detailsDescription.wordWrap = true;
					
				}
				return _detailsDescription;
			}
		}
		
		
		
		static GUIStyle _statusMessage;
		public static GUIStyle StatusMessage
		{
			get
			{
				if (_statusMessage == null)
				{
					_statusMessage = new GUIStyle();
					_statusMessage.alignment = TextAnchor.MiddleCenter;
					_statusMessage.stretchWidth = false;
					_statusMessage.stretchHeight = false;
					_statusMessage.fixedHeight = 20;
					_statusMessage.fontSize = 12;
					_statusMessage.margin = new RectOffset(10, 10, 2, 2);
					_statusMessage.normal.textColor = GetColor(0x00000066);
				}
				return _statusMessage;
			}
		}

		static GUIStyle _settingsDescription;
		public static GUIStyle SettingDescription
		{
			get
			{
				if (_settingsDescription == null)
				{
					_settingsDescription = new GUIStyle();
					_settingsDescription.alignment = TextAnchor.MiddleRight;
					_settingsDescription.imagePosition = ImagePosition.ImageOnly;
					_settingsDescription.fixedWidth = 32;
					_settingsDescription.fixedHeight = 20;
					_settingsDescription.stretchHeight = false;
					_settingsDescription.stretchWidth = false;
					_settingsDescription.margin = new RectOffset(10, 10, 2, 2);

				}
				return _settingsDescription;
			}
		}

		
		static GUIStyle _hLine;
		public static GUIStyle HLine
		{
			get
			{
				if (_hLine == null)
				{
					_hLine = new GUIStyle();
					_hLine.alignment = TextAnchor.MiddleCenter;
					_hLine.stretchWidth = true;
					_hLine.fixedHeight = 1;
					_hLine.normal.background = GetTexture(0x00000066);
				}
				return _hLine;
			}
		}
		
		
		static GUIStyle _vLine;
		public static GUIStyle VLine
		{
			get
			{
				if (_vLine == null)
				{
					_vLine = new GUIStyle();
					_vLine.alignment = TextAnchor.MiddleCenter;
					_vLine.stretchHeight = true;
					_vLine.fixedHeight = 0;
					_vLine.fixedWidth = 1;
					_vLine.normal.background = GetTexture(0xffffff66);
				}
				return _vLine;
			}
		}
		
		
		public static void VerticalLine()
		{
			GUILayout.Label("", VLine);
		}
		
		public static void HorizontalLine()
		{
			GUILayout.Label("", HLine);
		}

		static GUIStyle _hSeparator;
		public static GUIStyle HSeparator
		{
			get
			{
				if (_hSeparator == null)
				{
					_hSeparator = new GUIStyle();
					_hSeparator.alignment = TextAnchor.MiddleCenter;
					_hSeparator.stretchWidth = true;
					_hSeparator.fixedHeight = 1;
					_hSeparator.margin = new RectOffset(20, 20, 5, 5);
					_hSeparator.normal.background = GetTexture(0x00000066);
				}
				return _hSeparator;
			}
		}

		static GUIStyle _progressBar;
		public static GUIStyle ProgressBar
		{
			get
			{
				if (_progressBar == null)
				{
					_progressBar = new GUIStyle();
					_progressBar.alignment = TextAnchor.MiddleCenter;
					_progressBar.stretchWidth = true;
					_progressBar.stretchHeight = true;
					_progressBar.margin = new RectOffset(5, 5, 1, 1);
					_progressBar.normal.background = GetTexture(0x128ce1ff);
				}
				return _progressBar;
			}
		}

		public static void HorizontalSeparator()
		{
			GUILayout.Label("", HSeparator);
		}




	}
}

