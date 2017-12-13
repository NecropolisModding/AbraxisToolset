using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AbraxisToolset.Multiplayer;
using HBS;
using Necro;
using HBS.Text;
using HBS.DebugConsole;

namespace AbraxisToolset.DiscordRPC {
    public class DiscordManager: MonoBehaviour {

        public static DiscordManager instance;

        public const string ID = "388842201104252929";
        private DiscordRpc.EventHandlers handlers;
        private DiscordRpc.RichPresence presence;

        private bool shouldUpdate = false;

        //Rich Presence stats
        private string currentParty = "";
        private ulong lobbyID = 0;
        private int partyMembers = 0;
        private int partyMax = 4;

        //Rich Presence description values
        private string presenceState = "Loading";
        private int currentLevel = int.MinValue;

        private string largePictureID;
        private string largePictureText;

        private Rect outRect = new Rect( 0, 0, 0, 0 );
        private Rect inRect = new Rect( 0, 0, 0, 0 );
        private float fadeTime = 35;
        private float fadeProgress = 0;

        private string inviteUserID;
        public string inviteUserName;
        private bool isInvite = false;

        float updateTimer = 0;
        private Dictionary<string, string> actorIDtoPictureID = new Dictionary<string, string>() {
            { "BlackguardFemale", "blackguard_female" },
            { "BlackguardMale", "blackguard_male" },
            { "BruteFemale", "brute_female" },
            { "BruteMale", "brute_male" }
        };
        private Dictionary<string, string> actorIDtoText = new Dictionary<string, string>() {
            { "BlackguardFemale", "Playing Blackguard Female" },
            { "BlackguardMale", "Playing Blackguard Male" },
            { "BruteFemale", "Playing Brute Female" },
            { "BruteMale", "Playing Brute Male" }
        };

        public void Awake() {
            instance = this;
        }

        public void Update() {
            updateTimer -= Time.deltaTime;

            if( isInvite ) {

                fadeProgress -= Time.deltaTime;
                if( fadeProgress < 5 )
                    fadeProgress -= Time.deltaTime * 14;
                if( fadeProgress <= 0 ) {
                    DiscordRpc.Respond( inviteUserID, DiscordRpc.Reply.Ignore );
                    isInvite = false;
                    fadeProgress = 0;
                    Debug.Log( "Ignored invite" );
                }

                if( fadeProgress > 0 ) {
                    if( Necro.Platform.GetKeyDown( KeyCode.LeftArrow ) ) {
                        DiscordRpc.Respond( inviteUserID, DiscordRpc.Reply.No );
                        isInvite = false;
                        fadeProgress = 0;
                        Debug.Log("Denied invite");
                    } else if( Necro.Platform.GetKeyDown( KeyCode.RightArrow ) ) {
                        DiscordRpc.Respond( inviteUserID, DiscordRpc.Reply.Yes );
                        isInvite = false;
                        fadeProgress = 0;
                        Debug.Log( "Accepted invite" );
                    }
                }
            }


            if( updateTimer <= 0 ) {
                //Character updater
                {
                    if( CharacterSelectorCamera.HasInstance ) {
                        if( Patches.patch_CharacterSelectorCamera.Instance.GetLocalActor() != null ) {
                            SetLargePictureID( actorIDtoPictureID[Patches.patch_CharacterSelectorCamera.Instance.GetLocalActor().actorDefId] );
                            SetLargePictureText( actorIDtoText[Patches.patch_CharacterSelectorCamera.Instance.GetLocalActor().actorDefId] );
                        }
                    }

                    if( ThirdPersonCameraControl.HasInstance ) {
                        if( ThirdPersonCameraControl.Instance.CharacterActor != null ) {
                            SetLargePictureID( actorIDtoPictureID[ThirdPersonCameraControl.Instance.CharacterActor.actorDefId] );
                            SetLargePictureText( actorIDtoText[ThirdPersonCameraControl.Instance.CharacterActor.actorDefId] );
                        }
                    }
                }

                //Floor updater
                if( NecropoloidSingleton.HasInstance && ThirdPersonCameraControl.HasInstance ) {
                    if( ThirdPersonCameraControl.Instance.CharacterActor != null ) {
                        Necropoloid.Cell currentCell = NecropoloidSingleton.Instance.Necropoloid.GetCellFromPosition( ThirdPersonCameraControl.Instance.CharacterFocusPoint );
                        if( currentCell != null ) {
                            SetRichPresenceState( "In A Game" );
                            SetLevel( ( -currentCell.gridPos.y ) );
                        }
                    }
                } else if( Necro.UI.Menu_MainMenu.HasInstance ) {
                    SetRichPresenceState( "On Main Menu" );
                } else {
                    SetRichPresenceState( "Loading" );
                }
                updateTimer = 5;
            }

            if( shouldUpdate ) {
                DebugConsole.Log("Updating Discord Presence");
                shouldUpdate = false;
                UpdateRichPresenceValues();
                DiscordRpc.UpdatePresence( ref presence );
            }

            DiscordRpc.RunCallbacks();
        }

        public void OnEnable() {

            try {
                handlers = new DiscordRpc.EventHandlers();
                handlers.readyCallback = ReadyCallback;
                handlers.disconnectedCallback += DisconnectedCallback;
                handlers.errorCallback += ErrorCallback;
                handlers.joinCallback += JoinCallback;
                handlers.spectateCallback += SpectateCallback;
                handlers.requestCallback += RequestCallback;

                Debug.Log( "Initializing Discord" );

                presence = new DiscordRpc.RichPresence();

                DiscordRpc.Initialize( ID, ref handlers, true, "384490" );
            } catch( System.Exception e ) {
                Debug.LogError( e );
            }
        }

        public void OnGUI() {
            outRect = new Rect( Screen.width / 2 - 250, -75, 500, 55 );
            inRect = new Rect( Screen.width / 2 - 250, 25, 500, 55 );
            Rect currentRect = new Rect( Vector2.Lerp( outRect.position, inRect.position, fadeProgress / fadeTime ), Vector2.Lerp( outRect.size, inRect.size, fadeProgress / fadeTime ) );

            GUI.Box( currentRect, "" );
            GUILayout.BeginArea( currentRect );
            GUILayout.Label( string.Format( "<size=20>User {0} is asking to join your game</size>", inviteUserName ) );
            GUILayout.Label( "Press [Right Arrow] to accept, or [Left Arrow] to deny" );
            GUILayout.EndArea();
        }

        public void OnDestroy() {
            Debug.Log( "Discord Shutdown" );
            DiscordRpc.Shutdown();
        }

        public void UpdateRichPresenceValues() {
            presence.partyId = currentParty;
            presence.partyMax = partyMax;
            presence.partySize = partyMembers;

            presence.joinSecret = currentParty + "," + lobbyID.ToString();
            presence.largeImageKey = largePictureID;
            presence.largeImageText = largePictureText;

            presence.state = presenceState;
            if( currentLevel != int.MinValue ) {
                presence.details = string.Format( "On level {0}", currentLevel + 1 );
            }
        }
        public void UpdateRP() {
            shouldUpdate = true;
        }

        public void ReadyCallback() {
            Debug.Log( "Discord is Ready" );
        }
        public void DisconnectedCallback(int errorCode, string message) {
            Debug.Log( string.Format( "Discord: disconnect {0}: {1}", errorCode, message ) );
        }
        public void ErrorCallback(int errorCode, string message) {
            Debug.Log( string.Format( "Discord: error {0}: {1}", errorCode, message ) );
        }
        public void JoinCallback(string secret) {
            if( secret == string.Empty )
                return;

            List<string> parts = new List<string>();
            StringUtil.SplitCSV( secret, parts, true );

            Debug.Log( string.Format( "Discord: join ({0})", secret ) );

            Patches.patch_NetPlatformServicesSteam.QueueJoin( new Steamworks.CSteamID( ulong.Parse( parts[1] ) ) );
            SetPartyID( parts[0] );
        }
        public void SpectateCallback(string secret) {
            Debug.Log( string.Format( "Discord: spectate ({0})", secret ) );
        }
        public void RequestCallback(ref DiscordRpc.JoinRequest request) {
            Debug.Log( string.Format( "Discord: join request {0}#{1}: {2}", request.username, request.discriminator, request.userId ) );
            inviteUserName = request.username;
            inviteUserID = request.userId;
            fadeProgress = fadeTime;
            isInvite = true;
        }

        public void SetRichPresenceState(string state) {
            if( state != presenceState ) {
                presenceState = state;
                UpdateRP();
            }
        }
        public void SetPartyNumber(int numberOfMembers) {
            if( partyMembers != numberOfMembers ) {
                partyMembers = numberOfMembers;
                UpdateRP();
            }
        }
        public void SetPartyMaxNumber(int maxMembers) {
            if( partyMax != maxMembers ) {
                partyMax = maxMembers;
                UpdateRP();
            }
        }
        public void SetPartyID(string partyID) {
            if( currentParty != partyID ) {
                currentParty = partyID;
                UpdateRP();
            }
        }
        public void SetLobbyID(ulong lobbyID) {
            if( this.lobbyID != lobbyID ) {
                this.lobbyID = lobbyID;
                UpdateRP();
            }
        }
        public void SetLevel(int level) {
            if( level != currentLevel ) {
                currentLevel = level;
                UpdateRP();
            }
        }
        public void SetLargePictureID(string id) {
            if( id != largePictureID ) {
                largePictureID = id;
                UpdateRP();
            }
        }
        public void SetLargePictureText(string txt) {
            if( largePictureText != txt ) {
                largePictureText = txt;
                UpdateRP();
            }
        }

    }
}
