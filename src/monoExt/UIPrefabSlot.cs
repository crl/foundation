using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace foundation.monoExt
{
    /// <summary>
    /// ui的代理
    /// </summary>
    [AddComponentMenu("Lingyu/UIPrefabSlot")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class UIPrefabSlot : MonoBehaviour
    {
        public GameObject prefab;

        private bool isPlayingCreated = false;
        public GameObject instance { get; private set; }
#if UNITY_EDITOR
        void OnValidate()
        {
            CancelInvoke("CheckDirty");
            if (Application.isPlaying || PrefabUtility.GetPrefabType(this) == PrefabType.Prefab)
            {
                return;
            }
            Invoke("CheckDirty", 0.02f);
        }
#endif

        public void OnEnable()
        {
            
        }

        private bool isShaderFinded;
        private void CheckDirty()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (instance)
            {
                if (isPlayingCreated)
                {
                    return;
                }
                GameObject.DestroyImmediate(instance);
            }
            if (prefab)
            {
                if (isShaderFinded == false)
                {
                    isShaderFinded = true;
                    RenderUtils.ShaderFind(prefab);
                }
                instance = Instantiate(prefab) as GameObject;
                if (instance.activeSelf == false)
                {
                    instance.SetActive(true);
                }
                instance.name = prefab.name;
#if UNITY_EDITOR
                instance.hideFlags = HideFlags.HideAndDontSave;
#endif
                RectTransform r = instance.GetComponent<RectTransform>();
                r.SetParent(rectTransform, false);
                r.anchoredPosition = Vector2.zero;
            }
        }

        public void __create()
        {
            if (isPlayingCreated)
            {
                return;
            }
            isPlayingCreated = true;
            CheckDirty();

            if (instance != null)
            {
                UIPrefabSlot[] slots = instance.GetComponentsInChildren<UIPrefabSlot>(true);
                foreach (UIPrefabSlot slot in slots)
                {
                    slot.__create();
                }
            }

        }
    }

}