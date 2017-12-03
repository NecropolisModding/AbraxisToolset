using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Necro;
using UnityEngine.SceneManagement;

namespace AbraxisToolset {
    public class NecroMinimap : MonoBehaviour{

        public Camera minimapCamera;
        public Transform cameraTransform;
        public RenderTexture rt;
        private Light l;

        bool hasCopied = false;
        public static bool displayMinimap = false;

        public void Start() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnGUI() {
            if(!displayMinimap)
                return;
            GUILayout.Label( ThirdPersonCameraControl.HasInstance.ToString() + "|" + cameraTransform );
            if( ThirdPersonCameraControl.HasInstance && ThirdPersonCameraControl.Instance.Camera != null) {

                minimapCamera = ThirdPersonCameraControl.Instance.Camera;

                if( !hasCopied ) {

                    rt = new RenderTexture( 200, 200, 24 );
                    rt.anisoLevel = 2;
                    rt.Create();

                    hasCopied = true;

                    l = minimapCamera.gameObject.AddComponent<Light>();
                    l.type = LightType.Directional;
                    l.intensity = 2;
                    l.color = Color.white;
                    l.enabled = false;
                }
                cameraTransform = ThirdPersonCameraControl.Instance.Camera.transform;
                Vector3 previousPosition = cameraTransform.position;
                Quaternion previousRotation = cameraTransform.rotation;
                CameraClearFlags flag = minimapCamera.clearFlags;
                try {
                    cameraTransform.position = ThirdPersonCameraControl.Instance.CharacterFocusPoint + ( Vector3.up * 15 );
                    cameraTransform.rotation = Quaternion.LookRotation( Vector3.down, Vector3.forward );

                    MonoBehaviour[] mbs = cameraTransform.GetComponents<MonoBehaviour>();

                    foreach( MonoBehaviour mb in mbs ) {
                        mb.enabled = false;
                    }
                    ThirdPersonCameraControl.Instance.enabled = false;

                    minimapCamera.orthographic = true;
                    minimapCamera.orthographicSize = 20;
                    minimapCamera.targetTexture = rt;
                    l.enabled = true;

                    minimapCamera.clearFlags = CameraClearFlags.Color;
                    minimapCamera.backgroundColor = new Color( 1, 1, 1, 0 );

                    minimapCamera.Render();
                    foreach( MonoBehaviour mb in mbs )
                        mb.enabled = true;
                } catch {

                }

                l.enabled = false;
                minimapCamera.targetTexture = null;
                minimapCamera.orthographic = false;
                minimapCamera.clearFlags = flag;
                cameraTransform.position = previousPosition;
                cameraTransform.rotation = previousRotation;
                ThirdPersonCameraControl.Instance.enabled = true;

                string s = string.Empty;
                foreach( MonoBehaviour c in minimapCamera.gameObject.GetComponents<MonoBehaviour>() ) {
                    if( c.enabled )
                        s += c.GetType().Name + "|";
                }
                GUILayout.Label( s );

                GUI.DrawTexture( new Rect( 20, 100, 200, 200 ), rt );
            }
        }

        private void OnSceneLoaded(Scene newScene, LoadSceneMode mode) {
            //hasCopied = false;
        }

    }
}
