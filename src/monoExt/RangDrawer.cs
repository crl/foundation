using UnityEngine;

namespace foundation
{
    [AddComponentMenu("Lingyu/RangDrawer")]
    /// <summary>
    /// 区域绘制（如攻击）
    /// </summary>
    public class RangDrawer:MonoBehaviour
    {
        public float size = 0.1f;
        public int pointAmount = 100; //点的数目，值越大曲线越平滑  

        public Vector3 center = Vector3.zero;
        public float angle=60f;
        public float radius = 1;

        ///绘制类型
        public int type;
        protected void OnEnable()
        {
            drawSector();
        }
        public void drawSector()
        {
            LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = this.gameObject.AddComponent<LineRenderer>();
            }
            lineRenderer.startWidth = size;
            lineRenderer.endWidth=size;
            
            float eachAngle = angle/pointAmount;
            Vector3 forward = transform.forward;

            lineRenderer.positionCount=pointAmount;
            lineRenderer.SetPosition(0, center);
            lineRenderer.SetPosition(pointAmount - 1, center);

            for (int i = 1; i < pointAmount - 1; i++)
            {
                Vector3 pos = Quaternion.Euler(0f, -angle/2 + eachAngle*(i - 1), 0f)*forward*radius + center;
                lineRenderer.SetPosition(i, pos);
            }
        }
    }
}