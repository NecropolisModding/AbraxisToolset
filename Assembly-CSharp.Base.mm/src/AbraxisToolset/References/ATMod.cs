using HBS.DebugConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AbraxisToolset {
    public class ATMod {

        public string name = "modname";
        public string version = "version";

        public bool isEnabled = true;
        public bool hasGUI = false;

        public WindowProperties guiWindowProperties;

        public void PreInit() {
            guiWindowProperties = new WindowProperties() {
                id = 0,
                isOpened = true,
                windowFunction = OnGUI,
                closedFunction = OnGUIClosed,
                windowRect = new Rect( 15, 15, 100, 300 ),
            };
        }

        /// <summary>
        /// Called right after a .dll is loaded
        /// </summary>
        public virtual void Init() {

        }

        /// <summary>
        /// Called after all mods have called "Init"
        /// </summary>
        public virtual void OnLoad() {

        }

        /// <summary>
        /// GUI Code
        /// </summary>
        /// <param name="windowID">The window ID for this mod's GUI window</param>
        public virtual void OnGUI(int windowID) {

        }

        /// <summary>
        /// GUI Code
        /// </summary>
        /// <param name="windowID">The window ID for this mod's GUI window</param>
        public virtual void OnGUIClosed(int windowID) {

        }

        /// <summary>
        /// Called once per frame
        /// </summary>
        public virtual void Update() {

        }

        /// <summary>
        /// Called when the game exits
        /// </summary>
        public virtual void OnApplicationExit() {

        }

        /// <summary>
        /// Called when a mod is disabled
        /// </summary>
        public virtual void OnDisable() {

        }

        /// <summary>
        /// Called when a mod is enabled
        /// </summary>
        public virtual void OnEnable() {

        }

        public void Disable() {
            if( isEnabled == true ) {
                isEnabled = false;
                OnDisable();
            }
        }

        public void Enable() {
            if( isEnabled == false ) {
                isEnabled = true;
                OnEnable();
            }
        }

        public class WindowProperties {
            public int id;
            public bool isOpened;
            public Rect windowRect = new Rect( 15, 15, 100, 300 );
            private Rect closedRect = new Rect( 15, 15, 100, 300 );
            public GUI.WindowFunction windowFunction = delegate (int id) { };
            public GUI.WindowFunction closedFunction = delegate (int id) { };

            public void RunGUICode(int windowID, string windowName) {

                if( !isOpened ) {
                    closedRect.size = new Vector2( windowRect.width, 60 );
                    Rect newRect = GUI.Window( id, closedRect, WindowGUICodeClosed, windowName );

                    if( closedRect.Contains( Input.mousePosition ) )
                        GUI.FocusWindow( windowID );

                    closedRect = newRect;
                    windowRect.position = newRect.position;
                } else {
                    closedRect.size = new Vector2( windowRect.width, 60 );
                    Rect newRect = GUI.Window( id, windowRect, WindowGUICodeOpened, windowName );

                    if( closedRect.Contains( Input.mousePosition ) )
                        GUI.FocusWindow( windowID );

                    windowRect = newRect;
                    closedRect.position = newRect.position;
                }


            }

            public void WindowGUICodeClosed(int id) {
                if( GUI.Button( new Rect( 2, 2, 18, 18 ), "_" ) ) {
                    isOpened = !isOpened;
                }
                closedFunction( id );
                GUI.DragWindow();
            }

            public void WindowGUICodeOpened(int id) {
                if( GUI.Button( new Rect( 2, 2, 18, 18 ), "_" ) ) {
                    isOpened = !isOpened;
                }
                windowFunction( id );
                GUI.DragWindow();
            }
        }

    }
}
