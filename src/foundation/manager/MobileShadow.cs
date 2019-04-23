
	using System;
#if UNITY_EDITOR
	using UnityEditor;
#endif
using UnityEngine;

namespace foundation
{
    [RequireComponent(typeof(Camera))]
    public class MobileShadow : MonoBehaviour
    {
        public enum Dimension
        {
            x256 = 256,
            x512 = 512,
            x1024 = 1024,
            x2048 = 2048,
        }

        public bool isUsePlanShadow = false;
        public Light SunLight;
        [Space] public Dimension TextureSize = Dimension.x1024;

        public LayerMask CastShadowLayers;
        [Space] public float ShadowDistance = 25.0f;
        [Range(0, 1)] public float NearPlaneOffset = 0.9f;

        [Space] [Range(0, 1)] public float ShadowOpacity = 0.5f;

        //public Material shadowEdgeFadeMaterial;
        public Shader mobileShadowReplacementShader;

        private float _focusRadius;

        private int _shadowMatrixID;
        private int _shadowTextureID;
        private int _shadowBlurID;
        private int _shadowOpacityID;
        private int _sunPositionID;

        private Camera _viewCamera;
        private GameObject _shadowCameraGO;
        private Transform _sunTransform;
        private Camera _shadowCamera;
        private RenderTexture _shadowTexture;
        private Matrix4x4 _shadowMatrix;

        private Vector3[] _frustrumCorners = new Vector3[8];

        [Space] public bool SoftShadow;
        public float BlurSize = 1.0f;

        [Space] public bool ShowDebug;

        [Space] public float FadeArea = 0.03f;
        public float FadeMin = 0.4f;

        public Func<Vector3> GetFadTagPosCB;

        private readonly Matrix4x4 _shadowSpaceMatrix = new Matrix4x4()
        {
            m00 = 0.5f,m01 = 0.0f,m02 = 0.0f,m03 = 0.5f,
            m10 = 0.0f,m11 = 0.5f,m12 = 0.0f,m13 = 0.5f,
            m20 = 0.0f,m21 = 0.0f,m22 = 0.5f,m23 = 0.5f,
            m30 = 0.0f,m31 = 0.0f,m32 = 0.0f,m33 = 1.0f,
        };

        protected virtual void Start()
        {
            _viewCamera = GetComponent<Camera>();
            _shadowMatrixID = Shader.PropertyToID("_MobileShadowMatrix");
            _shadowTextureID = Shader.PropertyToID("_MobileShadowTexture");
            _shadowBlurID = Shader.PropertyToID("_MobileShadowBlur");
            _shadowOpacityID = Shader.PropertyToID("_MobileShadowOpacity");
            _sunPositionID = Shader.PropertyToID("_MobileShadowSunPosition");
            _shadowMatrix = Matrix4x4.identity;

            Init();
        }

        protected virtual void Init()
        {
            if (SunLight == null || _viewCamera == null) return;

            if (CastShadowLayers == 0)
            {
                CastShadowLayers = 1 << LayerX.GetPlayerLayer();
            }
            ///在编辑器下可以直接看AssetBundle资源的hack
            if (Application.isMobilePlatform == false && Application.isPlaying)
            {
                mobileShadowReplacementShader = Shader.Find("Hidden/MobileShadowReplacementShader");
            }
            //end hack

            Shader.EnableKeyword("_MOBILESHADOW_ON");
            _sunTransform = SunLight.transform;
            Shader.SetGlobalFloat(_shadowOpacityID, ShadowOpacity);
            Shader.SetGlobalVector(_sunPositionID, _sunTransform.forward);

            if (_shadowCameraGO == null)
            {
                _shadowCameraGO = new GameObject("_Shadow Camera");

                if (ShowDebug)
                    _shadowCameraGO.hideFlags = HideFlags.DontSave;
                else
                {
                    _shadowCameraGO.hideFlags = HideFlags.HideAndDontSave;
                }

                _shadowCamera = _shadowCameraGO.AddComponent<Camera>();
                _shadowCamera.clearFlags = CameraClearFlags.Color;
                _shadowCamera.backgroundColor = Color.white;
                _shadowCamera.depthTextureMode = DepthTextureMode.Depth;
                _shadowCamera.useOcclusionCulling = false;
                _shadowCamera.orthographic = true;
                _shadowCamera.depth = 0;
                _shadowCamera.aspect = 1f;
                _shadowCamera.nearClipPlane = 0.0f;
                _shadowCamera.farClipPlane = 100.0f;
                _shadowCamera.allowHDR = false;
                _shadowCamera.allowMSAA = false;
                _shadowCamera.orthographicSize = 30;

                _shadowCamera.cullingMask = CastShadowLayers;

                if (mobileShadowReplacementShader == null)
                {
                    mobileShadowReplacementShader = Shader.Find("Hidden/MobileShadowReplacementShader");
                }
                _shadowCamera.SetReplacementShader(mobileShadowReplacementShader, "");
                _shadowCamera.enabled = false;

                AllocateNewTexture();
                _shadowCamera.targetTexture = _shadowTexture;
            }

           
        }

        protected virtual void UpdateFade()
        {
            if (GetFadTagPosCB == null)
            {
                Shader.DisableKeyword("_MOBILEFADE_ON");
                return;
            }

            Vector3 v = GetFadTagPosCB();
            Shader.EnableKeyword("_MOBILEFADE_ON");
            Shader.SetGlobalVector("_CameraToPlayerDir",  v- transform.position);
            Shader.SetGlobalVector("_PlayerWorldPos", v);
            Shader.SetGlobalFloat("_DitherFadeArea", FadeArea);
            Shader.SetGlobalFloat("_DitherFadeBaseCutoff", FadeMin);
        }

        protected RenderTexture tempRT = null;

        protected virtual void OnPreRender()
        {
            if (BaseAppSetting.GetInstance().quality == 0)
            {
                Shader.DisableKeyword("_MOBILESHADOW_ON");
                Shader.DisableKeyword("_MOBILEFADE_ON");
                return;
            }

            UpdateFade();

            if (isUsePlanShadow)
            {
                Shader.DisableKeyword("_MOBILESHADOW_ON");
                return;
            }

            Shader.EnableKeyword("_MOBILESHADOW_ON");
            if (_sunTransform == null)
            {
                Init();
                return;
            }

            UpdateFocus();
            if ((int) TextureSize != _shadowTexture.width)
            {
                AllocateNewTexture();
            }

            Shader.SetGlobalVector("_MobileShadowDir", _shadowCamera.transform.forward);
            _shadowCamera.Render();

        }

        protected virtual void OnEnable()
        {
            if (_shadowCameraGO == null || _shadowCamera == null)
            {
                Init();
            }
        }

        protected virtual void OnDisable()
        {
            Shader.DisableKeyword("_MOBILESHADOW_ON");
            Shader.SetGlobalTexture(_shadowTextureID, null);

            if (_shadowTexture != null)
            {
                //DestroyImmediate(_shadowTexture);
                RenderTexture.ReleaseTemporary(_shadowTexture);
                _shadowTexture = null;
            }
            if (_shadowCameraGO != null)
            {
                _shadowCamera.targetTexture = null;
                DestroyImmediate(_shadowCameraGO);
            }

            if (tempRT != null)
            {
                RenderTexture.ReleaseTemporary(tempRT);
                tempRT = null;
            }
        }

        public void recatchFocus()
        {
            if (_shadowTexture)
            {
                Shader.SetGlobalTexture(_shadowTextureID, _shadowTexture);
            }
        }

        protected virtual void AllocateNewTexture()
        {
            _shadowCamera.targetTexture = null;  
            if (_shadowTexture != null)
            {
                //DestroyImmediate(_shadowTexture);
                RenderTexture.ReleaseTemporary(_shadowTexture);
                _shadowTexture = null;
            }
            if (tempRT != null)
            {
                RenderTexture.ReleaseTemporary(tempRT);
                tempRT = null;
            }

            _shadowTexture = RenderTexture.GetTemporary((int) TextureSize, (int) TextureSize, 24);
            _shadowCamera.targetTexture = _shadowTexture;
            Shader.SetGlobalTexture(_shadowTextureID, _shadowTexture);
        }

        protected virtual void UpdateFocus()
        {
            Vector3 fC = FindFrustrumCenter();

            Vector3 eye = fC - _sunTransform.forward*_focusRadius * 2.0f;
            _shadowCamera.transform.position = eye;
            _shadowCamera.transform.LookAt(fC);

            var shadowViewMat = _shadowCamera.worldToCameraMatrix;
            _shadowCamera.orthographicSize = _focusRadius * 2.0f;
            _shadowCamera.nearClipPlane = 0;
            _shadowCamera.farClipPlane = _focusRadius * 6.0f;

            Matrix4x4 shadowProjection = Matrix4x4.Ortho(-_focusRadius, _focusRadius, -_focusRadius, _focusRadius, 0,
               _focusRadius * 6.0f);
            _shadowCamera.projectionMatrix = shadowProjection;

            _shadowMatrix = _shadowSpaceMatrix * _shadowCamera.projectionMatrix * shadowViewMat;
            Shader.SetGlobalMatrix(_shadowMatrixID, _shadowMatrix);
        }

        protected virtual Vector3 FindFrustrumCenter()
        {
            FindFrustrumCorners(ShadowDistance * NearPlaneOffset, ShadowDistance);

            for (int i = 0; i < _frustrumCorners.Length; i++)
            {
                _frustrumCorners[i] = _viewCamera.ViewportToWorldPoint(_frustrumCorners[i]);
            }

            Vector3 frustrumCenter = Vector3.zero;
            for (int i = 0; i < 8; i++)
            {
                frustrumCenter = frustrumCenter + _frustrumCorners[i];
            }
            frustrumCenter = frustrumCenter / 8.0f;

            _focusRadius = (_frustrumCorners[0] - _frustrumCorners[6]).magnitude / 2.0f;
            _focusRadius *= 1.5f;

            var texelsPerUnit = (float) TextureSize / (_focusRadius * 2.0f);
            var scalar = Matrix4x4.Scale(new Vector3(texelsPerUnit, texelsPerUnit, texelsPerUnit));

            var sunRotation = _sunTransform.rotation.eulerAngles;

            var sunMatrix = Matrix4x4.TRS(_sunTransform.position,
                Quaternion.Euler(_sunTransform.rotation.eulerAngles.x, _sunTransform.rotation.eulerAngles.y, 0),
                _sunTransform.localScale);

            frustrumCenter = sunMatrix.inverse.MultiplyPoint3x4(frustrumCenter);
            frustrumCenter = scalar.MultiplyPoint3x4(frustrumCenter);
            frustrumCenter.x = Mathf.Floor(frustrumCenter.x);
            frustrumCenter.y = Mathf.Floor(frustrumCenter.y);
            frustrumCenter = scalar.inverse.MultiplyPoint3x4(frustrumCenter);
            frustrumCenter = sunMatrix.MultiplyPoint3x4(frustrumCenter);
            return frustrumCenter;
        }

        protected virtual void FindFrustrumCorners(float nearDistance, float farDistance)
        {
            _frustrumCorners[0].x = 0;
            _frustrumCorners[0].y = 1;
            _frustrumCorners[0].z = nearDistance;

            _frustrumCorners[1].x = 1;
            _frustrumCorners[1].y = 1;
            _frustrumCorners[1].z = nearDistance;

            _frustrumCorners[2].x = 1;
            _frustrumCorners[2].y = 0;
            _frustrumCorners[2].z = nearDistance;

            _frustrumCorners[3].x = 0;
            _frustrumCorners[3].y = 0;
            _frustrumCorners[3].z = nearDistance;

            _frustrumCorners[4].x = 0;
            _frustrumCorners[4].y = 1;
            _frustrumCorners[4].z = farDistance;

            _frustrumCorners[5].x = 1;
            _frustrumCorners[5].y = 1;
            _frustrumCorners[5].z = farDistance;

            _frustrumCorners[6].x = 1;
            _frustrumCorners[6].y = 0;
            _frustrumCorners[6].z = farDistance;

            _frustrumCorners[7].x = 0;
            _frustrumCorners[7].y = 0;
            _frustrumCorners[7].z = farDistance;
        }

        private void OnDrawGizmos()
        {
            if (!ShowDebug)
            {
                return;
            }
            FindFrustrumCorners(ShadowDistance * NearPlaneOffset, ShadowDistance);
            for (int i = 0; i < _frustrumCorners.Length; i++)
            {
                _frustrumCorners[i] = GetComponent<Camera>().ViewportToWorldPoint(_frustrumCorners[i]);
            }

            Vector3 frustrumCenter = Vector3.zero;
            for (int i = 0; i < 8; i++)
            {
                frustrumCenter = frustrumCenter + _frustrumCorners[i];
            }
            frustrumCenter = frustrumCenter / 8.0f;

            _focusRadius = (_frustrumCorners[0] - _frustrumCorners[6]).magnitude / 2.0f;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(frustrumCenter, _focusRadius);
        }

    }
}
