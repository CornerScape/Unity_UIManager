using UnityEngine;

namespace Szn.Framework.UI
{
    public class UIBase : MonoBehaviour
    {
        public sealed class UIAnimator
        {
            
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

        private UIBaseHandle handle;
        public UIBaseHandle Handle => handle ?? (handle= new UIBaseHandle(this));


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