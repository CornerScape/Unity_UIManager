using System;
using System.Collections;
using System.Collections.Generic;
using Szn.Framework.UtilPackage;
using UnityEngine;

namespace Szn.Framework.UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;

        public static UIManager Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new GameObject("UI Manager").AddComponent<UIManager>();
                }

                return instance;
            }
        }

        public static UIToast Toast { get; private set; }

        public static UIDialog Dialog { get; private set; }

        private class UIParam
        {
            public readonly UIBase HandleBase;

            public UIParam(UIBase InUIBase)
            {
                HandleBase = InUIBase;
            }
        }

        private class UIOpenParam : UIParam
        {
            public readonly object[] HandleParams;

            public UIOpenParam(UIBase InUIBase, object[] InParams)
                : base(InUIBase)
            {
                HandleParams = InParams;
            }
        }

        private Queue<UIParam> uiQueue;
        private bool isUIHandling;

        private class UINode
        {
            public UIBase CurBase;
            public UINode Next;
            public UINode Prev;
        }

        private UINode uiHistoryTail;

        private Transform uiRoot;

        private CanvasGroup maskCanvasGroup;
        private int maskTimes;

        private ScreenOrientation currentScreenOrientation;

        private void ShowMask()
        {
            Debug.Log("--- Lock --- FrameCount: " + Time.frameCount + "  Lock Count: " + maskTimes);
            ++maskTimes;
            if (maskTimes == 1)
            {
                maskCanvasGroup.alpha = 1;
                maskCanvasGroup.blocksRaycasts = true;
            }
        }

        private void HideMask()
        {
            Debug.Log("--- Unlock --- FrameCount: " + Time.frameCount + "  Lock Count: " + maskTimes);
            --maskTimes;
            if (maskTimes == 0)
            {
                maskCanvasGroup.alpha = 0;
                maskCanvasGroup.blocksRaycasts = false;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogError("Multiple ui manager found.");
                DestroyImmediate(gameObject);
            }

            Transform trans = transform;

            Toast = trans.Find("UIToast").GetComponent<UIToast>();

            Dialog = trans.Find("UIDialog").GetComponent<UIDialog>();

            maskCanvasGroup = trans.Find("UIMask").GetComponent<CanvasGroup>();
            if (null == maskCanvasGroup)
            {
                Debug.LogError("UI mask not found.");
            }

            uiRoot = trans.Find("UIRoot");
            if (null == uiRoot)
            {
                Debug.LogError("UI root");
            }

            uiQueue = new Queue<UIParam>((int) UIKey.Max);
        }

        private void Start()
        {
            currentScreenOrientation = Screen.orientation;
            DeviceTools.ScreenOrientationEvent += InOrientation =>
            {
                if (InOrientation == currentScreenOrientation) return;

                UIAdapt.UpdateData();

                foreach (UIParam uiParam in uiQueue)
                {
                    UIBase uiBase = uiParam.HandleBase;
                    if (uiBase.IsOpening)
                    {
                        uiBase.Handle.SelfAdapt();
                    }
                }
            };

            UnityGuiTest.UnityGUIAction += () =>
            {
                if (GUILayout.Button("Ok"))
                {
                    Dialog.Show("Msg", "I am dialog msg", "ok", () => { Debug.LogError("I am Ok."); });
                }

                if (GUILayout.Button("Cancel"))
                {
                    Dialog.Show("Msg", "I am dialog msg can cancel", "ok", "Cancel",
                        () => { Debug.LogError("I am Ok."); },
                        () => { Debug.LogError("I am Cancel."); });
                }
            };
        }

        public UIBase GetCurrentUI()
        {
            return uiHistoryTail?.CurBase;
        }

        private IEnumerator UIQueueUpdate()
        {
            ShowMask();
            isUIHandling = true;

            while (uiQueue.Count > 0)
            {
                UIParam handleParam = uiQueue.Dequeue();

                if (handleParam is UIOpenParam openParam)
                {
                    yield return StartCoroutine(openParam.HandleBase.Handle.SelfOpen(openParam.HandleParams));
                    if (null == uiHistoryTail)
                    {
                        uiHistoryTail = new UINode {CurBase = openParam.HandleBase, Prev = null, Next = null};
                    }
                    else
                    {
                        UINode node = new UINode
                            {CurBase = openParam.HandleBase, Prev = uiHistoryTail, Next = null};
                        uiHistoryTail.Next = node;
                        uiHistoryTail = node;
                    }
                }
                else
                {
                    yield return StartCoroutine(handleParam.HandleBase.Handle.SelfClose());
                }
            }

            yield return new WaitForEndOfFrame();

            isUIHandling = false;

            HideMask();
        }

        public UIBase OpenUI(UIKey InUIKey, bool InIsStayInMemory = true, params object[] InParams)
        {
            UIBase uiBase = UIFactory.Produce(InUIKey, uiRoot, InIsStayInMemory);

            if (null != uiBase)
            {
                uiQueue.Enqueue(new UIOpenParam(uiBase, InParams));
                if (!isUIHandling) StartCoroutine(UIQueueUpdate());
            }

            return uiBase;
        }

        public void CloseUI(UIKey InUIKey)
        {
            if (uiHistoryTail == null)
            {
                Debug.LogError("No UI currently open.");
                return;
            }

            UINode pointer;
            if (uiHistoryTail.CurBase.Key == InUIKey)
            {
                pointer = uiHistoryTail;
                uiHistoryTail = uiHistoryTail.Prev;
            }
            else
            {
                pointer = uiHistoryTail.Prev;
                while (pointer != null)
                {
                    if (pointer.CurBase.Key == InUIKey)
                    {
                        break;
                    }

                    pointer = pointer.Prev;
                }

                if (null == pointer)
                {
                    Debug.LogError($"No open ui named {InUIKey} was found.");
                    return;
                }
            }

            if (null != pointer.Prev) pointer.Prev.Next = pointer.Next;
            if (null != pointer.Next) pointer.Next.Prev = pointer.Prev;

            pointer.Prev = pointer.Next = null;

            uiQueue.Enqueue(new UIParam(pointer.CurBase));
            if (!isUIHandling) StartCoroutine(UIQueueUpdate());
        }
    }
}