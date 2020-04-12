using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JasperLIB;

namespace CheckVersion
{
    public partial class frmMain : Form
    {
        private Int32 m_nMachineNo;       
        //---------------------------------------------------------------------
        public frmMain()
        {
            InitializeComponent();
            if (!MyApp.Init())
            {
                this.Close();
            }
        }
        //---------------------------------------------------------------------
        private void frmMain_Shown(object sender, EventArgs e)
        {
            if( MyApp.MachineNo == 0 )
            {
                dlgShopInfo aDlg = new dlgShopInfo();

                aDlg.ShowDialog();
                m_nMachineNo = aDlg.MachineNo;
                if( m_nMachineNo == 0 )
                {
                    this.Close();
                }
            }
            else
            {
                m_nMachineNo = MyApp.MachineNo;
            }

            CheckUpdate();
            this.Close();
        }
        //---------------------------------------------------------------------
        private void CheckUpdate()
        {
            string lastUpdate = MyApp.GetNodeValue("/AppData/LastUpdate");
            
            MyWebPage myWebPage = new MyWebPage(MyApp.HostIP);
            if (myWebPage.PingHost())
            {
                string szURL = MyApp.WebSite + "/api/CheckUpdate.php";
                string param = "t1=" + lastUpdate;

                string buf = myWebPage.GetResponseString(szURL, param);
                if (buf != "")
                {
                    string[] aAry = buf.Split(',');
                    for (Int32 i = 0; i < aAry.Length; i += 2)
                    {
                        DownFile(aAry[i], aAry[i+1]);
                    }
                    listBox1.Items.Add("Update OK!");
                    lastUpdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    MyApp.SetNodeValue("/AppData/LastUpdate", lastUpdate);
                    MyApp.Save();
                }
                else
                {
                    listBox1.Items.Add("No Updated");
                }
            }             
        }
        //---------------------------------------------------------------------
        private void DownFile(string szFileName, string szSaveTo)
        {
            bool bRet = false;
            listBox1.Items.Add("Download " + szFileName);
            listBox1.Refresh();

            SFTPHelper sftp = new SFTPHelper(MyApp.HostIP, "root", "seiwa5588");
            if (sftp.Connect())
            {
                string szURL = "/var/www/html/version_update/" + szFileName;
                string szPath = MyApp.Path + szSaveTo;

                // 檢查資料夾是否存在？若不存在，則建立。
                if( !Directory.Exists(szPath) )
                {
                    DirectoryInfo szDir = Directory.CreateDirectory(szPath);
                }

                bRet = sftp.Get(szURL, szPath);

                if (bRet)
                {
                    listBox1.Items.Add("Download " + szFileName + " ---- OK!");
                    if( szSaveTo == "version_update")
                    {
                        string srcFile = szPath + @"\" + szFileName;
                        string destFile = System.Environment.CurrentDirectory + @"\" + szFileName;

                        File.Copy(srcFile, destFile, true);
                        File.Delete(srcFile);
                    }                    
                }                    
                else
                {
                    listBox1.Items.Add("Download " + szFileName + " ---- FAIL!");
                }
            }
            sftp.Disconnect();
            listBox1.Refresh();
        }
        //---------------------------------------------------------------------
    }
}
