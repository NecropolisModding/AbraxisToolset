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

        public WindowProperties guiWindowProperties;

        /// <summary>
        /// Called right after a .dll is loaded
        /// </summary>
        public virtual void Init() {
            guiWindowProperties = new WindowProperties() {
                id = 0,
                isOpened = true,
                windowFunction = OnGUI,
                closedFunction = OnGUIClosed,
                windowRect = new Rect( 15, 15, 100, 300 ),
            };
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

        public class WindowProperties {
            public int id;
            public bool isOpened;
            public Rect windowRect;
            private Rect closedRect;
            public GUI.WindowFunction windowFunction;
            public GUI.WindowFunction closedFunction;

            public void RunGUICode(int windowID, string windowName) {
                id = windowID;

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
