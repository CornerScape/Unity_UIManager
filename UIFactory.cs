using Szn.Framework.ConfigTable;
using UnityEngine;

namespace Szn.Framework.UI
{
    public class UIFactory : MonoBehaviour
    {
        public static UIBase Produce(string InUIName)
        {
            UIConfigTable configTable = GetConfigTable(InUIName);

            if (null == configTable)
            {
                Debug.LogError($"Can not find UI Config table named '{InUIName}'.");
                return null;
            }

            GameObject prefab = LoadUIPrefab(configTable.Path);
            if (null == prefab)
            {
                Debug.LogError($"No resource named '{InUIName}' was found under path {configTable.Path}.");
                return null;
            }

            UIBase bindBase = prefab.GetComponent<UIBase>();
            if (null == bindBase)
            {
                Debug.LogError($"Unbound 'UIBase' script on this resource could not be loaded.");
                return null;
            }

            bindBase.Handle.SelfLoaded();
            return bindBase;
        }

        private static UIConfigTable GetConfigTable(string InName)
        {
            return null;
        }

        private static GameObject LoadUIPrefab(string InPath)
        {
            return Resources.Load<GameObject>(InPath);
        }
    }

}

