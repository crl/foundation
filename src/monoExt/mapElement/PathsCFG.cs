using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    /// <summary>
    /// 路点编辑器
    /// </summary>
    [AddComponentMenu("Lingyu/PathsCFG")]
    [Serializable]
    public class PathsCFG : MonoCFG
    {
        /// <summary>
        /// 路径点列表
        /// </summary>
        public List<RefVector2> list = new List<RefVector2>();

        /// <summary>
        /// 额外连接点
        /// </summary>
        public List<KeyVector2> connList = new List<KeyVector2>();

        public int length
        {
            get { return list.Count; }
        }

        public PathsCFG()
        {
        }

        public Vector2 center
        {
            get
            {
                return new Vector2(this.transform.position.x, this.transform.position.z);
            }
        }

        public RefVector2 getRefVector2ByGUID(string guid)
        {
            foreach (RefVector2 refVector2 in list)
            {
                if (refVector2.guid == guid)
                {
                    return refVector2;
                }
            }

            return null;
        }

        public int getRefVector2IndexByGUID(string guid)
        {
            int len = list.Count;
            for (int i = 0; i < length; i++)
            {
                if (list[i].guid == guid)
                {
                    return i;
                }
            }
            return -1;
        }

        public Vector2 getLogicPosition(int index)
        {
            if (index > list.Count - 1)
            {
                index = list.Count - 1;
            }
            return list[index].getVector2() + center;
        }

        public RefVector2 getNearRefVector2(Vector2 value)
        {
            int nearStartIndex = -1;
            float nearStart = float.MaxValue;
            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                Vector2 v = list[i].getVector2() + center;
                float d = Vector2.Distance(value, v);
                if (d < nearStart)
                {
                    nearStart = d;
                    nearStartIndex = i;
                }
            }

            if (nearStart > 200f)
            {
                return null;
            }


            if (nearStartIndex != -1)
            {
                return list[nearStartIndex];
            }

            return null;
        }

        public bool getNearIndex(Vector2 start, Vector2 end, out int nearStartIndex, out int nearEndIndex)
        {
            nearStartIndex = -1;
            nearEndIndex = -1;
            float nearStart = float.MaxValue;
            float nearEnd = float.MaxValue;
            int len = list.Count;
            for (int i = 0; i < len; i++)
            {
                Vector2 v = list[i].getVector2() + center;
                float d = Vector2.Distance(start, v);
                if (d < nearStart)
                {
                    nearStart = d;
                    nearStartIndex = i;
                }
                d = Vector2.Distance(end, v);
                if (d < nearEnd)
                {
                    nearEnd = d;
                    nearEndIndex = i;
                }
            }

            return nearStartIndex != -1 && nearEndIndex != -1;
        }

        public bool addConn(KeyVector2 v)
        {
            if (v.x == v.y)
            {
                return false;
            }

            foreach (KeyVector2 item in connList)
            {
                if (item.isEqual(v))
                {
                    return false;
                }
            }
            connList.Add(v);
            return true;
        }
    }
}


[Serializable]
public class RefVector2
{
    public string guid = "";
    public float x;
    public float y;

    public Vector2 getVector2()
    {
        return new Vector2(x, y);
    }

    public void reset(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public string getGUID()
    {
        return guid;
    }
}
