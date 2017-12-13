using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using HBS.DebugConsole;
using UnityEngine;
using System.Collections;
using AbraxisToolset.DiscordRPC;

namespace AbraxisToolset.Multiplayer {
    public class ATMultiplayerManager: MonoBehaviour {

        public static Callback<LobbyCreated_t> m_LobbyCreated;
        public static Callback<LobbyEnter_t> m_LobbyEnter;
        public static Callback<LobbyKicked_t> m_LobbyKicked;

        public static List<CSteamID> lobbyIDs = new List<CSteamID>();
        public static List<CSteamID> lobbyOwners = new List<CSteamID>();

        private static float timer = 1f;

        public static bool inLobby = false;
        public static CSteamID lobbyID = new CSteamID( 0 );
        public string lobbyName;

        public void Init(object[] args) {
            try {
                m_LobbyCreated = Callback<LobbyCreated_t>.Create( LobbyCreated );
                m_LobbyEnter = Callback<LobbyEnter_t>.Create( LobbyEntered );
                m_LobbyKicked = Callback<LobbyKicked_t>.Create( LobbyExit );
            } catch( System.Exception e ) {
                Debug.LogError( e );
            }
        }

        public void Update() {
            if( !Patches.patch_SteamManager.Initialized )
                return;

            timer -= Time.deltaTime;
            if( timer <= 0 ) {
                timer = 5f;

                int newMembers = SteamMatchmaking.GetNumLobbyMembers( lobbyID );
                DiscordManager.instance.SetPartyNumber( newMembers );
            }
        }

        public void LobbyCreated(LobbyCreated_t pCallback) {
            lobbyName = new System.Random().Next().ToString();
        }

        public void LobbyEntered(LobbyEnter_t pCallback) {
            inLobby = true;
            lobbyID = new CSteamID( pCallback.m_ulSteamIDLobby );

            DiscordManager.instance.SetPartyID( lobbyName );
            DiscordManager.instance.SetLobbyID( pCallback.m_ulSteamIDLobby );
            DiscordManager.instance.SetPartyNumber( SteamMatchmaking.GetNumLobbyMembers( lobbyID ) );
            DiscordManager.instance.SetPartyMaxNumber( 4 );
        }

        public void LobbyExit(LobbyKicked_t pCallback) {
            inLobby = false;
            lobbyID = new CSteamID( 0 );
        }
    }
}
