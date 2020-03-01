using UnityEngine;

namespace Szn.Framework.UI
{
    public enum UIKey
    {
        A,
        B,
        C,
        D,
        E,
        Max
    }
    
    public static class UIConfig
    {
        public const int DESIGN_WIDTH_I = 720;
        public const int DESIGN_HEIGHT_I = 1280;

        public const int UI_CONTENT_OFFSET_LEFT_I = 0;
        public const int UI_CONTENT_OFFSET_RIGHT_I = 0;
        public const int UI_CONTENT_OFFSET_TOP_I = 0;
        public const int UI_CONTENT_OFFSET_BOTTOM_I = 0;

        public const string ANIM_ROOT_GAME_OBJ_NAME_S = "Panel";
        public const string OPEN_UI_ANIM_NAME_S = "On";
        public const string CLOSE_UI_ANIM_NAME_S = "Off";

        public const string CONTENT_ROOT_GAME_OBJ_NAME_S = "Content";
        public const string BACKGROUND_ROOT_GAME_OBJ_NAME_S = "Background";
    }
}