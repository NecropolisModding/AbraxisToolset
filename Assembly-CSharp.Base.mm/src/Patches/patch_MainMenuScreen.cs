using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Necro.UI;

namespace Patches {
    [MonoMod.MonoModPatch( "global::Necro.UI.Menu_MainMenu" )]
    class patch_MainMenuScreen : Menu_MainMenu{

        private extern void orig_Awake();
        public void Awake() {
            orig_Awake();
        }

    }
}
