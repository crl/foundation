using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Lingyu/UnitCFG")]
    public class UnitCFG : ElementCFG
    {
        public float nameY = 2.0f;
        public UnitType unitType = UnitType.Other;

        public float baseY = 0.0f;

        public List<TextureSet> textureSets=new List<TextureSet>();
        /// <summary>
        /// 旧的不要了
        /// </summary>
        //public List<Texture> replaceTextures = new List<Texture>();

        public bool hasTexeture
        {
            get { return textureSets.Count > 0; }
        }

        /// <summary>
        /// 运行时数据
        /// </summary>
        private Dictionary<int, List<TextureSet>> cacheMap;

        /// <summary>
        /// 取得可替换贴图
        /// </summary>
        /// <param name="index">可替换的列表中的索引</param>
        /// <param name="keyIndex">第几个材质</param>
        /// <param name="defaluTexture">如果找不到时的默认贴图</param>
        /// <returns></returns>
        public Texture getTexture(int index,int keyIndex=0,Texture defaluTexture = null)
        {
            if (index < 0 || index >= textureSets.Count)
            {
                return defaluTexture;
            }
            List<TextureSet> list = getTextureSets(keyIndex);
            if (list==null)
            {
                return defaluTexture;
            }

            if (index >= list.Count)
            {
                return defaluTexture;
            }
            TextureSet t = list[index];
            if (t == null)
            {
                return defaluTexture;
            }

            return t.texture;
        }

        public List<TextureSet> getTextureSets(int key=0)
        {
            if (textureSets.Count==0)
            {
                return null;
            }

            List<TextureSet> list;
            if (cacheMap == null)
            {
                cacheMap = new Dictionary<int, List<TextureSet>>();
                foreach (TextureSet textureSet in textureSets)
                {
                    if (cacheMap.TryGetValue(textureSet.index, out list) == false)
                    {
                        list = new List<TextureSet>();
                        cacheMap.Add(textureSet.index, list);
                    }
                    list.Add(textureSet);
                }
            }

            cacheMap.TryGetValue(key, out list);
            return list;
        }
        protected override void OnDrawGizmos()
        {
            Vector3 f = this.transform.position;
            if (unitType == UnitType.Start)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(f, 0.5f);

            }else if (unitType == UnitType.Tel)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(f, 0.5f);
            }
            else
            {
                if (GetComponentInChildren<Renderer>() == null)
                {
                    Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
                }
            }
          
            Vector3 t = this.transform.position + new Vector3(0, nameY, 0);
            Gizmos.DrawLine(f, t);
            Gizmos.DrawSphere(t, 0.1f);
        }

        public void receiptAnimationEvent(AnimationEvent e)
        {
            if (Application.isEditor)
            {
                Debug.Log(e);
            }
        }

    }


    public enum UnitType
    {
        Avatar,
        Npc,
        Monster,
        Collect,
        Mount,
        Particle,
        Weapon,
        Map,
        Tel,
        Start,
        Other
    }
}