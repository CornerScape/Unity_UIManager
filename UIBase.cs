using System.Collections;
using System.Collections.Generic;
using Szn.Framework.UtilPackage;
using UnityEngine;

namespace Szn.Framework.UI
{
    public class UIBase : MonoBehaviour
    {
        public GameObject GameObj { get; private set; }
        public Transform Trans { get; private set; }
        
        public UIKey Key { get; private set; }

        public sealed class UIBaseHandle
        {
            private readonly UIBase bindUIBase;
            private readonly bool hasOpenAnim;
            private readonly bool hasCloseAnim;

            private readonly Animator anim;
            private bool isAnimPlaying;

            private readonly WaitForSeconds waitOpenAnim;
            private readonly WaitForSeconds waitCloseAnim;
            private readonly CanvasGroup canvasGroup;
            public UIBaseHandle(UIBase InUIBase, Transform InRoot)
            {
                bindUIBase = InUIBase;
                hasOpenAnim = false;
                hasCloseAnim = false;

                canvasGroup = InRoot.GetComponent<CanvasGroup>();
                if (null == canvasGroup)
                {
                    Debug.LogError("Canvas Group Not Found.");
                }
                
                Debug.LogError($"get canvas group = {canvasGroup == null}");
                
                anim = InRoot.Find(UIConfig.ANIM_ROOT_GAME_OBJ_NAME_S)?.GetComponent<Animator>();
                if (anim == null) return;

                Dictionary<string, AnimationClip> animClipDict = anim.GetAnimationClipDict();
                if (animClipDict.TryGetValue(UIConfig.OPEN_UI_ANIM_NAME_S, out var openAnimClip) &&
                    null != openAnimClip)
                {
                    hasOpenAnim = true;
                    waitOpenAnim = new WaitForSeconds(openAnimClip.length);
                }

                if (animClipDict.TryGetValue(UIConfig.CLOSE_UI_ANIM_NAME_S, out var closeAnimClip) &&
                    null != closeAnimClip)
                {
                    hasCloseAnim = true;
                    waitCloseAnim = new WaitForSeconds(closeAnimClip.length);
                }
            }
            
            public void SelfLoaded(UIKey InUIKey)
            {
                bindUIBase.Key = InUIKey;
                bindUIBase.OnSelfLoaded();
            }

            public IEnumerator SelfOpen(params object[] InParams)
            {
                if (!hasOpenAnim)
                {
                    bindUIBase.OnSelfBeginOpen(InParams);
                    canvasGroup.alpha = 1;
                    canvasGroup.blocksRaycasts = true;
                    bindUIBase.OnSelfOpened();
                    yield break;
                }

                while (isAnimPlaying)
                {
                    yield return null;
                }

                bindUIBase.OnSelfBeginOpen(InParams);
                
                isAnimPlaying = true;

                anim.SetTrigger(UIConfig.OPEN_UI_ANIM_NAME_S);

                yield return waitOpenAnim;

                isAnimPlaying = false;
                
                bindUIBase.OnSelfOpened();
            }

            public IEnumerator SelfClose()
            {
                if (!hasCloseAnim)
                {
                    bindUIBase.OnSelfBeginClose();
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = false;
                    bindUIBase.OnSelfClosed();
                    yield break;
                }

                while (isAnimPlaying)
                {
                    yield return null;
                }

                bindUIBase.OnSelfBeginClose();
                
                isAnimPlaying = true;

                anim.SetTrigger(UIConfig.CLOSE_UI_ANIM_NAME_S);

                yield return waitCloseAnim;

                isAnimPlaying = false;
                
                bindUIBase.OnSelfClosed();
            }
            
            public void SelfHierarchyEnable()
            {
                bindUIBase.OnSelfHierarchyEnable();
            }

            public void SelfHierarchyDisable()
            {
                bindUIBase.OnSelfHierarchyDisable();
            }

            public void SelfDestroy()
            {
                bindUIBase.OnSelfDestroy();
            }
        }

        public UIBaseHandle Handle { get; private set; }


        private void Awake()
        {
            GameObj = gameObject;
            
            Trans = transform;

            Handle = new UIBaseHandle(this, Trans);
            
            Debug.LogError("<----------->");
        }

        protected virtual void OnSelfLoaded()
        {
            Debug.LogError("Parent OnSelfStart");
        }

        protected virtual void OnSelfBeginOpen(params object[] InParams)
        {
            Debug.LogError("Parent OnSelfOpen");
        }

        protected virtual void OnSelfOpened()
        {
            Debug.LogError("Parent OnSelfOpened");
        }

        protected virtual void OnSelfBeginClose()
        {
            Debug.LogError("Parent OnSelfBeginClose");
        }

        protected virtual void OnSelfClosed()
        {
            Debug.LogError("Parent OnSelfClosed");
        }

        protected virtual void OnSelfHierarchyEnable()
        {
            Debug.LogError("Parent OnSelfHierarchyEnable");
        }

        protected virtual void OnSelfHierarchyDisable()
        {
            Debug.LogError("Parent OnSelfHierarchyDisable");
        }
        
        protected virtual void OnSelfDestroy()
        {
            Debug.LogError("Parent OnSelfDestroy");
        }
    }
}