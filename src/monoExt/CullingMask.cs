using UnityEngine;
using UnityEngine.UI;

namespace foundation
{
    [RequireComponent(typeof(RawImage))]
    [ExecuteInEditMode]
    public class CullingMask : MonoBehaviour
    {
        private RawImage cullingImg;
        private RawImage parentImg;
        private Matrix4x4 matrix;
        private Vector3 scaleV3;
        private Vector2 maskScaleV2;
        private Matrix4x4 matrix2;
        private bool _isStart = false;

        // Use this for initialization
        void Start()
        {
            _isStart = true;
            matrix = new Matrix4x4();
            matrix2 = new Matrix4x4();
 
        }

        private static Material sharedMaterial;
        protected virtual void OnEnable()
        {
            parentImg = transform.parent.GetComponent<RawImage>();
            if (sharedMaterial == null)
            {
                sharedMaterial= new Material(Shader.Find("Lingyu/UIMaskShader"));
            }
            parentImg.material = sharedMaterial;

            parentImg.material.SetFloat("_Cutoff", 1.0f);

            cullingImg = UIUtils.GetRawImage(gameObject, "");
            Color color=cullingImg.color;
            color.a = 0f;

            cullingImg.color = color;

            setOffset();
        }

#if UNITY_EDITOR
            void Update()
                {
                    setOffset();
                }
#endif


        /// <summary>
        /// 只在编辑器状态中属性值变化时触发
        /// </summary>
        protected void OnValidate()
        {
            setOffset();
        }

        public void setOffset()
        {
            if (!_isStart || cullingImg == null)
            {
                Start();
                return;
            }
            parentImg.material.SetTexture("_MaskTex", cullingImg.mainTexture);

            float bgOriginW = parentImg.rectTransform.sizeDelta.x*parentImg.rectTransform.localScale.x;
            float bgOriginH = parentImg.rectTransform.sizeDelta.y*parentImg.rectTransform.localScale.y;

            float maskOriginW = cullingImg.rectTransform.sizeDelta.x*cullingImg.rectTransform.localScale.x;
            float maskOriginH = cullingImg.rectTransform.sizeDelta.y*cullingImg.rectTransform.localScale.y;
             
            scaleV3 = new Vector3(1/maskOriginW, 1/maskOriginH, 1);
            Vector3 offsetV3 = new Vector3(-0.5f, -0.5f, 0f);
            offsetV3.Scale(scaleV3);
            Vector3 sv3 = new Vector3(-1f, -1f, 1f);
            sv3.Scale(scaleV3);

            matrix.SetTRS(
                offsetV3,
                new Quaternion(0f, 0f, 0f, 1f),
                sv3
                );

            matrix2.SetTRS(
                new Vector3(0.5f - bgOriginW/maskOriginW/2, 0.5f - bgOriginH/maskOriginH/2, 0f),
                new Quaternion(0f, 0f, 0f, 1f),
                new Vector3(1f, 1f, 1f)
                );

            float scalX = bgOriginW/maskOriginW;
            float scaleY = bgOriginH/maskOriginH;

            maskScaleV2 = new Vector2(scalX, scaleY);

            Vector3 tmp = cullingImg.transform.localPosition;
            //切换坐标系
            tmp = matrix.MultiplyPoint3x4(tmp);
            //原点重合
            tmp = matrix2.MultiplyPoint3x4(tmp);

            //Debug.Log("t:"+tmp);

            parentImg.material.SetTextureScale("_MaskTex", maskScaleV2);
            parentImg.material.SetTextureOffset("_MaskTex", tmp);
        }



    }
}
