using UnityEngine;

namespace foundation
{
    [AddComponentMenu("Lingyu/FocusGameObject")]
    public class FocusGameObject : MonoBehaviour
    {
        public GameObject effect;

        public void setFocus(bool v)
        {
            if (effect == null) return;

            if (effect.transform.parent == null)
            {
                RenderUtils.ShaderFind(effect);
                effect = GameObject.Instantiate(effect);
                effect.transform.SetParent(transform);
                effect.transform.localPosition = Vector3.zero;
                effect.transform.localScale = Vector3.one;
                effect.transform.localEulerAngles = Vector3.zero;
            }

            if (v)
            {
                if (effect.activeSelf == false)
                    effect.SetActive(true);
            }
            else
            {
                if (effect.activeSelf == true)
                    effect.SetActive(false);
            }
        }
    }
}
