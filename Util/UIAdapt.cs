using UnityEngine;

namespace Szn.Framework.UI
{
    public static class UIAdapt
    {
        private static readonly Vector2 halfVector2 = new Vector2(.5f, .5f);
        private static Vector2 contentOffsetMin;
        private static Vector2 contentOffsetMax;
        private static Vector3 bgLocalScale;

        public static void UpdateData()
        {
            float width = Screen.width;
            float height = Screen.height;

            Rect safeAreaRect = Screen.safeArea;

            float widthScale = width / UIConfig.DESIGN_WIDTH_I;
            float heightScale = height / UIConfig.DESIGN_HEIGHT_I;

            float minScale = widthScale < heightScale ? widthScale : heightScale;
            float maxScale = widthScale + heightScale - minScale;

            float reciprocalCurrentScale = 1 / minScale;

            contentOffsetMin = new Vector2(UIConfig.UI_CONTENT_OFFSET_LEFT_I + safeAreaRect.x * reciprocalCurrentScale,
                UIConfig.UI_CONTENT_OFFSET_BOTTOM_I + safeAreaRect.y * reciprocalCurrentScale);

            contentOffsetMax =
                new Vector2(
                    -(UIConfig.UI_CONTENT_OFFSET_RIGHT_I +
                      (width - safeAreaRect.width - safeAreaRect.x) * reciprocalCurrentScale),
                    -(UIConfig.UI_CONTENT_OFFSET_TOP_I +
                      (height - safeAreaRect.height - safeAreaRect.y) * reciprocalCurrentScale));

            float scale = maxScale * reciprocalCurrentScale;
            bgLocalScale = new Vector3(scale, scale, scale);
        }

        public static void ContentAdapt(this RectTransform InContentRectTrans)
        {
            if (InContentRectTrans == null)
            {
                Debug.LogError("Connect Adapt need a RectTransform component!");
                return;
            }

            InContentRectTrans.anchorMin = Vector2.zero;
            InContentRectTrans.anchorMax = Vector2.one;

            InContentRectTrans.offsetMin = contentOffsetMin;
            InContentRectTrans.anchorMax = contentOffsetMax;
        }

        public static void BackgroundAdapt(this RectTransform InBgRectTrans)
        {
            if (InBgRectTrans == null)
            {
                Debug.LogError("Background Adapt need a RectTransform component!");
                return;
            }

            InBgRectTrans.anchorMin = halfVector2;
            InBgRectTrans.anchorMax = halfVector2;

            InBgRectTrans.localScale = bgLocalScale;
        }
    }
}