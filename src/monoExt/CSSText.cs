using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace clayui
{
    [AddComponentMenu("CSS/CSSText")]
    [RequireComponent(typeof(Text))]
    [DisallowMultipleComponent]
    public class CSSText : MonoBehaviour
    {
        [HideInInspector]
        public int classId = 0;

    }
}
