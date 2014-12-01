using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace UBS
{
	internal static class Styles
	{
		public const string imagePath = "Assets/packages/UBS/Editor/images/";
		public const string fontPath = "Assets/packages/UBS/Editor/font/bebas/";

		static Dictionary<ToolIcon, Texture2D> toolIcons = new Dictionary<ToolIcon, Texture2D>();

		static Font fontRegular;
		static Font fontBook;
		static Font fontBold;
		const int fontSizeSmall = 18;
		const int fontSizeMedium = 24;
		const int fontSizeLarge = 32;

		public enum Colors : long
		{
			White = 0xfffdfcff,
			Blue = 0x2f3c4aff,
			LightGrey = 0xf1f1f1ff,
			Grey = 0x363335ff,
			Orange = 0xff8033ff,
			Cyan = 0x5ae7c4ff,
			Purple = 0xb83c82ff,
			DarkBlue = 0x222c36ff,
			LightPurple = 0xff0066ff,
			LightBlue = 0x00ccccff,
			Transparent = 0xffffff00
		}

		static Dictionary<long,Texture2D>mTextures = new Dictionary<long, Texture2D>();
		
		static Texture2D GetTexture(long pColorRGBA)
		{
			if (mTextures.ContainsKey(pColorRGBA) && mTextures [pColorRGBA] != null)
				return mTextures [pColorRGBA];
				
			Color32 c = GetColor(pColorRGBA);
			
			var tmp = new Texture2D(4, 4);
			for (int x = 0; x < 4; x++)
				for (int y = 0; y < 4; y++)
					tmp.SetPixel(x, y, c);
			tmp.Apply();
			tmp.Compress(true);
			
			mTextures [pColorRGBA] = tmp;
			
			return tmp; 
		}
		
		static Color32 GetColor(long pColorRGBA)
		{
			byte r = (byte)((pColorRGBA & 0xff000000) >> 24);
			byte g = (byte)((pColorRGBA & 0xff0000) >> 16);
			byte b = (byte)((pColorRGBA & 0xff00) >> 8);
			byte a = (byte)((pColorRGBA & 0xff));
			
			Color32 c = new Color32(r, g, b, a);
			return c;
		}

		public static Texture2D GetPlatformIcon(BuildTarget mPlatform)
		{
			switch (mPlatform)
			{
				case BuildTarget.iPhone:
					return icoIOS;
				case BuildTarget.Android:
					return icoAndroid;
				case BuildTarget.StandaloneWindows:
					return icoWindows;
				case BuildTarget.StandaloneWindows64:
					return icoWindows;
				default:
					return new Texture2D(0, 0);
			}
		}

		public enum ToolIcon
		{
			Add,
			Remove,
			Copy,
			Up,
			Down,
			DrownDown,
			Search
		}
		
		public static Texture2D GetToolIcon(ToolIcon tool)
		{
			if (!toolIcons.ContainsKey(tool))
			{
				var toolIcon = (Texture2D)AssetDatabase.LoadAssetAtPath(string.Format("{0}icons/tools/ico_{1}.png", Styles.imagePath, tool.ToString().ToLower()), typeof(Texture2D));
				toolIcons.Add(tool, toolIcon);
			}
			return toolIcons [tool];
		}
        
		static Font GetFontRegular()
		{
			if (fontRegular == null)
			{
				fontRegular = (Font)AssetDatabase.LoadAssetAtPath(Styles.fontPath + "BebasNeue Regular.ttf", typeof(Font));
			}
			return fontRegular;
		}

		static Font GetFontBook()
		{
			if (fontBook == null)
			{
				fontBook = (Font)AssetDatabase.LoadAssetAtPath(Styles.fontPath + "BebasNeue Book.ttf", typeof(Font));
			}
			return fontBook;
		}

		static Font GetFontBold()
		{
			if (fontBold == null)
			{
				fontBold = (Font)AssetDatabase.LoadAssetAtPath(Styles.fontPath + "BebasNeue Bold.ttf", typeof(Font));
			}
			return fontBold;
		}

		static GUIStyle standardStyle;
		public static GUIStyle standard
		{
			get
			{
				if (standardStyle == null)
				{
					standardStyle = new GUIStyle();
					standardStyle.font = GetFontBook();
					standardStyle.normal.textColor = GetColor((long)Colors.DarkBlue);
					standardStyle.fontSize = fontSizeSmall;
					standardStyle.padding = new RectOffset();
					standardStyle.margin = new RectOffset(4, 2, 3, 3);
				}
				return standardStyle;
			}
		}

		static GUIStyle inputStyle;
		public static GUIStyle input
		{
			get
			{
				if (inputStyle == null)
				{
					inputStyle = new GUIStyle(standard);
					inputStyle.font = GetFontBook();
					inputStyle.padding = new RectOffset(2, 2, 2, 2);
					inputStyle.fixedHeight = inputStyle.fontSize * 1.25f;
					inputStyle.fixedWidth = inputStyle.fixedHeight * 15f;
					inputStyle.normal.background = GetTexture((long)Colors.White);

					// active input
					var activeTextClr = GetColor((long)Colors.DarkBlue);
					var activeBGClr = GetTexture((long)Colors.Cyan);
					inputStyle.hover.background = activeBGClr;
					inputStyle.hover.textColor = activeTextClr;
					inputStyle.active.background = activeBGClr;
					inputStyle.hover.textColor = activeTextClr;
					inputStyle.onFocused.textColor = activeTextClr;
				}
				return inputStyle;
			}
		}

		static GUIStyle smallInputStyle;
		public static GUIStyle smallInput
		{
			get
			{
				if (smallInputStyle == null)
				{
					smallInputStyle = new GUIStyle(input);
					smallInputStyle.margin = new RectOffset(0, 2, 1, 0);
					smallInputStyle.padding = new RectOffset(4, 0, 0, 0);
					smallInputStyle.fontSize = fontSizeSmall;
					smallInputStyle.fixedHeight = fontSizeSmall * 0.95f;
					smallInputStyle.fixedWidth = 60f;
				}
				return smallInputStyle;
			}
		}

		static GUIStyle rightAligned;
		public static GUIStyle RightAligned
		{
			get
			{
				if (rightAligned == null)
				{
					rightAligned = new GUIStyle();
					rightAligned.alignment = TextAnchor.MiddleRight;
				}
				return rightAligned;
			}
		}

		static GUIStyle dropdownStyle;
		public static GUIStyle dropdown
		{
			get
			{
				if (dropdownStyle == null)
				{
					dropdownStyle = new GUIStyle(input);
					dropdownStyle.font = GetFontBook();
				}
				return dropdownStyle;
			}
		}

		static GUIStyle dropdownIcon;
		public static GUIStyle DropDownIcon
		{
			get
			{
				if (dropdownIcon == null)
				{
					dropdownIcon = new GUIStyle();
					dropdownIcon.fixedWidth = 20f;
					dropdownIcon.contentOffset = new Vector2(256f, 0f);
					dropdownIcon.padding = new RectOffset(0, 0, 4, 0);
				}
				return dropdownIcon;
			}
		}
		
		static GUIStyle mList;
		public static GUIStyle list
		{
			get
			{
				if (mList == null)
				{
					mList = new GUIStyle();
					mList.normal.background = GetTexture(0x424646ff);
					mList.alignment = TextAnchor.UpperLeft;
					mList.fontSize = fontSizeSmall;
					mList.font = GetFontRegular();
					mList.stretchWidth = true;
					mList.stretchHeight = true;
				}
				return mList;
			}
		}

		static GUIStyle mBoldKey;
		public static GUIStyle boldKey
		{
			get
			{
				if (mBoldKey == null)
				{
					mBoldKey = new GUIStyle();
					if (EditorGUIUtility.isProSkin)
						mBoldKey.normal.textColor = GetColor(0xffffffff);
					else
						mBoldKey.normal.textColor = GetColor(0x000000ff);
					mBoldKey.font = GetFontBold();
					mBoldKey.alignment = TextAnchor.MiddleLeft;
					
					
				}
				return mBoldKey;
			}
		}

		static GUIStyle mBuildProcessEditorBackground;
		public static GUIStyle buildProcessEditorBackground
		{
			get
			{
				if (mBuildProcessEditorBackground == null)
				{
					mBuildProcessEditorBackground = new GUIStyle();
					if (EditorGUIUtility.isProSkin)
						mBuildProcessEditorBackground.normal.background = GetTexture(0x444444aa);
					else
						mBuildProcessEditorBackground.normal.background = GetTexture(0xeeeeeeaa);
				}
				return mBuildProcessEditorBackground;
			}
		}

		static Texture2D mGear;
		public static Texture2D gear
		{
			get
			{
				if (mGear == null)
				{
					mGear = (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.imagePath + "gear.png", typeof(Texture2D));

				}
				return mGear;
			}
		}
		static Texture2D mIcoIOS;
		public static Texture2D icoIOS
		{
			get
			{
				if (mIcoIOS == null)
				{
					mIcoIOS = (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.imagePath + "icons/platform/ico_ios.png", typeof(Texture2D));
					
				}
				return mIcoIOS;
			}
		}
		static Texture2D mIcoAndroid;
		public static Texture2D icoAndroid
		{
			get
			{
				if (mIcoAndroid == null)
				{
					mIcoAndroid = (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.imagePath + "icons/platform/ico_android.png", typeof(Texture2D));
					
				}
				return mIcoAndroid;
			}
		}
		static Texture2D mIcoWindows;
		public static Texture2D icoWindows
		{ 
			get
			{
				if (mIcoWindows == null)
				{
					mIcoWindows = (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.imagePath + "icons/platform/ico_windows.png", typeof(Texture2D));
					
				}
				return mIcoWindows;
			}
		}
		static GUIStyle mIcon;
		public static GUIStyle icon
		{ 
			get
			{
				if (mIcon == null)
				{
					mIcon = new GUIStyle();
					mIcon.fixedWidth = 20;
					mIcon.fixedHeight = 20;
					mIcon.contentOffset = new Vector2(-6f, -2f);
				}
				return mIcon;
			}
		}
        
		static GUIStyle mNormalValue;
		public static GUIStyle normalValue
		{
			get
			{
				if (mNormalValue == null)
				{
					mNormalValue = new GUIStyle();
					if (EditorGUIUtility.isProSkin)
						mNormalValue.normal.textColor = GetColor(0xffffffff);
					else
						mNormalValue.normal.textColor = GetColor(0x000000ff);
					mNormalValue.font = GetFontRegular();
					mNormalValue.alignment = TextAnchor.MiddleLeft;
					
					
				}
				return mNormalValue;
			}
		}

		// Inspector list
		static GUIStyle mSelectableListEntry;
		public static GUIStyle selectableListEntry
		{
			get
			{
				if (mSelectableListEntry == null)
				{
					mSelectableListEntry = new GUIStyle(list);
					mSelectableListEntry.stretchHeight = false;
					mSelectableListEntry.normal.textColor = GetColor((long)Colors.White);
					mSelectableListEntry.normal.background = GetTexture((long)Colors.Blue);
					mSelectableListEntry.padding = new RectOffset(10, 10, 6, 6);
					mSelectableListEntry.border = new RectOffset(0, 0, 0, 1);
					
				}
				return mSelectableListEntry;
			}
		}

		// Inspector list
		static GUIStyle mSelectableListEntryOdd;
		public static GUIStyle selectableListEntryOdd
		{
			get
			{
				if (mSelectableListEntryOdd == null)
				{
					mSelectableListEntryOdd = new GUIStyle(selectableListEntry);
					mSelectableListEntryOdd.normal.background = GetTexture((long)Colors.DarkBlue);
					
				}
				return mSelectableListEntryOdd;
			}
		}

		static GUIStyle mToolButton;
		public static GUIStyle ToolButton
		{
			get
			{
				if (mToolButton == null)
				{
					mToolButton = new GUIStyle();

					mToolButton.fixedHeight = 34f;
					mToolButton.fixedWidth = 34f;
					mToolButton.padding = new RectOffset(2, 2, 2, 2);
					mToolButton.margin = new RectOffset(0, 0, 1, 1);
					mToolButton.border = new RectOffset(0, 0, 0, 0);
					mToolButton.normal.background = GetTexture((long)Colors.Purple);
					mToolButton.active.background = GetTexture((long)Colors.LightPurple);
				}
				return mToolButton;
			}
		}

		static GUIStyle mToolColumn;
		public static GUIStyle ToolColumn
		{
			get
			{
				if (mToolColumn == null)
				{
					mToolColumn = new GUIStyle();
					mToolColumn.fixedWidth = 33f;
					mToolColumn.fixedHeight = 405f; // hack to color the entire column //TODO
					mToolColumn.normal.background = GetTexture((long)Colors.LightGrey);
				}
				return mToolColumn;
			}
		}

		static GUIStyle mProcessColumn;
		public static GUIStyle ProcessColumn
		{
			get
			{
				if (mProcessColumn == null)
				{
					mProcessColumn = new GUIStyle();
					mProcessColumn.fixedWidth = 250f;
					mProcessColumn.normal.background = GetTexture((long)Colors.DarkBlue);
				}
				return mProcessColumn;
			}
		}
        
		// Editor Window
		static GUIStyle mSelectedListEntry;
		public static GUIStyle selectedListEntryButton
		{
			get
			{
				if (mSelectedListEntry == null)
				{
					mSelectedListEntry = new GUIStyle();
					mSelectedListEntry.normal.background = GetTexture((long)Colors.Cyan);
					mSelectedListEntry.normal.textColor = GetColor((long)Colors.DarkBlue);
					mSelectedListEntry.font = GetFontBold();
					mSelectedListEntry.fontSize = fontSizeSmall;
					mSelectedListEntry.padding = new RectOffset(6, 6, 6, 6);
					mSelectedListEntry.margin = new RectOffset(4, 0, 0, 0);
					mSelectedListEntry.border = new RectOffset(0, 0, 0, 1);
				}
				return mSelectedListEntry;
			}
		}

		static GUIStyle mDirtySetting;
		public static GUIStyle dirtySetting
		{
			get
			{
				if (mDirtySetting == null)
				{
					mDirtySetting = new GUIStyle();
					mDirtySetting.normal.background = GetTexture(0x1b76d1ff);

				}
				return mDirtySetting;
			}
		}
		
		static GUIStyle mBigHint;
		public static GUIStyle bigHint
		{
			get
			{
				if (mBigHint == null)
				{
					mBigHint = new GUIStyle();
					mBigHint.alignment = TextAnchor.MiddleCenter;
					mBigHint.stretchWidth = true;
					mBigHint.stretchHeight = true;
					mBigHint.fontSize = fontSizeLarge;
					mBigHint.font = GetFontBold();
					mBigHint.normal.textColor = GetColor(0x00000066);
				}
				return mBigHint;
			}
		}
		static GUIStyle mMediumHint;
		public static GUIStyle mediumHint
		{
			get
			{
				if (mMediumHint == null)
				{
					mMediumHint = new GUIStyle();
					mMediumHint.alignment = TextAnchor.MiddleCenter;
					mMediumHint.stretchWidth = true;
					mMediumHint.stretchHeight = false;
					mMediumHint.fontSize = fontSizeMedium;
					mMediumHint.font = GetFontBold();
					mMediumHint.normal.textColor = GetColor(0x00000066);
				}
				return mMediumHint;
			}
		}
		
		static GUIStyle mDetailsGroup;
		public static GUIStyle detailsGroup
		{
			get
			{
				if (mDetailsGroup == null)
				{
					mDetailsGroup = new GUIStyle();
					mDetailsGroup.alignment = TextAnchor.UpperLeft;
					mDetailsGroup.margin = new RectOffset(2, 2, 2, 2);
					
				}
				return mDetailsGroup;
			}
		}
		
		static GUIStyle mInfoGroup;
		public static GUIStyle infoGroup
		{
			get
			{
				if (mInfoGroup == null)
				{
					mInfoGroup = new GUIStyle();
					mInfoGroup.alignment = TextAnchor.UpperLeft;
					mInfoGroup.margin = new RectOffset(20, 20, 2, 10);
					mInfoGroup.padding = new RectOffset(10, 10, 5, 5);
					mInfoGroup.normal.background = GetTexture(0x00000066);
					
				}
				return mInfoGroup;
			}
		}

		static GUIStyle mDetailsGizmo;
		public static GUIStyle detailsGizmo
		{
			get
			{
				if (mDetailsGizmo == null)
				{
					mDetailsGizmo = new GUIStyle();
					mDetailsGizmo.alignment = TextAnchor.MiddleLeft;
					mDetailsGizmo.fixedWidth = 32;
					mDetailsGizmo.fixedHeight = 32;
					mDetailsGizmo.margin = new RectOffset(10, 10, 0, 0);
					mDetailsGizmo.normal.textColor = Color.white;
					
				}
				return mDetailsGizmo;
			}
		}
		
			
		static GUIStyle mDetailsTitle;
		public static GUIStyle detailsTitle
		{
			get
			{
				if (mDetailsTitle == null)
				{
					mDetailsTitle = new GUIStyle(standard);
					mDetailsTitle.alignment = TextAnchor.MiddleLeft;
					mDetailsTitle.fontSize = fontSizeLarge;
					mDetailsTitle.font = GetFontBold();
					mDetailsTitle.padding = new RectOffset(10, 10, 10, 5);
					mDetailsTitle.margin = new RectOffset();
					mDetailsTitle.normal.textColor = GetColor((long)Colors.White);
					mDetailsTitle.normal.background = GetTexture((long)Colors.DarkBlue);
					
				}
				return mDetailsTitle;
			}
		}
		
		static GUIStyle mDetailsDescription;
		public static GUIStyle detailsDescription
		{
			get
			{
				if (mDetailsDescription == null)
				{
					mDetailsDescription = new GUIStyle();
					mDetailsDescription.alignment = TextAnchor.UpperLeft;
					mDetailsDescription.margin = new RectOffset(65, 10, 10, 15);
					mDetailsDescription.wordWrap = true;
					
				}
				return mDetailsDescription;
			}
		}
		
		
		
		static GUIStyle mStatusMessage;
		public static GUIStyle statusMessage
		{
			get
			{
				if (mStatusMessage == null)
				{
					mStatusMessage = new GUIStyle();
					mStatusMessage.alignment = TextAnchor.MiddleCenter;
					mStatusMessage.stretchWidth = false;
					mStatusMessage.stretchHeight = false;
					mStatusMessage.fixedHeight = 20;
					mStatusMessage.fontSize = fontSizeSmall;
					mStatusMessage.font = GetFontBook();
					mStatusMessage.margin = new RectOffset(10, 10, 2, 2);
					mStatusMessage.normal.textColor = GetColor((long)Colors.Grey);
				}
				return mStatusMessage;
			}
		}

		static GUIStyle mSettingsDescription;
		public static GUIStyle settingDescription
		{
			get
			{
				if (mSettingsDescription == null)
				{
					mSettingsDescription = new GUIStyle();
					mSettingsDescription.alignment = TextAnchor.MiddleRight;
					mSettingsDescription.imagePosition = ImagePosition.ImageOnly;
					mSettingsDescription.fixedWidth = 32;
					mSettingsDescription.fixedHeight = 20;
					mSettingsDescription.stretchHeight = false;
					mSettingsDescription.stretchWidth = false;
					mSettingsDescription.margin = new RectOffset(10, 10, 2, 2);

				}
				return mSettingsDescription;
			}
		}

		
		static GUIStyle mHLine;
		public static GUIStyle hLine
		{
			get
			{
				if (mHLine == null)
				{
					mHLine = new GUIStyle();
					mHLine.alignment = TextAnchor.MiddleCenter;
					mHLine.stretchWidth = true;
					mHLine.fixedHeight = 1;
					mHLine.normal.background = GetTexture(0x00000066);
				}
				return mHLine;
			}
		}
		
		
		static GUIStyle mVLine;
		public static GUIStyle vLine
		{
			get
			{
				if (mVLine == null)
				{
					mVLine = new GUIStyle();
					mVLine.alignment = TextAnchor.MiddleCenter;
					mVLine.stretchHeight = true;
					mVLine.fixedHeight = 0;
					mVLine.fixedWidth = 1;
					mVLine.normal.background = GetTexture(0xffffff66);
				}
				return mVLine;
			}
		}
		
		
		public static void VerticalLine()
		{
			GUILayout.Label("", vLine);
		}
		
		public static void HorizontalLine()
		{
			GUILayout.Label("", hLine);
		}

		static GUIStyle mHSeparator;
		public static GUIStyle hSeparator
		{
			get
			{
				if (mHSeparator == null)
				{
					mHSeparator = new GUIStyle();
					mHSeparator.alignment = TextAnchor.MiddleCenter;
					mHSeparator.stretchWidth = true;
					mHSeparator.fixedHeight = 1;
					mHSeparator.margin = new RectOffset(20, 20, 5, 5);
					mHSeparator.normal.background = GetTexture(0x00000066);
				}
				return mHSeparator;
			}
		}

		static GUIStyle mProgressBar;
		public static GUIStyle progressBar
		{
			get
			{
				if (mProgressBar == null)
				{
					mProgressBar = new GUIStyle();
					mProgressBar.alignment = TextAnchor.MiddleCenter;
					mProgressBar.stretchWidth = true;
					mProgressBar.stretchHeight = true;
					mProgressBar.font = GetFontBook(); 
					mProgressBar.margin = new RectOffset(5, 5, 1, 1);
					mProgressBar.normal.background = GetTexture(0x128ce1ff);
				}
				return mProgressBar;
			}
		}

		public static void HorizontalSeparator()
		{
			GUILayout.Label("", hSeparator);
		}




	}
}

