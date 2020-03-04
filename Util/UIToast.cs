using System.Collections;
using System.Collections.Generic;
using Szn.Framework.UtilPackage;
using UnityEngine;
using UnityEngine.UI;

namespace Szn.Framework.UI
{
    public enum ToastTime
    {
        Long,
        Middle,
        Short
    }

    public class UIToast : MonoBehaviour
    {
        private const float ANIM_TIME_F = .2f;
        private const float SHORT_PLAY_TIME_F = 1.2f;
        private const float MIDDLE_PLAY_TIME_F = 2.0f;
        private const float LONG_PLAY_TIME_F = 2.8f;
        private const float INTERVAL_TIME_F = .2f;

        private const float PADDING_TOP_F = 24;
        private const float PADDING_LEFT_F = 32;
        private float maxWidth;

        private bool isToastPlaying;

        private RectTransform bgRectTrans;
        private RectTransform toastRectTrans;
        private Text toastText;
        private TextGenerationSettings settings;
        private TextGenerator textGenerator;
        private CanvasGroup canvasGroup;

        private class ToastInfo
        {
            public readonly string Msg;
            public readonly ToastTime ShowTime;

            public ToastInfo(string InMsg, ToastTime InShowTime)
            {
                Msg = InMsg;
                ShowTime = InShowTime;
            }
        }

        private Queue<ToastInfo> toastQueue;

        private void Start()
        {
            Transform trans = transform;
            canvasGroup = trans.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            bgRectTrans = trans.Find("Background").GetComponent<RectTransform>();
            toastRectTrans = bgRectTrans.Find("Toast").GetComponent<RectTransform>();
            toastText = toastRectTrans.GetComponent<Text>();

            isToastPlaying = false;

            maxWidth = DeviceTools.ScreenWidth - PADDING_LEFT_F * 2;
            settings = toastText.GetGenerationSettings(new Vector2(maxWidth, DeviceTools.ScreenHeight));
            textGenerator = toastText.cachedTextGenerator;

            toastQueue = new Queue<ToastInfo>(8);
        }

        public void Show(string InMsg, ToastTime InToastTime = ToastTime.Middle)
        {
            toastQueue.Enqueue(new ToastInfo(InMsg, InToastTime));

            if (!isToastPlaying)
            {
                StartCoroutine(Play());
            }
        }

        private IEnumerator Play()
        {
            isToastPlaying = true;

            while (true)
            {
                ToastInfo toastInfo = toastQueue.Peek();
                textGenerator.Populate(toastInfo.Msg, settings);
                float width = 0;
                float height = 0;

                IList<UILineInfo> lineInfos = textGenerator.lines;
                if (textGenerator.lineCount == 1)
                {
                    IList<UICharInfo> charInfos = textGenerator.characters;
                    int count = charInfos.Count;
                    for (int i = 0; i < count; i++)
                    {
                        width += charInfos[i].charWidth;
                    }

                    height = lineInfos[0].height;
                }
                else
                {
                    IList<UICharInfo> charInfos = textGenerator.characters;
                    int count = charInfos.Count;
                    float curWidth = charInfos[0].charWidth;
                    for (int i = 1; i < count; i++)
                    {
                        if (charInfos[i - 1].charWidth > 0 && charInfos[i].charWidth < float.Epsilon)
                        {
                            if (width < curWidth) width = curWidth;

                            curWidth = 0;
                        }
                        else
                        {
                            curWidth += charInfos[i].charWidth;
                        }
                    }

                    for (int i = 0; i < textGenerator.lineCount; i++)
                    {
                        height += lineInfos[i].height;
                    }
                }

                toastText.text = toastInfo.Msg;
                if (width + PADDING_LEFT_F * 2 > maxWidth)
                {
                    width = maxWidth - PADDING_LEFT_F * 2;
                }

                toastRectTrans.sizeDelta = new Vector2(width, height);
                bgRectTrans.sizeDelta = new Vector2(width + PADDING_LEFT_F, height + PADDING_TOP_F);

                float playTime = 0;
                switch (toastInfo.ShowTime)
                {
                    case ToastTime.Long:
                        playTime = LONG_PLAY_TIME_F;
                        break;
                    case ToastTime.Middle:
                        playTime = MIDDLE_PLAY_TIME_F;
                        break;

                    case ToastTime.Short:
                        playTime = SHORT_PLAY_TIME_F;
                        break;
                }

                WaitForSeconds animWait = new WaitForSeconds(ANIM_TIME_F * .1f);
                for (int i = 0; i < 10; i++)
                {
                    canvasGroup.alpha += .1f;
                    yield return animWait;
                }

                yield return new WaitForSeconds(playTime);
                
                for (int i = 0; i < 10; i++)
                {
                    canvasGroup.alpha -= .1f;
                    yield return animWait;
                }

                toastQueue.Dequeue();

                if (toastQueue.Count > 0) yield return new WaitForSeconds(INTERVAL_TIME_F);
                else break;
            }
        }
    }
}