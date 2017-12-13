using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Patches {
    [MonoMod.MonoModPatch("global::Necro.AppPackageUtil")]
    class patch_AppPackageUtil {

        public static extern void orig_Refresh();
        public static void Refresh() {
            try {
                AbraxisToolset.ATAPIManager.Create();
            } catch (System.Exception e ) {
                Debug.LogError( e );
            }
            orig_Refresh();
        }

    }
}
