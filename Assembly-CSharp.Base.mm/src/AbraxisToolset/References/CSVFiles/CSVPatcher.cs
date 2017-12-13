using Necro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using AbraxisToolset.ModLoader;
using HBS.DebugConsole;

namespace AbraxisToolset.CSVFiles {
    public class CSVPatcher {

        public static string backupPath = "/BACKUP_data/";

        public static readonly SimpleListCSV itemCSV = new SimpleListCSV();
        public static readonly SimpleListCSV creatureCSV = new SimpleListCSV();
        public static readonly SimpleListCSV gameActionsCSV = new SimpleListCSV();
        public static readonly SimpleListCSV shopListCSV = new SimpleListCSV();
        public static readonly SimpleListCSV spawnTablesCSV = new SimpleListCSV();

        public static readonly SimpleListCSV colorPalettsCSV = new SimpleListCSV();
        public static readonly SimpleListCSV BHObjectives = new SimpleListCSV();
        public static readonly SimpleListCSV animActionsCSV = new SimpleListCSV();
        public static readonly SimpleListCSV AIRegionsCSV = new SimpleListCSV();
        public static readonly SimpleListCSV AIDispositionCSV = new SimpleListCSV();
        public static readonly SimpleListCSV creditsCSV = new SimpleListCSV();
        public static readonly SimpleListCSV rigsCSV = new SimpleListCSV();

        public static readonly MultiElementListCSV variablesCSV = new MultiElementListCSV();
        public static readonly MultiElementListCSV lootTableCSV = new MultiElementListCSV();

        public static readonly Dictionary<string, ICSVFile> csvFiles = new Dictionary<string, ICSVFile>() {
            {"Items" , itemCSV},
            {"Creatures" , creatureCSV},
            {"Game Actions" , gameActionsCSV},
            {"Shop List" , shopListCSV},
            {"Spawn Tables" , spawnTablesCSV},
            {"ColorPalettes" , colorPalettsCSV},
            {"BH Objectives" , BHObjectives},
            //{"Anim Actions" , animActionsCSV},
            {"AI Regions" , AIRegionsCSV},
            {"AI Dispositions" , AIDispositionCSV},
            {"Credits" , creditsCSV},
            {"Rigs" , rigsCSV},
            {"Variables", variablesCSV},
            {"Loot Tables", lootTableCSV}
        };

        //loot tables, variables

        public static void PatchCSVs() {
            string backupFolder = ResolveBackupPath( string.Empty );
            string dataPath = ResolveResourcePath( string.Empty );

            //Back up default CSV files.
            if( !Directory.Exists( backupFolder ) ) {
                CopyFilesRecursively( new DirectoryInfo( dataPath ), new DirectoryInfo( backupFolder ) );
            }

            //Load CSV Files
            {
                foreach( KeyValuePair<string, ICSVFile> KVP in csvFiles ) {
                    string path = string.Format( "TUNING/{0}.csv", KVP.Key );
                    KVP.Value.ReadFromFile( ResolveResourcePath( path ) );
                }
            }

            //Patch CSV Data using mods
            {
                string modPath = Directory.GetParent( Application.dataPath ).FullName;
                modPath += ATModManager.MOD_DATA_PATH;
                string[] csvMods = Directory.GetFiles( modPath, "*.csv", SearchOption.AllDirectories );

                foreach( string csvPath in csvMods ) {
                    string fileName = Path.GetFileNameWithoutExtension( csvPath );

                    if( csvFiles.ContainsKey( fileName ) ) {
                        try {
                            csvFiles[fileName].PatchFromFile( csvPath );
                           // Debug.Log( string.Format( "Patched {0}.csv", fileName ) );
                        } catch( System.Exception e ) {
                            Debug.LogError( e );
                        }
                    }
                }
            }

            //Write patched CSV data to disk
            {
                foreach( KeyValuePair<string, ICSVFile> KVP in csvFiles ) {
                    string path = string.Format( "TUNING/{0}.csv", KVP.Key );
                    KVP.Value.WriteToFile( ResolveResourcePath( path ) );
                }
            }
        }

        public static void RestoreDefaultCSVs() {
            string backupFolder = ResolveBackupPath( string.Empty );
            string dataPath = ResolveResourcePath( string.Empty );

            //Delete current(patched) files
            if( Directory.Exists( dataPath ) ) {
                Directory.Delete( dataPath, true );
            }

            //Copy backup to current
            CopyFilesRecursively( new DirectoryInfo( backupFolder ), new DirectoryInfo( dataPath ) );

            //Delete backup
            if( Directory.Exists( backupFolder ) )
                Directory.Delete( backupFolder, true );

            itemCSV.Clear();
        }

        public static string ResolveBackupPath(string path) {
            return Application.streamingAssetsPath + backupPath + path;
        }
        public static string ResolveResourcePath(string path) {
            return Application.streamingAssetsPath + "/data/" + path;
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target) {
            foreach( DirectoryInfo dir in source.GetDirectories() )
                CopyFilesRecursively( dir, target.CreateSubdirectory( dir.Name ) );
            foreach( FileInfo file in source.GetFiles() )
                file.CopyTo( Path.Combine( target.FullName, file.Name ) );
        }

    }
}
