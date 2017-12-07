using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patches {
    [MonoMod.MonoModPatch("global::HBS.Data.DataLoader")]
    class patch_DataLoader {

        private extern void orig_VerifyIntegrity(byte[] key, string path);
        private void VerifyIntegrity(byte[] key, string path) {

        }

    }
}
