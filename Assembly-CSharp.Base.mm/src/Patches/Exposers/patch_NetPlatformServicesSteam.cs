using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbraxisToolset;
using HBS.DebugConsole;
using UnityEngine;
using Necro;
using Steamworks;

namespace Patches {
    [MonoMod.MonoModPatch( "global::Necro.NetPlatformServicesSteam" )]
    class patch_NetPlatformServicesSteam: Necro.NetPlatformServicesSteam {

        public static patch_NetPlatformServicesSteam instance;
        public static bool isQueued = false;
        public static CSteamID queuedLobbyJoin;

        [MonoMod.MonoModIgnore]
        NetPlatformLobbyState lobbyState;

        [MonoMod.MonoModIgnore]
        private CSteamID playerID;
        public CSteamID GetPlayerID() {
            return playerID;
        }

        private extern void orig_Awake();
        private void Awake() {
            try {
                instance = this;

                try {
                    if( !SteamAPI.Init() ) {
                        base.enabled = false;
                        return;
                    }
                } catch( Exception ) {
                    base.enabled = false;
                    return;
                }

                NetPlatformServices.activeServices = this;
                StartCoroutine( this.WaitForReady() );
                StartCoroutine( ModSteamInit() );
            } catch( Exception ) {
            }
        }

        [MonoMod.MonoModIgnore]
        public IEnumerator WaitForReady() { yield return null; }
        public IEnumerator ModSteamInit() {

            while( !patch_SteamManager.Initialized ) {
                yield return new WaitForSeconds( 0.1f );
            }

            yield return new WaitForSeconds( 0.05f );

            try {
                NecroHooks.postNetworkSetup();
            } catch( System.Exception e ) {
                Debug.LogError( e );
            }

            try {
                if( isQueued ) {
                    DebugConsole.Log( "Join Game " + queuedLobbyJoin.m_SteamID );
                    JoinGame( queuedLobbyJoin );
                    isQueued = false;
                }
            } catch( System.Exception e ) {
                Debug.LogError( e );
            }

        }

        private extern void orig_Update();
        private void Update() {
            if( isQueued ) {
                DebugConsole.Log( "Discord : Join Game " + queuedLobbyJoin.m_SteamID );
                JoinGame( queuedLobbyJoin );
                isQueued = false;
            }
            orig_Update();
        }

        public static void QueueJoin(CSteamID lobbyID) {
            isQueued = true;
            queuedLobbyJoin = lobbyID;
        }

        public void JoinGame(CSteamID lobbyID) {
            this.lobbyState = NetPlatformLobbyState.JOINING;
            NetPlatformServicesSteam.log.Info( "Joining lobby: " + lobbyID );
            SteamMatchmaking.JoinLobby( lobbyID );
        }

    }
}
