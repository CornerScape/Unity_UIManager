using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Szn.Framework.ConfigTable
{
    public class UIConfigTable
    {
        public readonly string Name;

        public readonly string Path;

        public UIConfigTable(string InName, string InPath)
        {
            Name = InName;
            Path = InPath;
        }
    }
}

