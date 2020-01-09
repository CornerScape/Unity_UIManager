using System.Collections;
using System.Collections.Generic;
using Szn.Framework.UtilPackage;
using UnityEngine;

namespace Szn.Framework.UI
{
    public class UIBase : MonoBehaviour
    {
        public Transform Trans { get; private set; }
        public sealed class UIAnimator
        {
            public readonly bool HasAnim;
            public readonly bool HasOpenAnim;
            public readonly bool HasCloseAnim;

            private readonly Animator anim;
            private bool isAnimPlaying;

            private WaitForSeconds waitOpenAnim;
            private WaitForSeconds waitCloseAnim;
            public UIAnimator(Transform InRoot)
            {
                HasAnim = false;
                HasOpenAnim = false;
                HasCloseAnim = false;

                anim = InRoot.Find(UIConfig.ANIM_ROOT_GAME_OBJ_NAME_S)?.GetComponent<Animator>();
                if (anim == null) return;

                Dictionary<string, AnimationClip> animClipDict = anim.GetAnimationClipDict();
                if (animClipDict.TryGetValue(UIConfig.OPEN_UI_ANIM_NAME_S, out var openAnimClip) &&
                    null != openAnimClip)
                {
                    HasAnim = true;
                    waitOpenAnim = new WaitForSeconds(openAnimClip.length);
                }

                if (animClipDict.TryGetValue(UIConfig.CLOSE_UI_ANIM_NAME_S, out var closeAnimClip) &&
                    null != closeAnimClip)
                {
                    HasCloseAnim = true;
                    waitCloseAnim = new WaitForSeconds(closeAnimClip.length);
                }

                HasAnim = HasOpenAnim || HasCloseAnim;
            }

            public IEnumerator PlayOpenAnim()
            {
                if(!HasOpenAnim) yield break;
                
                while (isAnimPlaying)
                {
                    yield return null;
                }

                isAnimPlaying = true;
                
                anim.SetTrigger(UIConfig.OPEN_UI_ANIM_NAME_S);

                yield return waitOpenAnim;

                isAnimPlaying = false;
            }

            public IEnumerator PlayCloseAnim()
            {
                if(!HasCloseAnim) yield break;
                
                while (isAnimPlaying)
                {
                    yield return null;
                }

                isAnimPlaying = true;
                
                anim.SetTrigger(UIConfig.CLOSE_UI_ANIM_NAME_S);

                yield return waitCloseAnim;

                isAnimPlaying = false;
            }
        }

        public sealed class UIBaseHandle
        {
            private readonly UIBase bindUIBase;

            public UIBaseHandle(UIBase InUIBase)
            {
                bindUIBase = InUIBase;
            }

            public void SelfLoaded()
            {
                bindUIBase.OnSelfLoaded();
            }

            public void SelfOpen(params object[] InParams)
            {
                bindUIBase.OnSelfOpen(InParams);
            }

            public void SelfEnable()
            {
                bindUIBase.OnSelfEnable();
            }

            public void SelfDisable()
            {
                bindUIBase.OnSelfDisable();
            }

            public void SelfDestroy()
            {
                bindUIBase.OnSelfDestroy();
            }
        }

        public UIBaseHandle Handle { get; private set; }
        
        public UIAnimator UIAnim{ get; private set; }

        private void Awake()
        {
            Trans = transform;
            Handle = new UIBaseHandle(this);
            UIAnim = new UIAnimator(Trans);
        }

        protected virtual void OnSelfLoaded()
        {
            Debug.LogError("Parent OnSelfStart");
        }

        protected virtual void OnSelfOpen(params object[] InParams)
        {
            Debug.LogError("Parent OnSelfOpen");
        }

        protected virtual void OnSelfEnable()
        {
            Debug.LogError("Parent OnSelfEnable");
        }

        protected virtual void OnSelfDisable()
        {
            Debug.LogError("Parent OnSelfDisable");
        }

        protected virtual void OnSelfDestroy()
        {
            Debug.LogError("Parent OnSelfDestroy");
        }
    }
}