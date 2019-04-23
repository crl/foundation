using foundation;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System;

namespace foundation
{
    public class LYRagdollManager
    {
        protected static LYRagdollManager sInstance = new LYRagdollManager();
        public static LYRagdollManager Instance
        {
            get { return sInstance; }
        }

        public class ItemD
        {
            public int idx = 0;
            public string name;
            public XmlNode root = null;

            public string GetXmlNodeName(XmlNode node_)
            {
                foreach (XmlAttribute att in node_.Attributes)
                {
                    if (att.Name == "name")
                        return att.Value;
                }
                return "";
            }

            protected bool BNodeInTransform(Transform t_, XmlNode node_)
            {
                Transform retT = t_.gameObject.GetChildTransform(GetXmlNodeName(node_));
                if (retT == null)
                {
                    Debug.Log("找不到对应的骨骼：" + GetXmlNodeName(node_));
                    return false;
                }

                foreach (XmlNode child in node_.ChildNodes)
                {
                    if (BNodeInTransform(retT, child) == false)
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool BFit(GameObject obj_)
            {
                return BNodeInTransform(obj_.transform, root);
            }
        }

        protected List<ItemD> nodeLst = new List<ItemD>();
        protected Dictionary<string, int> keyMap = new Dictionary<string, int>();

        public class templateItemD
        {
            public string templateName = "";
            public string prefabName = "";
            public Dictionary<string, string> nameMap = new Dictionary<string, string>();
        }
        protected Dictionary<string , templateItemD> templateMap = new Dictionary<string, templateItemD>();

        public LYRagdollManager()
        {
        }

        public List<ItemD> NodeLst { get { return nodeLst; } }
        
        public List<string> NameLst
        {
            get
            {
                List<string> retLst = new List<string>();
                for (int i = 0; i < NodeLst.Count; i ++)
                    retLst.Add(NodeLst[i].name);
                return retLst;
            }
        }

        public int GetIndexByName(string name_)
        {
            for (int i = 0; i < NodeLst.Count; i ++)
            {
                if (name_ == NodeLst[i].name)
                    return i;
            }
            return -1;
        }

        protected void LoadConfigFile(string pathName_)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(pathName_);
                foreach (XmlNode node in xmlDocument.ChildNodes)
                {
                    if (node.Name == "root")
                    {
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            XmlNode item = node.ChildNodes[i];
                            ItemD d = new ItemD();
                            d.idx = i;
                            d.name = item.Name.ToLower();
                            d.root = item.FirstChild;
                            nodeLst.Add(d);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        protected void LoadTemplateFile(string pathName_)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(pathName_);
                templateItemD templateItem = new templateItemD();

                foreach (XmlNode node in xmlDocument.ChildNodes)
                {
                    if (node.Name == "root")
                    {
                        foreach (XmlAttribute a in node.Attributes)
                        {
                            if (a.Name == "templateName")
                                templateItem.templateName = a.Value.ToLower();
                            else if (a.Name == "prefabName")
                                templateItem.prefabName = a.Value.ToLower();
                        }
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            string orgName = "";
                            string newName = "";
                            foreach (XmlAttribute a in node.ChildNodes[i].Attributes)
                            {
                                if (a.Name == "orgName")
                                    orgName = a.Value;
                                if (a.Name == "newName")
                                    newName = a.Value;
                            }
                            if (string.IsNullOrEmpty(orgName) == false && string.IsNullOrEmpty(newName) == false)
                                templateItem.nameMap[orgName] = newName;
                        }
                    }
                }

                if (string.IsNullOrEmpty(templateItem.prefabName) == false
                    && string.IsNullOrEmpty(templateItem.templateName) == false)
                    templateMap.Add(templateItem.prefabName, templateItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Initial(string path_)
        {
            nodeLst.Clear();
            keyMap.Clear();
            templateMap.Clear();

            //读取config
            LoadConfigFile(path_ + "/ragdollConfig.xml");

            //读取模板
            List<string> fileLst = FileHelper.FindFile(path_ + "/template" , new string[] { "*.xml" });
            for (int i = 0; i < fileLst.Count; i ++)
                LoadTemplateFile(fileLst[i]);
        }
        
        protected ItemD GetNodeByFit(GameObject obj_)
        {
            for (int i = nodeLst.Count - 1; i >= 0; i--)
            {
                ItemD item = nodeLst[i];
                if (item.BFit(obj_) == true)
                    return item;
            }
            return null;
        }

        protected ItemD GetNodeItemDByObj(GameObject obj_)
        {
            ItemD item = null;
            if (keyMap.ContainsKey(obj_.name) == false)
            {
                item = GetNodeByFit(obj_);
                if (item == null)
                    return null;
                keyMap[obj_.name] = item.idx;
            }

            if (keyMap.ContainsKey(obj_.name) == false)
                return null;

            return nodeLst[keyMap[obj_.name]];
        }
        
        protected ItemD GetNodeItemByTemplateName(string name_)
        {
            for (int i = 0; i < NodeLst.Count; i ++)
            {
                if (name_ == NodeLst[i].name)
                    return NodeLst[i];
            }
            return null;
        }

        public bool BEnable(GameObject obj_)
        {
            LYRagdoll rd = obj_.GetComponentInChildren<LYRagdoll>();
            if (rd == null)
                return false;
            return rd.enabled;
        }

        protected templateItemD GetTemplateItem(string prefabName_)
        {
            prefabName_ = prefabName_.ToLower();
            if (templateMap.ContainsKey(prefabName_) == false)
                return null;
            return templateMap[prefabName_];
        }

        public void EnableByNodeItem(GameObject obj_, string nodeItemName_)
        {
            LYRagdoll rd = obj_.GetComponentInChildren<LYRagdoll>();
            if (rd != null)
                rd.enabled = false;

            ItemD item = GetNodeItemByTemplateName(nodeItemName_);
            if (item == null)
                return;

            if(rd == null)
                rd = obj_.AddComponent<LYRagdoll>();
            rd.LoadFromNode(item , null);
            rd.OnEnable();
        }

        public void Clear(GameObject obj_)
        {
            LYRagdoll rd = obj_.GetComponentInChildren<LYRagdoll>();
            if (rd == null)
                return;
            rd.Clear();
        }

        public LYRagdoll Enable(GameObject obj_)
        {
            string name = obj_.name.ToLower();
            name = name.Replace("(clone)", "");

            LYRagdoll rd = obj_.GetComponentInChildren<LYRagdoll>();
            if (rd != null)
            {
                rd.enabled = true;
                return rd ;
            }

            templateItemD template = GetTemplateItem(name);
            if (template == null)
                return null;
            ItemD item = template != null ? GetNodeItemByTemplateName(template.templateName) : GetNodeItemDByObj(obj_);
            if (item == null)
                return null ;

            rd = obj_.AddComponent<LYRagdoll>();
            rd.LoadFromNode(item , template);
            rd.OnEnable();

            return rd;
        }

        public void Disable(GameObject obj_)
        {
            LYRagdoll rd = obj_.GetComponentInChildren<LYRagdoll>();
            if (rd != null)
                rd.enabled = false;
        }
    }

}