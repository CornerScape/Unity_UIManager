using UnityEngine;

namespace Szn.Framework.UI
{
    public class UITest : UIBase
    {
        protected override void OnSelfLoaded()
        {
            base.OnSelfLoaded();
            Debug.LogError("Child Start...");
        }

        protected override void OnSelfOpen(params object[] InParams)
        {
            base.OnSelfOpen(InParams);
            Debug.LogError("Child Open...");
        }

        protected override void OnSelfEnable()
        {
            base.OnSelfEnable();
            Debug.LogError("Child Enable...");
        }

        protected override void OnSelfDisable()
        {
            base.OnSelfDisable();
            Debug.LogError("Child Disable...");
        }

        protected override void OnSelfDestroy()
        {
            base.OnSelfDestroy();
            Debug.LogError("Child Destroy...");
        }
    }
}

