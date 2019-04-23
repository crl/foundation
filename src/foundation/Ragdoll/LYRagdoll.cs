using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using foundation;
using UnityEngine;

/// <summary>
/// 布娃娃系统
/// 2017.12.15
/// 
/// 使用unity自带的CharacterJoint控件
/// 主要实现动态绑定到prefab上后
///    1、禁用已有动画系统　Animator Animation
///    2、通过配置文件读取配置信息并且在对应节点上添加　CharacterJoint BoxCollider CapsuleCollider RigidBody
///    3、恢复原有动画系统功能
/// </summary>

namespace foundation
{
    public class LYRagdoll : MonoBehaviour
    {
        public class BoneItemD
        {
            public BoneItemD parent;
            public List<BoneItemD> childLst = new List<BoneItemD>();

            public string boneName;
            public string colliderType; //节点类型 b = box , c = capsule

            public float minLimit;
            public float maxLimit;
            public float swingLimit;
            public Vector3 axis;
            public Vector3 normalAxis;
            public float radiusScale;
            public float density = 1.5f;
            public float summedMass = 0.4f;
            public float minSize;
            public bool bEncapsulateInParent = true;//是否包含在父碰撞盒内

            public Transform anchor;
            public CharacterJoint joint;
            public Rigidbody rigidbody;
            public Collider collider;

            public LYRagdollManager.templateItemD templateItem;

            public string BoneName
            {
                get
                {
                    if (templateItem != null)
                    {
                        string tempStr = "";
                        if (templateItem.nameMap.TryGetValue(boneName, out tempStr) == true)
                            return tempStr;
                    }
                    return boneName;
                }
                set { boneName = value; }
            }

            public BoneItemD()
            {

            }

            public bool enable
            {
                set
                {
                    if (value == false)
                    {
                        if (joint != null)
                            UnityEngine.Object.DestroyImmediate(joint);
                        if (rigidbody != null)
                            UnityEngine.Object.DestroyImmediate(rigidbody);
                        if (collider != null)
                            UnityEngine.Object.DestroyImmediate(collider);
                        joint = null;
                        rigidbody = null;
                        collider = null;
                    }
                    else
                    {
                        BuildCollider();
                        BuildRigidbody();
                        BuildJoint();
                    }

                    for (int i = 0; i < childLst.Count; i++)
                        childLst[i].enable = value;
                }
            }
            protected void BuildCollider()
            {
                if (collider != null)
                    return;
                if (anchor == null)
                    return;

                if (colliderType == "b" || parent == null)
                {//是碰撞盒
                    Bounds bound = GetBounds();
                    //                for(int i = 0 ; i < childLst.Count ; i ++)
                    //                    bound = this.Clip(bound, anchor, childLst[i].anchor, false);
                    //                if (parent != null)
                    //                    bound = this.Clip(bound, anchor, parent.anchor, false); 
                    BoxCollider boxCollider = anchor.gameObject.GetComponent<BoxCollider>();
                    if(boxCollider == null)
                        boxCollider = anchor.gameObject.AddComponent<BoxCollider>();
                    boxCollider.center = bound.center;
                    boxCollider.size = bound.size ;
                    collider = boxCollider;
                }
                else if (colliderType == "c" && parent != null)
                {//是碰撞胶囊
                    Vector3 stPos = Vector3.zero;
                    Vector3 endPos = Vector3.zero;
                    if (childLst.Count <= 0)
                    {
                        for (int i = 0; i < anchor.childCount; i++)
                            endPos += anchor.GetChild(i).position;
                        endPos /= anchor.childCount;
                    }
                    else
                    {
                        for (int i = 0; i < childLst.Count; i++)
                            endPos += childLst[i].anchor.position;
                        endPos /= childLst.Count;
                    }
                    endPos = anchor.InverseTransformPoint(endPos);

                    float dis = Vector3.Distance(stPos, endPos);

                    CapsuleCollider capsuleCollider = anchor.gameObject.GetComponent<CapsuleCollider>();
                    if(capsuleCollider == null)
                        capsuleCollider = anchor.gameObject.AddComponent<CapsuleCollider>();
                    capsuleCollider.direction = 0;
                    capsuleCollider.center = (stPos + endPos) * 0.5f;
                    capsuleCollider.height = Mathf.Abs(dis);
                    capsuleCollider.radius = Mathf.Abs(this.minSize);
                    collider = capsuleCollider;

                }
                else if (colliderType == "s")
                {//是碰撞球
//                    int num;
//                    float single;
                    SphereCollider sphereCollider = anchor.gameObject.GetComponent<SphereCollider>();
                    if(sphereCollider == null)
                        sphereCollider = anchor.gameObject.AddComponent<SphereCollider>();
                    sphereCollider.radius = this.minSize;
                    sphereCollider.center = Vector3.zero;
                }
            }

            protected void BuildJoint()
            {
                if (parent == null)
                    return;
                if (parent.anchor == null)
                    return;
                if (joint != null)
                    return;
                if (anchor == null)
                    return;

                joint = anchor.gameObject.GetComponent<CharacterJoint>();
                if(joint == null)
                    joint = anchor.gameObject.AddComponent<CharacterJoint>();
                joint.enableCollision = true;
                joint.enablePreprocessing = true;
                joint.axis = this.axis;//CalculateDirectionAxis(anchor.InverseTransformDirection(axis));
                joint.swingAxis = this.normalAxis;//CalculateDirectionAxis(anchor.InverseTransformDirection(normalAxis));
                joint.anchor = Vector3.zero;
                joint.connectedBody = parent.anchor.GetComponent<Rigidbody>();
                //joint.enablePreprocessing = false;
                SoftJointLimit softJointLimit = new SoftJointLimit()
                {
                    contactDistance = 10,
                    limit = minLimit,
                    bounciness = 0,
                };
                joint.lowTwistLimit = softJointLimit;

                softJointLimit.limit = maxLimit;
                joint.highTwistLimit = softJointLimit;

                softJointLimit.limit = swingLimit;
                joint.swing1Limit = softJointLimit;

                softJointLimit.limit = 0f;
                joint.swing2Limit = softJointLimit;

                SoftJointLimitSpring sjls = new SoftJointLimitSpring()
                {
                    damper = 1,
                    spring = 0,
                };
                joint.swingLimitSpring = sjls;
                joint.twistLimitSpring = sjls;

                joint.breakForce = float.PositiveInfinity;
                joint.breakTorque = float.PositiveInfinity;
            }

            protected void BuildRigidbody()
            {
                if (rigidbody != null)
                    return;
                if (anchor == null)
                    return;

                rigidbody = anchor.gameObject.GetComponent<Rigidbody>();
                if (rigidbody == null)
                    rigidbody = anchor.gameObject.AddComponent<Rigidbody>();
                rigidbody.mass = density;
            }
            private Bounds GetBounds()
            {
                Bounds item = new Bounds();
                for (int i = 0; i < childLst.Count; i++)
                    if (childLst[i].bEncapsulateInParent == true && childLst[i].anchor != null)
                        item.Encapsulate(anchor.InverseTransformPoint(childLst[i].anchor.position));
                if (parent != null && parent.anchor != null)
                    item.Encapsulate(anchor.InverseTransformPoint(parent.anchor.position));

                Vector3 vector3 = item.size;
                vector3[SmallestComponent(item.size)] = vector3[LargestComponent(item.size)] / 2f;
                item.size = vector3;
                return item;
            }

            private Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
            {
                int item = LargestComponent(bounds.size);
                if (Vector3.Dot(Vector3.up, relativeTo.TransformPoint(bounds.max)) > Vector3.Dot(Vector3.up, relativeTo.TransformPoint(bounds.min)) != below)
                {
                    Vector3 vector3 = bounds.max;
                    Vector3 vector31 = relativeTo.InverseTransformPoint(clipTransform.position);
                    vector3[item] = vector31[item];
                    bounds.max = vector3;
                }
                else
                {
                    Vector3 vector32 = bounds.min;
                    Vector3 vector33 = relativeTo.InverseTransformPoint(clipTransform.position);
                    vector32[item] = vector33[item];
                    bounds.min = vector32;
                }
                return bounds;
            }
            private static int LargestComponent(Vector3 point)
            {
                int num = 0;
                if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
                {
                    num = 1;
                }
                if (Mathf.Abs(point[2]) > Mathf.Abs(point[num]))
                {
                    num = 2;
                }
                return num;
            }
            private static int SmallestComponent(Vector3 point)
            {
                int num = 0;
                if (Mathf.Abs(point[1]) < Mathf.Abs(point[0]))
                {
                    num = 1;
                }
                if (Mathf.Abs(point[2]) < Mathf.Abs(point[num]))
                {
                    num = 2;
                }
                return num;
            }

            public void SaveXml(XmlDocument doc_ , XmlElement parent_)
            {
                XmlElement item = doc_.CreateElement("item", "");
                item.SetAttribute("orgName", this.boneName);
                item.SetAttribute("newName", this.anchor != null?this.anchor.name:this.boneName);
                parent_.AppendChild(item) ;

                for (int i = 0; i < childLst.Count; i ++)
                {
                    childLst[i].SaveXml(doc_ , parent_);
                }
            }
        }

        protected BoneItemD rootItem = null;
        protected List<Animator> animLst = new List<Animator>();
        protected LYRagdollManager.ItemD ragDollItem = null;
        protected LYRagdollManager.templateItemD templateItem = null;

        public BoneItemD RootItem { get { return rootItem; } }

        public LYRagdollManager.ItemD RagdollItem { get { return ragDollItem; } }

        // Use this for initialization
        void Start()
        {
            Animator[] lst = gameObject.GetComponentsInChildren<Animator>();
            for (int i = 0; i < lst.Length; i++)
            {
                if (lst[i].enabled == true)
                {
                    animLst.Add(lst[i]);
                }
            }
        }

        protected bool EnableAnim
        {
            set
            {
                if (animLst.Count <= 0)
                {
                    Animator[] lst = gameObject.GetComponentsInChildren<Animator>();
                    for (int i = 0; i < lst.Length; i++)
                    {
                        animLst.Add(lst[i]);
                    }
                }
                for (int i = 0; i < animLst.Count; i++)
                {
                    animLst[i].enabled = value;
                }
            }
        }

        public void OnEnable()
        {
            if (rootItem != null)
                rootItem.enable = true;

            EnableAnim = false;
        }

        public void OnDisable()
        {
            if (rootItem != null)
                rootItem.enable = false;

            EnableAnim = true;
        }

        public void Clear()
        {
            OnDisable();
            UnityEngine.Object.DestroyImmediate(this);
        }
        
        public void LoadFromNode(LYRagdollManager.ItemD item_ , LYRagdollManager.templateItemD template_)
        {
            if (item_ == null)
                return;
            if (item_.root == null)
                return;
            ragDollItem = item_;
            templateItem = template_;
            rootItem = ReadXmlNode(item_.root, null , templateItem);
        }

        protected List<float> GetFloatLst(string str_)
        {
            List<float> retF = new List<float>();
            string[] strLst = str_.Split(',');
            for (int i = 0; i < strLst.Length; i++)
                retF.Add(float.Parse(strLst[i]));
            return retF;
        }

        protected Vector3 GetVector3(string str_)
        {
            List<float> retF = GetFloatLst(str_);
            return new Vector3(retF[0], retF[1], retF[2]);
        }

        protected Vector3 GetDirByType(string str_)
        {
            switch (str_)
            {
                case "right":
                    return Vector3.right;
                case "up":
                    return Vector3.up;
                case "forward":
                    return Vector3.forward;
                case "left":
                    return Vector3.left;
                case "down":
                    return Vector3.down;
                case "back":
                    return Vector3.back;
            }
            return Vector3.up;
        }

        protected BoneItemD ReadXmlNode(XmlNode node_, BoneItemD parent_ , LYRagdollManager.templateItemD templateItem_)
        {
            if (node_ == null)
                return null;

            BoneItemD item = new BoneItemD();
            item.parent = parent_;
            item.templateItem = templateItem_;

            List<float> tempFLst = new List<float>();


            for (int i = 0; i < node_.Attributes.Count; i++)
            {
                XmlAttribute a = node_.Attributes[i];
                if (a.Name == "name")
                {
                    item.BoneName = a.Value;
                    break;
                }
            }

            item.anchor = gameObject.GetChildTransform(item.BoneName);
            //if (item.anchor == null)
            //    return null;

            for (int i = 0; i < node_.Attributes.Count; i++)
            {
                XmlAttribute a = node_.Attributes[i];
                switch (a.Name)
                {
                    case "type":
                        item.colliderType = a.Value;
                        break;
                    case "limit":
                        tempFLst = GetFloatLst(a.Value);
                        item.minLimit = tempFLst[0];
                        item.maxLimit = tempFLst[1];
                        break;
                    case "swing":
                        item.swingLimit = GetFloatLst(a.Value)[0];
                        break;
                    case "axis":
                        item.axis = GetDirByType(a.Value);
                        break;
                    case "normalAxis":
                        item.normalAxis = GetDirByType(a.Value);
                        break;
                    case "radiusScale":
                        item.radiusScale = GetFloatLst(a.Value)[0];
                        break;
                    case "density":
                        item.density = GetFloatLst(a.Value)[0];
                        break;
                    case "summedMass":
                        item.summedMass = GetFloatLst(a.Value)[0];
                        break;
                    case "minSize":
                        item.minSize = GetFloatLst(a.Value)[0];
                        break;
                    case "Encapsulate":
                        if (a.Value == "false")
                            item.bEncapsulateInParent = false;
                        break;
                }
            }

            for (int i = 0; i < node_.ChildNodes.Count; i++)
            {
                BoneItemD child = ReadXmlNode(node_.ChildNodes[i], item , templateItem_);
                if (child != null)
                    item.childLst.Add(child);
            }

            return item;
        }

        public void SaveXml(string pathName_ , string prefabName_)
        {
            if (RootItem == null)
                return;

            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);
            XmlElement root = doc.CreateElement("root");
            root.SetAttribute("templateName", this.ragDollItem.name);
            root.SetAttribute("prefabName" , prefabName_);
            doc.AppendChild(root);

            RootItem.SaveXml(doc , root);
            doc.Save(pathName_);
        }
        
    }
}