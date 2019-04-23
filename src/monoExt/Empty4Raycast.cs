using UnityEngine.UI;

namespace foundation
{
    /// <summary>
    /// 关于填充率
    /// 由于UI部分用了透明效果 会产生overdraw。
    /// </summary>
    public class Empty4Raycast : MaskableGraphic
    {
        protected Empty4Raycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}