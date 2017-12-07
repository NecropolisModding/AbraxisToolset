using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patches {
    [MonoMod.MonoModPatch("global::Necro.AppPackageUtil")]
    class patch_AppPackageUtil {

        public static extern void orig_Refresh();
        public static void Refresh() {
            AbraxisToolset.ATAPIManager.Create();
            orig_Refresh();
        }

    }
}
