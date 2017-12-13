using Necro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Patches {
    [MonoMod.MonoModPatch( "global::Necro.CharacterSelectorCamera" )]
    public class patch_CharacterSelectorCamera : CharacterSelectorCamera {

        [MonoMod.MonoModIgnore]
        public static patch_CharacterSelectorCamera Instance { get; }

        [MonoMod.MonoModIgnore]
        private HallwayAssembly currentHallway;

        public Actor GetLocalActor() {
            return currentHallway.LocalAvatar;
        }

        [MonoMod.MonoModIgnore]
        private class HallwayAssembly {
            [MonoMod.MonoModIgnore]
            public Actor LocalAvatar { get; }
        }

    }
}