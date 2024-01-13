using System;
using UnityEngine;
using Object = System.Object;

namespace LoserCheatMod
{
    public class Draw
    {
        public static GUIStyle BgStyle;
        public static GUIStyle OnStyle;
        public static GUIStyle OffStyle;
        public static GUIStyle LabelStyle;
        public static GUIStyle BtnStyle;
        public static Texture2D btnpresstexture;
        public static Texture2D btntexture;
        public static Texture2D onpresstexture;
        public static Texture2D ontexture;
        public static Texture2D offpresstexture;
        public static Texture2D offtexture;
        public static Texture2D backtexture;

        public static void InitStyles()
        {
            if (BgStyle == null)
            {
                BgStyle = new GUIStyle();
                BgStyle.normal.background = BackTexture;
                BgStyle.onNormal.background = BackTexture;
                BgStyle.active.background = BackTexture;
                BgStyle.onActive.background = BackTexture;
                BgStyle.normal.textColor = Color.white;
                BgStyle.onNormal.textColor = Color.white;
                BgStyle.active.textColor = Color.white;
                BgStyle.onActive.textColor = Color.white;
                BgStyle.fontSize = 18;
                BgStyle.fontStyle = (FontStyle)0;
                BgStyle.alignment = (TextAnchor)1;
            }
            if (LabelStyle == null)
            {
                LabelStyle = new GUIStyle();
                LabelStyle.normal.textColor = Color.white;
                LabelStyle.onNormal.textColor = Color.white;
                LabelStyle.active.textColor = Color.white;
                LabelStyle.onActive.textColor = Color.white;
                LabelStyle.fontSize = 16;
                LabelStyle.fontStyle = (FontStyle)0;
                LabelStyle.alignment = (TextAnchor)4;
            }
            if (OffStyle == null)
            {
                OffStyle = new GUIStyle();
                OffStyle.normal.background = OffTexture;
                OffStyle.onNormal.background = OffTexture;
                OffStyle.active.background = OffPressTexture;
                OffStyle.onActive.background = OffPressTexture;
                OffStyle.normal.textColor = Color.white;
                OffStyle.onNormal.textColor = Color.white;
                OffStyle.active.textColor = Color.white;
                OffStyle.onActive.textColor = Color.white;
                OffStyle.fontSize = 16;
                OffStyle.fontStyle = (FontStyle)0;
                OffStyle.alignment = (TextAnchor)4;
            }
            if (OnStyle == null)
            {
                OnStyle = new GUIStyle();
                OnStyle.normal.background = OnTexture;
                OnStyle.onNormal.background = OnTexture;
                OnStyle.active.background = OnPressTexture;
                OnStyle.onActive.background = OnPressTexture;
                OnStyle.normal.textColor = Color.white;
                OnStyle.onNormal.textColor = Color.white;
                OnStyle.active.textColor = Color.white;
                OnStyle.onActive.textColor = Color.white;
                OnStyle.fontSize = 16;
                OnStyle.fontStyle = (FontStyle)0;
                OnStyle.alignment = (TextAnchor)4;
            }
            if (BtnStyle == null)
            {
                BtnStyle = new GUIStyle();
                BtnStyle.normal.background = BtnTexture;
                BtnStyle.onNormal.background = BtnTexture;
                BtnStyle.active.background = BtnPressTexture;
                BtnStyle.onActive.background = BtnPressTexture;
                BtnStyle.normal.textColor = Color.white;
                BtnStyle.onNormal.textColor = Color.white;
                BtnStyle.active.textColor = Color.white;
                BtnStyle.onActive.textColor = Color.white;
                BtnStyle.fontSize = 16;
                BtnStyle.fontStyle = (FontStyle)0;
                BtnStyle.alignment = (TextAnchor)4;
            }
        }

        public static Texture2D NewTexture2D => new Texture2D(1, 1);

        public static Texture2D BtnTexture
        {
            get
            {
                if (Object.Equals((Object)btntexture, (Object)null))
                {
                    btntexture = NewTexture2D;
                    btntexture.SetPixel(0, 0, new Color32((byte)65, (byte)65, (byte)65, byte.MaxValue));
                    btntexture.Apply();
                }
                return btntexture;
            }
        }

        public static Texture2D BtnPressTexture
        {
            get
            {
                if (Object.Equals((Object)btnpresstexture, (Object)null))
                {
                    btnpresstexture = NewTexture2D;
                    btnpresstexture.SetPixel(0, 0, new Color32((byte)130, (byte)130, (byte)130, byte.MaxValue));
                    btnpresstexture.Apply();
                }
                return btnpresstexture;
            }
        }

        public static Texture2D OnPressTexture
        {
            get
            {
                if (Object.Equals((Object)onpresstexture, (Object)null))
                {
                    onpresstexture = NewTexture2D;
                    onpresstexture.SetPixel(0, 0, new Color32((byte)0, (byte)150, (byte)0, byte.MaxValue));
                    onpresstexture.Apply();
                }
                return onpresstexture;
            }
        }

        public static Texture2D OnTexture
        {
            get
            {
                if (Object.Equals((Object)ontexture, (Object)null))
                {
                    ontexture = NewTexture2D;
                    ontexture.SetPixel(0, 0, new Color32((byte)0, (byte)100, (byte)0, byte.MaxValue));
                    ontexture.Apply();
                }
                return ontexture;
            }
        }

        public static Texture2D OffPressTexture
        {
            get
            {
                if (Object.Equals((Object)offpresstexture, (Object)null))
                {
                    offpresstexture = NewTexture2D;
                    offpresstexture.SetPixel(0, 0, new Color32((byte)150, (byte)0, (byte)0, byte.MaxValue));
                    offpresstexture.Apply();
                }
                return offpresstexture;
            }
        }

        public static Texture2D OffTexture
        {
            get
            {
                if (Object.Equals((Object)offtexture, (Object)null))
                {
                    offtexture = NewTexture2D;
                    offtexture.SetPixel(0, 0, Color.blue);
                    offtexture.Apply();
                }
                return offtexture;
            }
        }

        public static Texture2D BackTexture
        {
            get
            {
                if (Object.Equals((Object)backtexture, (Object)null))
                {
                    backtexture = NewTexture2D;
                    backtexture.SetPixel(0, 0, new Color32((byte)42, (byte)42, (byte)42, (byte)200));
                    backtexture.Apply();
                }
                return backtexture;
            }
        }

        public static Rect Window() => new Rect(((Rect)Mods.ModWindowPos).x - 5f, ((Rect)Mods.ModWindowPos).y - 5f, Mods.ModWindowWidht + 10f, 45f + (float)(40 * Mods.PageLines[Mods.PageCurrent]));

        public static Rect BtnRight12(int line) => new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht - 85.0), ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 85f, 35f);

        public static Rect LabelFull(int line) => new Rect(((Rect)Mods.ModWindowPos).x, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht - 90f, 35f);

        public static Rect LabelShorter(int line) => new Rect(((Rect)Mods.ModWindowPos).x, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht - 180f, 35f);

        public static Rect LabelShortest(int line) => new Rect(((Rect)Mods.ModWindowPos).x, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 2 - 85f - 5f, 35f);

        public static Rect BtnRight12Shortest(int line) => new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht / 2 - 85.0), ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 85f, 35f);

        public static Rect LabelShortestRight(int line) => new Rect(((Rect)Mods.ModWindowPos).x + Mods.ModWindowWidht / 2 + 5f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 2 - 95f, 35f);

        public static Rect LabelThirdLeft(int line) => new Rect(((Rect)Mods.ModWindowPos).x, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 3, 35f);

        public static Rect LabelThirdCenter(int line) => new Rect(((Rect)Mods.ModWindowPos).x + Mods.ModWindowWidht / 3 + 5f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 3 - 5f, 35f);

        public static Rect LabelThirdRight(int line) => new Rect(((Rect)Mods.ModWindowPos).x + 2 * (Mods.ModWindowWidht / 3) + 5f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 3 - 5f, 35f);

        public static Rect HalfLabelRight(int line) => new Rect(((Rect)Mods.ModWindowPos).x + Mods.ModWindowWidht / 2 - 45f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 2 - 45f, 35f);
        public static Rect HalfLaberlShorter(int line) => new Rect(((Rect)Mods.ModWindowPos).x + Mods.ModWindowWidht / 2 - 145f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 2 - 35f, 35f);
        
        public static Rect BtnLeft1(int line) => new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht - 85.0), ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 40f, 35f);
        public static Rect BtnLeft1Shorter(int line) => new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht - 175.0), ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 40f, 35f);

        public static Rect BtnLeft2(int line) => new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht - 40.0), ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 40f, 35f);
        public static Rect BtnLeft2Shorter(int line) => new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht - 130.0), ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 40f, 35f);

        public static Rect BtnPageNext(int line) => new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht - 40.0), ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 40f, 35f);

        public static Rect BtnPagePrevious(int line) => new Rect(((Rect)Mods.ModWindowPos).x, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 40f, 35f);

        public static Rect BtnPage(int line) => new Rect(((Rect)Mods.ModWindowPos).x + 45f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht - 90f, 35f);

        public static float Slider(int line, float value, float min, float max) => GUI.HorizontalSlider(new Rect((float)((double)((Rect)Mods.ModWindowPos).x + (double)Mods.ModWindowWidht - 85.0), ((Rect)Mods.ModWindowPos).y + 20f + (float)(40 * line), 85f, 35f), value, min, max);

        public static Rect TextFieldRect(int line) => new Rect(((Rect)Mods.ModWindowPos).x + 80f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 2 - 130f, 35f);
        public static Rect TextFieldRectShort(int line) => new Rect(((Rect)Mods.ModWindowPos).x + 80f, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), Mods.ModWindowWidht / 2 - 230f, 35f);

        public static Rect TextFieldLabelRect(int line) => new Rect(((Rect)Mods.ModWindowPos).x, ((Rect)Mods.ModWindowPos).y + (float)(40 * line), 80f, 35f);

        public static String TextField(int line, String text, GUIStyle style)
        {
            GUI.Label(TextFieldLabelRect(line), "Search:", style);
            return GUI.TextField(TextFieldRect(line), text, style);
        }
        public static String TextFieldShort(int line, String text, GUIStyle style)
        {
            GUI.Label(TextFieldLabelRect(line), "Search:", style);
            return GUI.TextField(TextFieldRectShort(line), text, style);
        }

    }
}
