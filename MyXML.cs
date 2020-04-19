using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JasperLIB
{
    public class MyXML
    {
        private XmlDocument m_aXmlDoc;
        //---------------------------------------------------------------------
        public MyXML()
        {
            m_aXmlDoc = new XmlDocument();
        }
        //---------------------------------------------------------------------
        public MyXML(string szData)
        {
            m_aXmlDoc = new XmlDocument();
            this.LoadString(szData);
        }
        //---------------------------------------------------------------------
        public void ReSet()
        {
            XmlElement root = m_aXmlDoc.DocumentElement;
            root.RemoveAll();
        }
        //---------------------------------------------------------------------
        public void LoadFile(string szFile)
        {
            m_aXmlDoc.Load(szFile);
        }
        //---------------------------------------------------------------------
        public void SaveFile(string szFile)
        {
            m_aXmlDoc.Save(szFile);
        }
        //---------------------------------------------------------------------
        public void LoadString(string szXML)
        {
            m_aXmlDoc.LoadXml(szXML);
        }
        //---------------------------------------------------------------------
        public XmlNodeList GetNodeList(string szNodePath)
        {
            XmlNodeList aNodes = m_aXmlDoc.SelectNodes(szNodePath);
            return aNodes;
        }
        //---------------------------------------------------------------------
        public XmlNode GetNode(string szNodePath)
        {
            XmlNode aNode = m_aXmlDoc.SelectSingleNode(szNodePath);
            return aNode;
        }
        //---------------------------------------------------------------------
        public XmlNode GetNode(XmlNode aNode, string szChildName)
        {
            XmlNode aChildNode = aNode.SelectSingleNode(szChildName);

            return aChildNode;
        }
        //---------------------------------------------------------------------
        public void InsertNode(string szParent, string szName, string szValue)
        {
            XmlNode aNode = GetNode(szParent);
            InsertNode(aNode, szName, szValue);
        }
        //---------------------------------------------------------------------
        public void InsertNode(XmlNode aNode, string szChildName, string szValue)
        {
            XmlElement aElem = m_aXmlDoc.CreateElement(szChildName);
            XmlText aText = m_aXmlDoc.CreateTextNode(szValue);
            aElem.AppendChild(aText);

            if (aNode == null)
                m_aXmlDoc.DocumentElement.AppendChild(aElem);
            else
                aNode.AppendChild(aElem);
        }
        //---------------------------------------------------------------------
        private void InsertNode(string szNodePath, string szValue)
        {
            if (szNodePath == "") return;

            Int32 i = szNodePath.LastIndexOf("/");
            if (i > 0)
            {
                string szParent = szNodePath.Substring(0, i);
                string szNodeName = szNodePath.Substring(i + 1);

                InsertNode(szParent, szNodeName, szValue);
            }
        }
        //---------------------------------------------------------------------
        public void RemoveNode(XmlNode aNode)
        {
            if (aNode == null) return;
            aNode.ParentNode.RemoveChild(aNode);
        }
        //---------------------------------------------------------------------
        public void RemoveNode(string szName)
        {
            XmlNode aNode = this.GetNode(szName);
            if (aNode == null) return;
            aNode.ParentNode.RemoveChild(aNode);
        }
        //---------------------------------------------------------------------
        public string GetValue(XmlNode aNode, string szChildName)
        {
            XmlNode aChildNode = aNode.SelectSingleNode(szChildName);
            if (aChildNode == null)
                return "";
            else
                return aChildNode.InnerText;
        }
        //---------------------------------------------------------------------
        public string GetValue(string szNodePath)
        {
            XmlNode aNode = GetNode(szNodePath);
            if (aNode == null)
                return "";
            else
                return aNode.InnerText;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// 設定某一 Node 的值，若不存在，則新增一個 Node 在根節點之下。
        /// </summary>
        /// <param name="szName">Node 名稱</param>
        /// <param name="szValue">Node 值</param>
        public void SetValue(string szNodePath, string szValue)
        {
            XmlNode aNode = GetNode(szNodePath);
            if (aNode != null)
                aNode.InnerText = szValue;
            else
                InsertNode(szNodePath, szValue);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// 設定某一 Node 的值，若不存在，則新增一個 Node 在 szParent 之下。
        /// </summary>
        /// <param name="szParent">父節點名稱</param>
        /// <param name="szName">Node 名稱</param>
        /// <param name="szValue">Node 值</param>
        public void SetValue(string szParent, string szName, string szValue)
        {
            XmlNode aNode = GetNode(szName);
            if (aNode != null)
                aNode.InnerText = szValue;
            else
                InsertNode(szParent, szName, szValue);
        }
        //---------------------------------------------------------------------
        public string GetAttr(string szName, string szAttr)
        {
            XmlNode aNode = GetNode(szName);
            if (aNode == null)
                return "";
            else
                return GetAttr(aNode, szAttr);
        }
        //---------------------------------------------------------------------
        public string GetAttr(XmlNode aNode, string szAttr)
        {
            XmlElement elem = (XmlElement)aNode;
            if (elem.HasAttribute(szAttr))
            {
                XmlAttribute attr = elem.GetAttributeNode(szAttr);
                return attr.Value;
            }
            else
                return "";
        }
        //---------------------------------------------------------------------
        public void SetAttr(string szName, string szAttr, string szValue)
        {
            XmlNode aNode = GetNode(szName);
            if (aNode == null)
            {
                InsertNode(aNode, szName, "");
                aNode = GetNode(szName);
            }
            SetAttr(aNode, szAttr, szValue);
        }
        //---------------------------------------------------------------------
        public void SetAttr(XmlNode aNode, string szAttr, string szValue)
        {
            XmlElement elem = (XmlElement)aNode;
            XmlAttribute attr;
            if (elem.HasAttribute(szAttr))
            {
                if (szValue == "")
                    this.RemoveAttr(aNode, szAttr);
                else
                {
                    attr = elem.GetAttributeNode(szAttr);
                    attr.Value = szValue;
                }
            }
            else
            {
                if (szValue != "")
                {
                    attr = m_aXmlDoc.CreateAttribute(szAttr);
                    attr.Value = szValue;
                    elem.SetAttributeNode(attr);
                }
            }
        }
        //---------------------------------------------------------------------
        public void RemoveAttr(XmlNode aNode, string szAttr)
        {
            XmlElement elem = (XmlElement)aNode;
            elem.RemoveAttribute(szAttr);
        }
        //---------------------------------------------------------------------
    }
}
