using AbraxisToolset.Multiplayer;
using Necro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using HBS;
using HBS.Nav;
using AbraxisToolset.ModLoader;

namespace AbraxisToolset {
    public class ATGUIManager {

        public static Rect windowRect = new Rect( 15, 15, 300, 500 );
        public static Vector2 windowScroll = new Vector2();
        public static Vector2 enabledScroll = new Vector2();
        public static Vector2 disabledScroll = new Vector2();

        public static WindowTab[] tabs = new WindowTab[]{
                new WindowTab() { function = ModsTab, tabName = "Mods" },
                new WindowTab() { function = OptionsTab, tabName = "Options" },
                new WindowTab() { function = DebugTab, tabName = "Debug" },
        };
        public static int openedIndex = 0;

        public static void OnGUI() {
            windowRect = GUI.Window( 84633563, windowRect, WindowGUI, "Abraxis Toolset" );
        }
        private static void WindowGUI(int id) {
            windowScroll = GUILayout.BeginScrollView( windowScroll );
            {
                GUILayout.BeginHorizontal();
                for( int i = 0; i < tabs.Length; i++ ) {
                    if( GUILayout.Button( tabs[i].tabName ) ) {
                        OpenTab( i );
                        break;
                    }
                }
                GUILayout.EndHorizontal();

                tabs[openedIndex].function();
            }
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
        public static void OpenTab(int index) {
            if( index != openedIndex ) {
                openedIndex = index;
                for( int i = 0; i < tabs.Length; i++ ) {
                    tabs[i].isOpened = index == i;
                }
            }
        }

        public static void ModsTab() {
            return;

            //Enabled mods
            {
                CenterLabel( "Enabled Mods" );
                enabledScroll = GUILayout.BeginScrollView( enabledScroll, "Box", GUILayout.Height( 150 ) );

                foreach( ATMod mod in ATModManager.loadedMods ) {
                    if( !mod.isEnabled )
                        continue;

                    if( GUILayout.Toggle(false, mod.name) ) {
                        mod.Disable();
                    }
                }

                GUILayout.EndScrollView();
            }

            //Disabled mods
            {
                CenterLabel( "Disabled Mods" );
                disabledScroll = GUILayout.BeginScrollView( disabledScroll, "Box", GUILayout.Height( 150 ) );

                foreach( ATMod mod in ATModManager.loadedMods ) {
                    if( mod.isEnabled )
                        continue;

                    if( GUILayout.Toggle( false, mod.name ) ) {
                        mod.Enable();
                    }
                }

                GUILayout.EndScrollView();
            }
        }
        public static void OptionsTab() {

        }
        public static void DebugTab() {
            GUILayout.Label("Main Menu:" + Necro.UI.Menu_MainMenu.HasInstance);
        }

        public static void CenterLabel(string txt) {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label( txt, GUILayout.ExpandWidth( false ) );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public class WindowTab {
            public string tabName = "Tab";
            public bool isOpened = false;
            public Action function;
        }
    }
}
