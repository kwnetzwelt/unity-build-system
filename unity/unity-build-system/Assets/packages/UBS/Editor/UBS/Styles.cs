using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace UBS
{
	public static class Styles
	{
		public const string kImagePath ="Assets/UEDS/Editor/images/";

		static Dictionary<long,Texture2D>mTextures = new Dictionary<long, Texture2D>();
		
		static Texture2D GetTexture(long pColorRGBA)
		{
			if(mTextures.ContainsKey(pColorRGBA) && mTextures[pColorRGBA] != null)
				return mTextures[pColorRGBA];
				
			Color32 c = GetColor(pColorRGBA);
			
			var tmp = new Texture2D(4,4);
			for(int x = 0;x < 4;x++)
				for(int y = 0;y < 4;y++)
					tmp.SetPixel(x,y,c);
			tmp.Apply();
			tmp.Compress(true);
			
			mTextures[pColorRGBA] = tmp;
			
			return tmp;
		}
		
		static Color32 GetColor(long pColorRGBA)
		{
			byte r =(byte)( (pColorRGBA & 0xff000000) >> 24 );
			byte g =(byte)( (pColorRGBA & 0xff0000) >> 16 );
			byte b =(byte)( (pColorRGBA & 0xff00) >> 8 );
			byte a =(byte)( (pColorRGBA & 0xff) );
			
			Color32 c = new Color32(r,g,b,a);
			return c;
		}
		
		static GUIStyle mList;
		public static GUIStyle list
		{
			get
			{
				if(mList == null)
				{
					mList = new GUIStyle();
					mList.normal.background = GetTexture(0x424646ff);
					mList.alignment = TextAnchor.UpperLeft;
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
				if(mBoldKey == null)
				{
					mBoldKey = new GUIStyle();
					if(EditorGUIUtility.isProSkin)
						mBoldKey.normal.textColor = GetColor(0xffffffff);
					else
						mBoldKey.normal.textColor = GetColor(0x000000ff);
					mBoldKey.fontStyle = FontStyle.Bold;
					mBoldKey.alignment = TextAnchor.MiddleLeft;
					
					
				}
				return mBoldKey;
			}
		}

		static GUIStyle mNormalValue;
		public static GUIStyle normalValue
		{
			get
			{
				if(mNormalValue == null)
				{
					mNormalValue = new GUIStyle();
					if(EditorGUIUtility.isProSkin)
						mNormalValue.normal.textColor = GetColor(0xffffffff);
					else
						mNormalValue.normal.textColor = GetColor(0x000000ff);
					mNormalValue.fontStyle = FontStyle.Normal;
					mNormalValue.alignment = TextAnchor.MiddleLeft;
					
					
				}
				return mNormalValue;
			}
		}

		static GUIStyle mSelectableListEntryText;
		public static GUIStyle selectableListEntryText
		{
			get
			{
				if(mSelectableListEntryText == null)
				{
					mSelectableListEntryText = new GUIStyle(list);
					mSelectableListEntryText.normal.textColor = GetColor(0xb5b5b5ff);
					mSelectableListEntryText.fontStyle = FontStyle.Bold;

					
				}
				return mSelectableListEntryText;
			}
		}

		static GUIStyle mSelectableListEntry;
		public static GUIStyle selectableListEntry
		{
			get
			{
				if(mSelectableListEntry == null)
				{
					mSelectableListEntry = new GUIStyle(list);
					mSelectableListEntry.stretchHeight = false;
					mSelectableListEntry.normal.textColor = GetColor(0xb5b5b5ff);
					mSelectableListEntry.fontStyle = FontStyle.Bold;
					mSelectableListEntry.padding = new RectOffset(20,20,10,9);
					mSelectableListEntry.border = new RectOffset(0,0,0,1);
					
				}
				return mSelectableListEntry;
			}
		}
		
		static GUIStyle mSelectableListEntryOdd;
		public static GUIStyle selectableListEntryOdd
		{
			get
			{
				if(mSelectableListEntryOdd == null)
				{
					mSelectableListEntryOdd = new GUIStyle( selectableListEntry );
					mSelectableListEntryOdd.normal.background = GetTexture(0x333535ff);
					
				}
				return mSelectableListEntryOdd;
			}
		}

		static GUIStyle mToolButton;
		public static GUIStyle toolButton {
			get
			{
				if(mToolButton == null)
				{
					mToolButton = new GUIStyle( "TE toolbarbutton" );

					mToolButton.fontSize = 24;
					mToolButton.fontStyle = FontStyle.Bold;
					mToolButton.fixedHeight = 28;
					mToolButton.padding = new RectOffset( 5, 5, 5, 5);
					mToolButton.contentOffset = new Vector2(-1,-3);
					mToolButton.clipping = TextClipping.Overflow;

				}
				return mToolButton;
			}

		}
		
		static GUIStyle mSelectedListEntry;
		public static GUIStyle selectedListEntry
		{
			get
			{
				if(mSelectedListEntry == null)
				{
					mSelectedListEntry = new GUIStyle();
					mSelectedListEntry.normal.background = GetTexture(0x1b76d1ff);
					mSelectedListEntry.normal.textColor = GetColor(0xffffffff);
					mSelectedListEntry.fontStyle = FontStyle.Bold;
					mSelectedListEntry.padding = new RectOffset(20,20,10,9);
					mSelectedListEntry.border = new RectOffset(0,0,0,1);
				}
				return mSelectedListEntry;
			}
		}

		static GUIStyle mDirtySetting;
		public static GUIStyle dirtySetting
		{
			get
			{
				if(mDirtySetting == null)
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
				if(mBigHint == null)
				{
					mBigHint = new GUIStyle();
					mBigHint.alignment = TextAnchor.MiddleCenter;
					mBigHint.stretchWidth = true;
					mBigHint.stretchHeight = true;
					mBigHint.fontSize = 32;
					mBigHint.fontStyle = FontStyle.Bold;
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
				if(mMediumHint == null)
				{
					mMediumHint = new GUIStyle();
					mMediumHint.alignment = TextAnchor.MiddleCenter;
					mMediumHint.stretchWidth = true;
					mMediumHint.stretchHeight = false;
					mMediumHint.fontSize = 24;
					mMediumHint.fontStyle = FontStyle.Bold;
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
				if(mDetailsGroup == null)
				{
					mDetailsGroup = new GUIStyle();
					mDetailsGroup.alignment = TextAnchor.UpperLeft;
					mDetailsGroup.margin = new RectOffset(2,2,2,2);
					
				}
				return mDetailsGroup;
			}
		}
		
		static GUIStyle mInfoGroup;
		public static GUIStyle infoGroup
		{
			get
			{
				if(mInfoGroup == null)
				{
					mInfoGroup = new GUIStyle();
					mInfoGroup.alignment = TextAnchor.UpperLeft;
					mInfoGroup.margin = new RectOffset(20,20,2,10);
					mInfoGroup.padding = new RectOffset(10,10,5,5);
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
				if(mDetailsGizmo == null)
				{
					mDetailsGizmo = new GUIStyle();
					mDetailsGizmo.alignment = TextAnchor.MiddleLeft;
					mDetailsGizmo.fixedWidth = 32;
					mDetailsGizmo.fixedHeight = 32;
					mDetailsGizmo.margin = new RectOffset(10,10,0,0);
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
				if(mDetailsTitle == null)
				{
					mDetailsTitle = new GUIStyle();
					mDetailsTitle.alignment = TextAnchor.MiddleLeft;
					mDetailsTitle.fontSize = 32;
					mDetailsTitle.fontStyle = FontStyle.Bold;
					mDetailsTitle.margin = new RectOffset(10,10,10,10);
					mDetailsTitle.normal.textColor = GetColor(0x00000066);
					
				}
				return mDetailsTitle;
			}
		}
		
		static GUIStyle mDetailsDescription;
		public static GUIStyle detailsDescription
		{
			get
			{
				if(mDetailsDescription == null)
				{
					mDetailsDescription = new GUIStyle();
					mDetailsDescription.alignment = TextAnchor.UpperLeft;
					mDetailsDescription.margin = new RectOffset(65,10,10,15);
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
				if(mStatusMessage == null)
				{
					mStatusMessage = new GUIStyle();
					mStatusMessage.alignment = TextAnchor.MiddleCenter;
					mStatusMessage.stretchWidth = false;
					mStatusMessage.stretchHeight = false;
					mStatusMessage.fixedHeight = 20;
					mStatusMessage.fontSize = 12;
					mStatusMessage.margin = new RectOffset(10,10,2,2);
					mStatusMessage.normal.textColor = GetColor(0x00000066);
				}
				return mStatusMessage;
			}
		}

		static GUIStyle mSettingsDescription;
		public static GUIStyle settingDescription
		{
			get
			{
				if(mSettingsDescription == null)
				{
					mSettingsDescription = new GUIStyle();
					mSettingsDescription.alignment = TextAnchor.MiddleRight;
					mSettingsDescription.imagePosition = ImagePosition.ImageOnly;
					mSettingsDescription.fixedWidth = 32;
					mSettingsDescription.fixedHeight = 20;
					mSettingsDescription.stretchHeight = false;
					mSettingsDescription.stretchWidth = false;
					mSettingsDescription.margin = new RectOffset(10,10,2,2);

				}
				return mSettingsDescription;
			}
		}

		
		static GUIStyle mHLine;
		public static GUIStyle hLine
		{
			get
			{
				if(mHLine == null)
				{
					mHLine = new GUIStyle();
					mHLine.alignment = TextAnchor.MiddleCenter;
					mHLine.stretchWidth = true;
					mHLine.fixedHeight = 1;
					mHLine.normal.background  = GetTexture(0x00000066);
				}
				return mHLine;
			}
		}
		
		
		static GUIStyle mVLine;
		public static GUIStyle vLine
		{
			get
			{
				if(mVLine == null)
				{
					mVLine = new GUIStyle();
					mVLine.alignment = TextAnchor.MiddleCenter;
					mVLine.stretchHeight = true;
					mVLine.fixedHeight = 0;
					mVLine.fixedWidth = 1;
					mVLine.normal.background  = GetTexture(0xffffff66);
				}
				return mVLine;
			}
		}
		
		
		public static void VerticalLine()
		{
			GUILayout.Label("",vLine);
		}
		
		public static void HorizontalLine()
		{
			GUILayout.Label("",hLine);
		}

		static GUIStyle mHSeparator;
		public static GUIStyle hSeparator
		{
			get
			{
				if(mHSeparator == null)
				{
					mHSeparator = new GUIStyle();
					mHSeparator.alignment = TextAnchor.MiddleCenter;
					mHSeparator.stretchWidth = true;
					mHSeparator.fixedHeight = 1;
					mHSeparator.margin = new RectOffset(20,20,5,5);
					mHSeparator.normal.background  = GetTexture(0x00000066);
				}
				return mHSeparator;
			}
		}

		static GUIStyle mProgressBar;
		public static GUIStyle progressBar
		{
			get
			{
				if(mProgressBar == null)
				{
					mProgressBar = new GUIStyle();
					mProgressBar.alignment = TextAnchor.MiddleCenter;
					mProgressBar.stretchWidth = true;
					mProgressBar.stretchHeight = true;
					mProgressBar.margin = new RectOffset(5,5,1,1);
					mProgressBar.normal.background  = GetTexture(0x128ce1ff);
				}
				return mProgressBar;
			}
		}

		public static void HorizontalSeparator()
		{
			GUILayout.Label("",hSeparator);
		}


		static GUIContent mOpenFileContent;
		public static GUIContent openFileContent
		{
			get 
			{
				if(mOpenFileContent == null)
				{
					mOpenFileContent = new GUIContent();
					mOpenFileContent.image =  (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.kImagePath + "open.png", typeof(Texture2D));
					mOpenFileContent.text = "Open";
				}
				return mOpenFileContent;
			}
		}

		static GUIContent mSaveFileContent;
		public static GUIContent saveFileContent
		{
			get 
			{
				if(mSaveFileContent == null)
				{
					mSaveFileContent = new GUIContent();
					mSaveFileContent.image =  (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.kImagePath + "save.png", typeof(Texture2D));
					mSaveFileContent.text = "Save";
				}
				return mSaveFileContent;
			}
		}



		static GUIContent mDupInstanceContent;
		public static GUIContent dupInstanceContent
		{
			get
			{
				if(mDupInstanceContent == null)
				{
					mDupInstanceContent = new GUIContent();
					mDupInstanceContent.image =  (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.kImagePath + "duplicate.png", typeof(Texture2D));
					mDupInstanceContent.text = "Duplicate Instance";
				}
				return mDupInstanceContent;
			}
		}
		static GUIContent mAddInstanceContent;
		public static GUIContent addInstanceContent
		{
			get 
			{
				if(mAddInstanceContent == null)
				{
					mAddInstanceContent = new GUIContent();
					mAddInstanceContent.image =  (Texture2D)AssetDatabase.LoadAssetAtPath( Styles.kImagePath + "add.png", typeof(Texture2D));
					mAddInstanceContent.text = "Add Instance";
				}
				return mAddInstanceContent;
			}
		}
		static GUIContent mDelInstanceContent;
		public static GUIContent delInstanceContent
		{
			get 
			{
				if(mDelInstanceContent == null)
				{
					mDelInstanceContent = new GUIContent();
					mDelInstanceContent.image =  (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.kImagePath + "delete.png", typeof(Texture2D));
					mDelInstanceContent.text = "Remove Instance";
				}
				return mDelInstanceContent;
			}
		}
		
		static GUIContent mRenameInstanceContent;
		public static GUIContent renameInstanceContent
		{
			get 
			{
				if(mRenameInstanceContent == null)
				{
					mRenameInstanceContent = new GUIContent();
					mRenameInstanceContent.image =  (Texture2D)AssetDatabase.LoadAssetAtPath(Styles.kImagePath + "rename.png", typeof(Texture2D));
					mRenameInstanceContent.tooltip = "Rename Instance";
				}
				return mRenameInstanceContent;
			}
		}


		static string mUpTexture = "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TUw6Y29tLmFkb2JlLnhtcAAAAAAAPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iWE1QIENvcmUgNS40LjAiPgogICA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPgogICAgICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgICAgICAgICB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iCiAgICAgICAgICAgIHhtbG5zOnRpZmY9Imh0dHA6Ly9ucy5hZG9iZS5jb20vdGlmZi8xLjAvIgogICAgICAgICAgICB4bWxuczpleGlmPSJodHRwOi8vbnMuYWRvYmUuY29tL2V4aWYvMS4wLyI+CiAgICAgICAgIDx4bXA6TW9kaWZ5RGF0ZT4yMDE0LTA1LTIwVDIyOjA1OjYzPC94bXA6TW9kaWZ5RGF0ZT4KICAgICAgICAgPHhtcDpDcmVhdG9yVG9vbD5QaXhlbG1hdG9yIDMuMTwveG1wOkNyZWF0b3JUb29sPgogICAgICAgICA8dGlmZjpPcmllbnRhdGlvbj4xPC90aWZmOk9yaWVudGF0aW9uPgogICAgICAgICA8dGlmZjpDb21wcmVzc2lvbj41PC90aWZmOkNvbXByZXNzaW9uPgogICAgICAgICA8dGlmZjpSZXNvbHV0aW9uVW5pdD4xPC90aWZmOlJlc29sdXRpb25Vbml0PgogICAgICAgICA8dGlmZjpZUmVzb2x1dGlvbj43MjwvdGlmZjpZUmVzb2x1dGlvbj4KICAgICAgICAgPHRpZmY6WFJlc29sdXRpb24+NzI8L3RpZmY6WFJlc29sdXRpb24+CiAgICAgICAgIDxleGlmOlBpeGVsWERpbWVuc2lvbj4yNDwvZXhpZjpQaXhlbFhEaW1lbnNpb24+CiAgICAgICAgIDxleGlmOkNvbG9yU3BhY2U+MTwvZXhpZjpDb2xvclNwYWNlPgogICAgICAgICA8ZXhpZjpQaXhlbFlEaW1lbnNpb24+MjQ8L2V4aWY6UGl4ZWxZRGltZW5zaW9uPgogICAgICA8L3JkZjpEZXNjcmlwdGlvbj4KICAgPC9yZGY6UkRGPgo8L3g6eG1wbWV0YT4Kr6DZ0gAAAppJREFUSA3tVM9rWkEQ3t0gNtgmSM1BetFC+2ihVKqEUpDgNfdc8x/k1lPOueToX+RBKCJarMUWK1QhlBwSKDYtVNzd6TfzfJJQ83yFHrOyv+fNfN83syp11/6nApOzs/3JZLL/Lz51UuNOp5N6mNv5qDWpy4vLF5VKZZ7kW5PEiG2y2eyR8j7wjoJcNneU9LtEDEaj0Y4xZkSKtkEATU89+SdoF+sCJWJARCfeu23vPUiQgnOs6WSdc4Gyzmg4HL6Ez/eKlFGAz4i80spoRFPmVRAEH+J8rGVgna8TOePJAr1TDp2cV9Z6Y62txznnu9gAg37/wDu75+aQBk5pIZHzVhGJXHuDQf8gLsitSR6Px/eurn58JlIFBX2QWOmSZMCKTnA/2dp68KxYLP5eFehWBt+n07feuQISCrRILBJBkMcTOtjgTjl0sCpMYbvKOZ+tZNDr9R6hYoaAmdEwIY18oiGO0jpigo1BUEyk9K8NY4JSqfRNDK8NKxk4a0+BOuMYOWuNmdcsizASJqQsWIEMM8yAzek1v8vlXwy63e5ryPGOcbMVYeIVypKRyp7/LqThgo2YH5PRG/pNuVxuhZfheIMBkKC653X2Qqw5o4bmGJADTFiyO5AKc4I1//hJ4KfBAiUdAouC3AjQbrUPQXmX651frVsk2GEtJYoIPEdB+T3I62b3YuN2W+3WYeSc56VEjUbjfnoz/QW2eWRSbPRC93BroAFnFEItZGMrbMWcM7TYn89ms6e1Wu0nO1kySKdSxyi/vMiC8pMyZFnAwjlOMvSxjDRcR6WKkxA9bMJS9vlUKn0sCDEI1Gaz+RgAPmGXZkRwK+XIwbgsGR03SSnOBGsEn0/BTKQXhmI3w/HzarX6VT68G+IU+APvEcEUwKrh+gAAAABJRU5ErkJggg==";

		static GUIContent mUpTextureCnt = null;
		public static GUIContent upTexture
		{
			get
			{
				if(mUpTextureCnt == null)
				{
					Texture2D txture = new Texture2D(24,24);
					byte[] data = Convert.FromBase64String(mUpTexture);
					txture.LoadImage(data);
					mUpTextureCnt = new GUIContent(txture);
				}
				return mUpTextureCnt;
			}
		}
	}
}

