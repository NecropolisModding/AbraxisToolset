using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patches {
    [MonoMod.MonoModPatch("global::SteamManager")]
    public class patch_SteamManager {

        [MonoMod.MonoModIgnore]
        public static bool Initialized { get; }

    }
}
