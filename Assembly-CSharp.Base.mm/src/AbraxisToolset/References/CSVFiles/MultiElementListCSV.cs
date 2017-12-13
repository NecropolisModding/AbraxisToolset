using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HBS.Text;
using HBS.DebugConsole;
using UnityEngine;

namespace AbraxisToolset.CSVFiles {
    public class MultiElementListCSV: ICSVFile {
        public Dictionary<string, MultiElementEntry> entries = new Dictionary<string, MultiElementEntry>();
        public MultiElementEntry defEntry;

        //ID prefixes
        public const string patchAddPrefix = "patchAdd";
        public const string patchOverPrefix = "patchOver";

        //Element Tags
        public const string elementRemoveTag = "[remove]";

        public void ReadFromFile(string path) {
            if( !File.Exists( path ) )
                throw new System.IO.FileNotFoundException( "File not found at " + path );
            string[] lines = File.ReadAllLines( path );
            string fileName = Path.GetFileNameWithoutExtension( path );

            //Defenition entry
            {
                //Split by commas
                List<string> seperatedLines = new List<string>();
                StringUtil.SplitCSV( lines[0], seperatedLines );

                //Set entry
                defEntry = new MultiElementEntry() {
                    firstValues = seperatedLines.ToArray()
                };
            }

            int emptyCount = 0;
            int noLineCount = 0;

            MultiElementEntry currentEntry = null;
            List<string[]> elements = new List<string[]>();

            //Add entries
            for( int i = 1; i < lines.Length; i++ ) {
                if( lines[i].Length == 0 )
                    continue;
                List<string> seperatedLines = new List<string>();
                StringUtil.SplitCSV( lines[i], seperatedLines );

                if( seperatedLines.Count <= 1 ) {
                    noLineCount++;
                    continue;
                }

                bool isEmpty = true;
                foreach( string s in seperatedLines ) {
                    if( s != string.Empty ) {
                        isEmpty = false;
                        break;
                    }
                }
                if( isEmpty ) {
                    emptyCount++;
                    continue;
                }

                //If the first element isn't empty, we're starting a new list.
                if( seperatedLines[0] != string.Empty ) {
                    if( currentEntry != null ) {
                        currentEntry.otherEntries = elements.ToArray();
                        elements.Clear();

                        entries[currentEntry.ID] = currentEntry;
                    }
                    currentEntry = new MultiElementEntry() {
                        firstValues = seperatedLines.ToArray(),
                    };
                }
                //If the first element IS empty, we're adding to the current list.
                else {
                    elements.Add( seperatedLines.ToArray() );
                }
            }

            if( currentEntry != null ) {
                currentEntry.otherEntries = elements.ToArray();
                elements.Clear();
                entries[currentEntry.ID] = currentEntry;
            }
        }
        public void WriteToFile(string path) {
            try {
                List<string> lines = new List<string>();
                string fileName = Path.GetFileNameWithoutExtension( path );

                //Def entry
                {
                    string line = string.Empty;
                    //Append values
                    foreach( string s in defEntry.firstValues ) {
                        line += s + ',';
                    }
                    //Append new line
                    lines.Add( line );
                }


                foreach( MultiElementEntry entry in entries.Values ) {
                    try {
                        string line = string.Empty;
                        List<string> components = new List<string>();
                        //Append first values
                        foreach( string s in entry.firstValues ) {
                            StringUtil.SplitCSV( s, components );
                            if( components.Count > 1 ) {
                                line += '"' + s + '"' + ',';
                            } else {
                                line += s + ',';
                            }
                        }
                        //Append new line
                        lines.Add( line );

                        //Append other values
                        foreach( string[] otherS in entry.otherEntries ) {
                            line = string.Empty;
                            foreach( string s in otherS ) {
                                StringUtil.SplitCSV( s, components );
                                if( components.Count > 1 ) {
                                    line += '"' + s + '"' + ',';
                                } else {
                                    line += s + ',';
                                }
                            }
                            lines.Add( line );
                        }
                    } catch( System.Exception e ) {
                        Debug.LogError( e );
                        Debug.LogError( "Failed to write entry " + entry.ID + " to file" );
                    }
                }

                Debug.Log( string.Format( "Writing {0} lines to file {1}, out of {2} entries", lines.Count, fileName, entries.Values.Count ) );
                File.WriteAllLines( path, lines.ToArray() );
            } catch( System.Exception e ) {
                Debug.LogError( e );
            }
        }
        public void PatchFromFile(string path) {
            if( !File.Exists( path ) )
                throw new System.IO.FileNotFoundException( "File not found at " + path );
            string[] lines = File.ReadAllLines( path );

            List<MultiElementEntry> patchEntries = new List<MultiElementEntry>();
            MultiElementEntry currentEntry = null;
            List<string[]> elements = new List<string[]>();

            //Parse multi-element entries
            for( int i = 0; i < lines.Length; i++ ) {
                if( lines[i].Length == 0 )
                    continue;
                List<string> seperatedLines = new List<string>();
                StringUtil.SplitCSV( lines[i], seperatedLines );

                if( seperatedLines.Count <= 1 )
                    continue;

                bool isEmpty = true;
                foreach( string s in seperatedLines ) {
                    if( s != string.Empty ) {
                        isEmpty = false;
                        break;
                    }
                }
                if( isEmpty )
                    continue;

                //If the first element isn't empty, we're starting a new list.
                if( seperatedLines[0] != string.Empty ) {
                    if( currentEntry != null ) {
                        currentEntry.otherEntries = elements.ToArray();
                        elements.Clear();

                        patchEntries.Add( currentEntry );
                    }
                    currentEntry = new MultiElementEntry() {
                        firstValues = seperatedLines.ToArray(),
                    };
                }
                //If the first element IS empty, we're adding to the current list.
                else {
                    elements.Add( seperatedLines.ToArray() );
                }
            }

            if( currentEntry != null ) {
                currentEntry.otherEntries = elements.ToArray();
                elements.Clear();
                patchEntries.Add( currentEntry );
            }

            //Patch entries
            foreach( MultiElementEntry patch in patchEntries ) {
                string idWithoutPrefix = ATCSVUtil.GetWithoutPrefix( patch.ID );
                string prefix = ATCSVUtil.GetPrefix( patch.ID );

                if( prefix == patchOverPrefix ) {
                    PatchOver( patch, idWithoutPrefix );
                } else if( prefix == patchAddPrefix ) {
                    PatchAdd( patch, idWithoutPrefix );
                } else {
                    Debug.Log( string.Format( "Patching failed with entry {0}, no prefixes where matched.", patch.ID ) );
                }
            }
        }

        public void AddEntry(MultiElementEntry entry) {
            try {
                //Add entry to entry dictionary
                entries[entry.ID] = entry;
            } catch( System.Exception e ) {
                Debug.LogError( e );
            }
        }
        public void Clear() {
            entries.Clear();
            defEntry = null;
        }

        public void PatchAdd(MultiElementEntry newEntry, string entryIDWithoutPrefix) {
            //If there's no entry to patch, just add this entry.
            if( !entries.ContainsKey( entryIDWithoutPrefix ) ) {
                //Set ID to be the non-prefix version
                newEntry.firstValues[0] = entryIDWithoutPrefix;

                //Add the entry
                AddEntry( newEntry );
            } else {

                //Get Item Entry to add to
                MultiElementEntry patchEntry = entries[entryIDWithoutPrefix];

                //Add first entry to existing entry
                for( int i = 1; i < newEntry.firstValues.Length; i++ ) {
                    patchEntry.firstValues[i] += newEntry.firstValues[i];
                }

                //Add other entries
                for( int i = 0; i < newEntry.otherEntries.Length; i++ ) {
                    for( int j = 0; j < newEntry.otherEntries[i].Length; j++ ) {
                        patchEntry.otherEntries[i][j] += newEntry.otherEntries[i][j];
                    }
                }

                entries[entryIDWithoutPrefix] = patchEntry;
            }
        }
        public void PatchOver(MultiElementEntry newEntry, string entryIDWithoutPrefix) {
            //If there's no entry to patch, just add this entry.
            if( !entries.ContainsKey( entryIDWithoutPrefix ) ) {
                //Set ID to be the non-prefix version
                newEntry.firstValues[0] = entryIDWithoutPrefix;

                //Add the entry
                AddEntry( newEntry );
            } else {

                //Get Item Entry to add to
                MultiElementEntry patchEntry = entries[entryIDWithoutPrefix];

                //Patch over existing entries
                for( int i = 1; i < newEntry.firstValues.Length; i++ ) {
                    //store element
                    string element = newEntry.firstValues[i];

                    //If element is empty, skip it.
                    if( element == string.Empty )
                        continue;
                    //If the element is the tag to remove, remove the original element.
                    else if( element == elementRemoveTag )
                        patchEntry.firstValues[i] = string.Empty;
                    //If the other two checks aren't true, just overwrite.
                    else
                        patchEntry.firstValues[i] = element;
                }

                //Add other entries
                for( int i = 0; i < newEntry.otherEntries.Length; i++ ) {
                    for( int j = 0; j < newEntry.otherEntries[i].Length; j++ ) {
                        //store element
                        string element = newEntry.otherEntries[i][j];

                        //If element is empty, skip it.
                        if( element == string.Empty )
                            continue;
                        //If the element is the tag to remove, remove the original element.
                        else if( element == elementRemoveTag )
                            patchEntry.otherEntries[i][j] = string.Empty;
                        //If the other two checks aren't true, just overwrite.
                        else
                            patchEntry.otherEntries[i][j] = element;
                    }
                }

                entries[entryIDWithoutPrefix] = patchEntry;
            }
        }

        public class MultiElementEntry {
            public string[] firstValues;
            public string[][] otherEntries;

            public string ID { get { return firstValues[0]; } }
        }
    }
}
