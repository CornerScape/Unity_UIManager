using System.Collections.Generic;
using Szn.Framework.ConfigTable;
using UnityEngine;

namespace Szn.Framework.UI
{
    public static class UIFactory
    {
        private static readonly Dictionary<int, UIConfigTable> _uiConfigTables = new Dictionary<int, UIConfigTable>
        {
            {0, new UIConfigTable("A", "UI/A")},
            {1, new UIConfigTable("B", "UI/B")},
            {2, new UIConfigTable("C", "UI/C")},
            {3, new UIConfigTable("D", "UI/D")},
            {4, new UIConfigTable("E", "UI/E")}
        };
        private static readonly Dictionary<UIKey, UIBase> _uiBasePools = new Dictionary<UIKey, UIBase>(new UIKeyEqualityComparer());

        public static UIBase Produce(UIKey InUIKey, Transform InParent, bool InIsStayInMemory = true)
        {
            if (_uiBasePools.TryGetValue(InUIKey, out var bindBase))
            {
                if (null != bindBase)
                {
                    bindBase.Trans.SetAsLastSibling();
                    return bindBase;
                }
            }

            UIConfigTable configTable = GetConfigTable((int) InUIKey);

            if (null == configTable)
            {
                Debug.LogError($"Can not find UI Config table named '{InUIKey}'.");
                return null;
            }

            GameObject prefab = Object.Instantiate(LoadUIPrefab(configTable.Path));
            if (null == prefab)
            {
                Debug.LogError($"No resource named '{InUIKey}' was found under path {configTable.Path}.");
                return null;
            }

            Transform trans = prefab.transform;
            trans.SetParent(InParent);
            trans.localPosition = Vector3.zero;
            trans.rotation = Quaternion.identity;
            trans.localScale = Vector3.one;
            trans.SetAsLastSibling();

            bindBase = prefab.GetComponent<UIBase>();
            if (null == bindBase)
            {
                Debug.LogError($"Unbound 'UIBase' script on this resource could not be loaded.");
                return null;
            }

            if (InIsStayInMemory) _uiBasePools.Add(InUIKey, bindBase);
            
            bindBase.Handle.SelfLoaded(InUIKey);

            return bindBase;
        }

        public static void Destroy(UIKey InUIKey)
        {
            if (_uiBasePools.TryGetValue(InUIKey, out var uiBase))
            {
                _uiBasePools.Remove(InUIKey);

                if (null != uiBase) Object.DestroyImmediate(uiBase.GameObj);
            }
        }

        public static void DestroyAll()
        {
            foreach (KeyValuePair<UIKey,UIBase> keyValuePair in _uiBasePools)
            {
                if(null != keyValuePair.Value) Object.DestroyImmediate(keyValuePair.Value.GameObj);
            }
            
            _uiBasePools.Clear();
        }

        private static UIConfigTable GetConfigTable(int InId)
        {
            return _uiConfigTables[InId];
        }

        private static GameObject LoadUIPrefab(string InPath)
        {
            return Resources.Load<GameObject>(InPath);
        }
    }

    public class UIKeyEqualityComparer : IEqualityComparer<UIKey>
    {
        public bool Equals(UIKey InX, UIKey InY)
        {
            return (int) InX == (int) InY;
        }

        public int GetHashCode(UIKey InObj)
        {
            return (int) InObj;
        }
    }
}