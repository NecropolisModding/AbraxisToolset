using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AbraxisToolset {
    public static class NecroHooks {

        /// <summary>
        /// This hook is called whenever a level finishes loading.
        /// </summary>
        public static NecroHookFunction onCellsFinished = delegate (object[] args) { };
        public static NecroHookFunction onCellCreated = delegate (object[] args) { };

        /// <summary>
        /// This hook is called for every actor, on each actor's update.
        /// </summary>
        public static NecroHookFunction OnActorUpdate = delegate (object[] args) { };
        public static NecroHookFunction OnActorKilled = delegate (object[] args) { };
        public static NecroHookFunction OnActorSpawned = delegate (object[] args) { };
        public static NecroHookFunction OnActorPreDespawn = delegate (object[] args) { };


        public static NecroHookFunction preParserLoad = delegate (object[] args) { Debug.Log( "AT Mod : Pre Parsers Load" ); };
        public static NecroHookFunction postNetworkSetup = delegate (object[] args) { Debug.Log( "AT Mod : Post Network Setup" ); };

        public delegate void NecroHookFunction(params object[] args);

    }
}
