using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using System.Reflection;

using HBS.Scripting;
using HBS.Scripting.Attributes;
using HBS.Scripting.Reflection;
using HBS.DebugConsole;

namespace AbraxisToolset.ModLoader {
    public class ATModManager {

        public const string MOD_DATA_PATH = "/mods";
        public static List<ATMod> loadedMods = new List<ATMod>();

        public static void LoadMods() {

            string modPath = Directory.GetParent( Application.dataPath ).FullName;
            modPath += MOD_DATA_PATH;

            //Create mods folder
            if( !Directory.Exists( modPath ) ) {
                Directory.CreateDirectory( modPath );
            }

            //Get all mods
            string[] modPaths = Directory.GetFiles( modPath, "*.dll" );
            Assembly[] readMods = new Assembly[modPaths.Length];

            //Cache the type that classes inherit from
            Type modType = typeof( ATMod );

            //loadedMods.Add( new TestMod() );
            //loadedMods[0].Init();

            //Loop through all found .dll's
            for( int i = 0; i < modPaths.Length; i++ ) {

                //Get the current mod path
                string path = modPaths[i];

                //Try to load the mod
                try {

                    //Load the assembly from file, and get it's types
                    Assembly assembly = Assembly.LoadFile( path );
                    Type[] classTypes = assembly.GetTypes();

                    //Loop through types
                    foreach( Type classType in classTypes ) {
                        //If the class inherits from the mod type
                        if( classType.BaseType == modType ) {
                            //Create the mod and call Init() and OnLoad()
                            ATMod mod = (ATMod)Activator.CreateInstance( classType );
                            mod.Init();
                            loadedMods.Add( mod );
                        }
                    }
                } catch( System.Exception e ) {
                    Debug.Log( e );
                }

            }

            Debug.Log(loadedMods.Count + " mods loaded");

            //loadedMods[0].OnLoad();

            foreach( ATMod mod in loadedMods ) {
                mod.OnLoad();
            }


        }

    }
}
