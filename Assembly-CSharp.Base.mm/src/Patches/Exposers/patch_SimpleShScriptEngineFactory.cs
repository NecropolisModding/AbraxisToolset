using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HBS.Scripting.Attributes;
using HBS.Scripting.Reflection;
using HBS.Scripting;

namespace Patches {
    [MonoMod.MonoModPatch( "global::HBS.Scripting.SimpleSh.SimpleShScriptEngineFactory" )]
    public class patch_SimpleShScriptEngineFactory {

        [MonoMod.MonoModIgnore]
        public static IBindings globalBindings;

        public static IBindings GetGlobalBindings {
            get {
                return globalBindings;
            }
        }

    }
}
