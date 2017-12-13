using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using UnityEngine;

using HBS;
using HBS.DebugConsole;

using AbraxisToolset;
using AbraxisToolset.ModLoader;
using AbraxisToolset.Multiplayer;

using Necro;

using HBS.Scripting;
using HBS.Scripting.Attributes;
using HBS.Scripting.Reflection;

using AbraxisToolset.CSVFiles;
using AbraxisToolset.DiscordRPC;

namespace AbraxisToolset {
    public class ATAPIManager: MonoBehaviour {

        public static bool doesInstanceExist = false;
        public static ATAPIManager instance;
        public static ATMultiplayerManager multiplayerManager;

        public bool hasAdded = false;
        public static DebugConsole debugConsole;

        public const int baseID = 81378971;

        public Necropoloid.Cell currentCell;

        public static void Create() {
            if( doesInstanceExist )
                return;

            GameObject newObject = new GameObject {
                name = "Necropolis Mod Manager"
            };

            try {
                newObject.AddComponent<DiscordManager>();
                multiplayerManager = newObject.AddComponent<ATMultiplayerManager>();

                NecroHooks.postNetworkSetup += multiplayerManager.Init;

                newObject.AddComponent<ATAPIManager>();
            } catch (System.Exception e ) {
                Debug.LogError( e );
            }

            DontDestroyOnLoad( newObject );

            //Patch all CSV's.
            CSVPatcher.PatchCSVs();

            //Load DLL mods.
            ATModManager.LoadMods();
        }

        public void Awake() {
            instance = this;
            doesInstanceExist = true;
        }

        public void Update() {

            try {
                multiplayerManager.Update();
            } catch( System.Exception e ) {
                DebugConsole.LogError( e.ToString() );
            }

            if( LazySingletonBehavior<DebugConsole>.HasInstance ) {
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

            UpdateCurrentCell();
            UpdateMods();
        }

        public void UpdateCurrentCell() {
            if( NecropoloidSingleton.HasInstance && GameDirector.HasInstance) {
                if( GameDirector.Instance.CurrentLocalAvatar != null ) {
                    Vector3 position = LazySingletonBehavior<GameDirector>.Instance.CurrentLocalAvatar.Position;
                    currentCell = NecropoloidSingleton.Instance.Necropoloid.GetCellFromPosition( position );
                }
            }
        }
        public void UpdateMods() {
            foreach( ATMod mod in ATModManager.loadedMods ) {
                try {
                    mod.Update();
                } catch( System.Exception e ) {
                    DebugConsole.LogError( e.ToString() );
                }
            }
        }

        public void OnGUI() {
            int id = 1;

            if( debugConsole != null && debugConsole.mode != DebugConsole.WindowMode.Hidden ) {

                ATGUIManager.OnGUI();

                foreach( ATMod mod in ATModManager.loadedMods ) {

                    if( !mod.hasGUI ) {
                        id++;
                        continue;
                    }

                    try {
                        mod.guiWindowProperties.RunGUICode( baseID + id, mod.name );
                    } catch( System.Exception e ) {
                        DebugConsole.Log( e.ToString() );
                    }
                }

            }

        }
        public void OnApplicationQuit() {
            foreach( ATMod mod in ATModManager.loadedMods ) {
                mod.OnApplicationExit();
            }

            CSVPatcher.RestoreDefaultCSVs();
        }
    }
}
