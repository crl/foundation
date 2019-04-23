using UnityEngine;

namespace foundation
{
    [AddComponentMenu("Lingyu/ParticleSystemTimeScaleIgnore")]
    public class ParticleSystemTimeScaleIgnore: MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void Update()
        {
            _particleSystem.Simulate(Time.unscaledDeltaTime, true, false);
        }
    }
}