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

        protected override void OnSelfBeginOpen(params object[] InParams)
        {
            base.OnSelfBeginOpen(InParams);
            Debug.LogError("Child Open...");
        }

        protected override void OnSelfHierarchyEnable()
        {
            base.OnSelfHierarchyEnable();
            
            Debug.LogError("Child Enable...");
        }

        protected override void OnSelfHierarchyDisable()
        {
            base.OnSelfHierarchyDisable();

            Debug.LogError("Child Disable...");
        }

        protected override void OnSelfDestroy()
        {
            base.OnSelfDestroy();
            Debug.LogError("Child Destroy...");
        }
    }
}

