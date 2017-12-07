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

        public static bool doesInstanceExist = false;

        public bool hasAdded = false;
        public static DebugConsole debugConsole;

        public const int baseID = 81378971;

        public static void Create() {
            if( doesInstanceExist )
                return;

            GameObject newObject = new GameObject();
            newObject.name = "Necropolis Mod Manager";

            newObject.AddComponent<ATAPIManager>();
            newObject.AddComponent<NecroMinimap>();

            DontDestroyOnLoad( newObject );

            ATModManager.LoadMods();
            doesInstanceExist = true;
        }

        public void Awake() {

        }

        public void Update() {

            if(LazySingletonBehavior<DebugConsole>.HasInstance ) {
                if( !hasAdded ) {
                    LazySingletonBehavior<DebugConsole>.Instance.enabled = true;
                    debugConsole = LazySingletonBehavior<DebugConsole>.Instance;
                    DebugConsole.CaptureUnityLogs = true;
                    debugConsole.gameObject.AddComponent<DebugConsoleHelper>();
                    hasAdded = true;
                }
            } else {
                hasAdded = false;
            }

            foreach( ATMod mod in ATModManager.loadedMods ) {
                try {
                    mod.Update();
                } catch( System.Exception e ) {
                    Debug.LogError( e );
                }
            }

        }

        public void OnGUI() {
            int id = 1;

            if( debugConsole != null && debugConsole.mode != DebugConsole.WindowMode.Hidden) {

                foreach( ATMod mod in ATModManager.loadedMods ) {
                    try {
                        mod.guiWindowProperties.RunGUICode( baseID + id, mod.name );
                    } catch( System.Exception e ) {
                        DebugConsole.Log( e.ToString() );
                    }

                    id++;
                }

            }

        }

        public void OnApplicationQuit() {
            foreach( ATMod mod in ATModManager.loadedMods ) {
                mod.OnApplicationExit();
            }
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
