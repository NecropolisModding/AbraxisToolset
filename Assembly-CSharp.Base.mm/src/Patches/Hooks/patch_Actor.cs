using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbraxisToolset;
using Necro;

namespace Patches {
    /*[MonoMod.MonoModPatch( "global::Necro.Actor" )]
    class patch_Actor: Necro.Actor {

        public void Update() {
            NecroHooks.OnActorUpdate(this);
        }

        public extern void orig_Init(ActorDef actorDef, int actorLevel, ActorBody body, bool initViaSpawn, SessionDataActor sda = null);
        public void Init(ActorDef actorDef, int actorLevel, ActorBody body, bool initViaSpawn, SessionDataActor sda = null) {
            NecroHooks.OnActorSpawned(this);
            orig_Init(actorDef, actorLevel, body, initViaSpawn, sda);
        }

        public extern void orig_Kill(bool loot = true, bool force = false, Actor killer = null, bool deathSequence = true, bool fromNetwork = false);
        public void Killbool (bool loot = true, bool force = false, Actor killer = null, bool deathSequence = true, bool fromNetwork = false) {
            NecroHooks.OnActorKilled(this);
            orig_Kill(loot, force, killer, deathSequence, fromNetwork);
        }

        public extern void orig_Despawn();
        public void Despawn() {
            NecroHooks.OnActorPreDespawn(this);
            orig_Despawn();
        }

    }*/
}
