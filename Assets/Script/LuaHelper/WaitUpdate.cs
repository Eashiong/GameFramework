using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
namespace YourNamespace
{
    public class WaitForUpdate : CustomYieldInstruction

    {
        public override bool keepWaiting
        {
            get { return false; }
        }
    }
}