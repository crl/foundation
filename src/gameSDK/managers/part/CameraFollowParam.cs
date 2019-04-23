using UnityEngine;

namespace gameSDK
{
    [System.Serializable]
    public class CameraFollowParam
    {
        /// <summary>
        /// 距离
        /// </summary>
        [Range(0, 20)]
        public float distance = 5f;
        [Range(0, 20)]
        public float height = 2f;

        [Range(0, 20)]
        public float heightLookAt;
        public Vector3 rotationAngle = Vector3.zero;

        public Vector3 offset = new Vector3(0f, 0f, 0f);

        public Vector3 offsetLookAt = new Vector3(0f, 0f, 0f);
        public Vector3 cameraOffset = new Vector3(0f, 0f, 0f);
    }
}