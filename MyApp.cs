using System;
using System.IO;
using System.Windows.Forms;

namespace JasperLIB
{
    public class MyApp
    {
        private static MyXML m_aAppXML = null;
        private static string m_szFileXML = "";
        private static string m_szPath;
        private static Int32 m_nMachineNo = 0;
        private static string m_szCodeNo = "";
        private static string m_szIP = "";
        private static string m_szWebSite = "";
        //---------------------------------------------------------------------
        public static bool Init()
        {
            string szPath = System.Environment.CurrentDirectory;

            m_szFileXML = szPath + @"\Sys\AppData.xml";

            if (!File.Exists(m_szFileXML))
            {
                MessageBox.Show(m_szFileXML + " open error!");
                return false;
            }

            m_aAppXML = new MyXML();
            m_aAppXML.LoadFile(m_szFileXML);

            // 取得 Path 所在，若未設定則為當前位置。
            string buf = m_aAppXML.GetValue("/AppData/DataPath");
            if(buf == "" )
            {
                buf = szPath;
            }

            m_szPath = MyString.CPath(buf);

            // 取得機台基本設定
            m_nMachineNo = MyString.CInt(m_aAppXML.GetValue("/AppData/MachineNo"));
            m_szCodeNo = m_aAppXML.GetValue("/AppData/CodeNo");
            m_szWebSite = m_aAppXML.GetValue("/AppData/WebSite");
            m_szIP = m_aAppXML.GetValue("/AppData/IP");

            return true;
        }
        //---------------------------------------------------------------------
        public static string GetNodeValue(string szNodePath)
        {
            return m_aAppXML.GetValue(szNodePath);
        }
        //---------------------------------------------------------------------
        public static void SetNodeValue(string szNodePath, string szValue)
        {
            m_aAppXML.SetValue(szNodePath, szValue);
        }
        //---------------------------------------------------------------------
        public static void Save()
        {
            m_aAppXML.SaveFile(m_szFileXML);
        }
        //---------------------------------------------------------------------
        public static Int32 MachineNo
        {
            get { return m_nMachineNo; }
            set { m_nMachineNo = value; }
        }
        //---------------------------------------------------------------------
        public static string CodeNo
        {
            get { return m_szCodeNo; }
            set { m_szCodeNo = value; }
        }
        //---------------------------------------------------------------------
        public static string Path
        {
            get { return m_szPath; }
        }
        //---------------------------------------------------------------------
        public static string WebSite
        {
            get { return m_szWebSite; }
        }
        //---------------------------------------------------------------------
        public static string HostIP
        {
            get { return m_szIP; }
        }
        //---------------------------------------------------------------------
        public static bool FileExist(string szFile)
        {
            return File.Exists(szFile);
        }
        //---------------------------------------------------------------------
    }
}
