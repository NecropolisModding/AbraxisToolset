using HBS.Text;
using Necro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbraxisToolset;
using UnityEngine;

namespace Patches {
    [MonoMod.MonoModPatch("global::Necro.DataManager")]
    class patch_DataManger {

        [MonoMod.MonoModIgnore]
        private Dictionary<string, Action<TextFieldParser, EffectDef>> geMethodParsers;
        public Dictionary<string, Action<TextFieldParser, EffectDef>> GetMethodParsers() {
            return geMethodParsers;
        }

        public extern void orig_Awake();
        public void Awake() {
            Debug.Log("Pre parser load");
            NecroHooks.preParserLoad();
            orig_Awake();
        }

    }
}
