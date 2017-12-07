using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AbraxisToolset {
    public class TestMod: ATMod {

        public override void Init() {
            base.Init();
            this.name = "Test Mod";
            this.version = "0.1";
        }

        public override void OnGUI(int windowID) {
            base.OnGUI( windowID );

            GUILayout.Label("Words");
        }
    }
}
