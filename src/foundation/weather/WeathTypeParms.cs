using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    [System.Serializable]
    public class WeathTypeParms
    {
        protected WeatherManager weatherManager;

        [Header("EffectParticle")]
        public ParticleSystem particleSystem;
        public Vector3 offset=Vector3.zero;
        [Range(-1, 60f)] public float particleRuningTime = -1;
        [Range(0, 1)] public float Gloss = 0;
        public Color forgColor = Color.white;

        [Header("Active GameObject")]
        public List<GameObject> gameObjects = new List<GameObject>();

        /// <summary>
        /// 如果particleSystem是个prefab
        /// </summary>
        private ParticleSystem particleSystemInstance;

        private float startTime;
        private bool isTimeEnd = false;
        public void init(WeatherManager value)
        {
            this.weatherManager = value;
            if (particleSystem != null && particleSystem.gameObject.scene.name == null)
            {
                RenderUtils.ShaderFind(particleSystem.gameObject);
            }
        }

        public virtual void sleep()
        {
            if (particleSystem != null)
            {
                if (particleSystem.gameObject.scene.name == null)
                {
                    if (particleSystemInstance != null)
                    {
                        particleSystemInstance.SetActive(false);
                    }
                }
                else
                {
                    particleSystem.SetActive(false);
                }
            }

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(false);
            }
        }

        public virtual void awake()
        {
            startTime = Time.time;
            isTimeEnd = false;
            if (particleSystem != null)
            {
                if (particleSystem.gameObject.scene.name == null)
                {
                    if (particleSystemInstance != null)
                    {
                        particleSystemInstance.SetActive(true);
                    }
                    else
                    {
                        particleSystemInstance = GameObject.Instantiate(particleSystem);
                        particleSystemInstance.SetActive(true);
                    }
                }
                else
                {
                    particleSystem.SetActive(true);
                }
            }
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(true);
            }
        }

        public virtual void update()
        {
            if (isTimeEnd || weatherManager==null)
            {
                return;
            }
            if (particleRuningTime > 0)
            {
                if (Time.time - startTime > particleRuningTime)
                {
                    isTimeEnd = true;
                    if (particleSystemInstance != null)
                    {
                        particleSystemInstance.SetActive(false);
                    }
                    return;
                }
            }

            if (particleSystemInstance != null)
            {
                particleSystemInstance.transform.position = weatherManager.transform.position + offset;
            }
            //todo other;
        }

        public virtual void OnDestroy()
        {
            if (particleSystemInstance != null)
            {
                GameObject.DestroyImmediate(particleSystemInstance);
                particleSystemInstance = null;
            }
        }
    }
}