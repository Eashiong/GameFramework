using System.Collections;
using System.Collections.Generic;
using CSObjectWrapEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace YourNamespace.EditorTool
{
    public class XluaBuild
    {
        [MenuItem("XLua/ Regenerated Code", false, 2)]
        public static void Regenerated()
        {
            Generator.ClearAll();
            Generator.GenAll();

        }
    }

}
