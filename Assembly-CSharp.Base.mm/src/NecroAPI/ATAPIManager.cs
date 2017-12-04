using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HBS;
using HBS.DebugConsole;
using Necro;
using AbraxisToolset.ModLoader;

using HBS.Scripting;
using HBS.Scripting.Attributes;
using HBS.Scripting.Reflection;

namespace AbraxisToolset {
    public class ATAPIManager: MonoBehaviour {

        public bool hasAdded = false;
        public static DebugConsole debugConsole;

        public static void Create() {
            GameObject newObject = new GameObject();
            newObject.name = "Necropolis Mod Manager";

            newObject.AddComponent<ATAPIManager>();
            newObject.AddComponent<NecroMinimap>();

            DontDestroyOnLoad( newObject );
        }

        public void Awake() {

        }

        public void Update() {

            if( !hasAdded && LazySingletonBehavior<DebugConsole>.HasInstance ) {

                LazySingletonBehavior<DebugConsole>.Instance.enabled = true;
                debugConsole = LazySingletonBehavior<DebugConsole>.Instance;
                DebugConsole.CaptureUnityLogs = true;

                debugConsole.gameObject.AddComponent<DebugConsoleHelper>();
                hasAdded = true;

                ATModManager.LoadMods();

            }

            foreach(ATMod mod in ATModManager.loadedMods ) {
                try {
                    mod.Update();
                } catch (System.Exception e ) {
                    Debug.LogError( e );
                }
            }

        }

        public void OnGUI() {

        }

        public void Start() {

        }

        [ScriptBinding]
        public void TestCommand() {

        }

        [ScriptBinding]
        public static void ToggleMinimap() {
            NecroMinimap.displayMinimap = !NecroMinimap.displayMinimap;
        }

    }
}
