using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JasperLIB;

namespace CheckVersion
{
    public partial class dlgShopInfo : Form
    {
        public Int32 MachineNo { get; set; }

        private MyXML m_aXML = null;
        private string m_szFileXML = "";
        private string m_szPhotoCount = "";
        private bool m_bFlag = false;
        //---------------------------------------------------------------------
        public dlgShopInfo()
        {
            InitializeComponent();

            m_szFileXML = MyApp.Path + @"Sys\USBWatch.xml";

            m_aXML = new MyXML();
            m_aXML.LoadFile(m_szFileXML);

            txtCamera.Text = m_aXML.GetValue("/USBWatch/USBInfo/CameraName");
            txtPrinter.Text = m_aXML.GetValue("/USBWatch/USBInfo/PrinterName");
        }
        //---------------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {
            btnOK.Enabled = false;

            if( m_bFlag)
            {
                SaveData();
                MachineEnabled(this.MachineNo.ToString());

                this.Close();
            }
            else
            {
                string szCodeNo = txtCodeNo.Text.Trim();
                if(szCodeNo != "")
                {
                    m_bFlag = GetShopName(szCodeNo);
                    if( !m_bFlag )
                    {
                        txtCodeNo.Focus();
                    }
                    else
                    {
                        btnOK.Text = "Confirm";
                    }
                    btnOK.Enabled = true;
                }
            }
        }
        //---------------------------------------------------------------------
        private bool GetShopName(string szNo)
        {
            bool bFlag = false;
 
            MyWebPage myWebPage = new MyWebPage(MyApp.HostIP);
            if (myWebPage.PingHost())
            {
                string szURL = MyApp.WebSite + "/api/GetShopName.php";
                string param = "CodeNo=" + szNo;

                string buf = myWebPage.GetResponseString(szURL, param);
                if (buf != "")
                {
                    string[] aAry = buf.Split(',');

                    this.MachineNo = Int32.Parse(aAry[0]);
                    m_szPhotoCount = aAry[1];
                    txtDeviceID.Text = aAry[2];
                    txtShopName.Text = aAry[3];

                    bFlag = true;
                }
                else
                {
                    MessageBox.Show("ERR, CodeNo！");
                }
            }
            else
            {
                MessageBox.Show("ERR, PingHost()！");
            }

            return bFlag;
        }
        //---------------------------------------------------------------------
        private void SaveData()
        {
            m_aXML.SetValue("/USBWatch/USBInfo/CameraName", txtCamera.Text.Trim());
            m_aXML.SetValue("/USBWatch/USBInfo/PrinterName", txtPrinter.Text.Trim());

            m_aXML.SaveFile(m_szFileXML);

            MyApp.SetNodeValue("/AppData/MachineNo", this.MachineNo.ToString());
            MyApp.SetNodeValue("/AppData/CodeNo", txtCodeNo.Text.Trim());
            MyApp.SetNodeValue("/AppData/DeviceID", txtDeviceID.Text.Trim());
            MyApp.SetNodeValue("/AppData/ShopName", txtShopName.Text.Trim());
            MyApp.SetNodeValue("/AppData/PhotoCount", m_szPhotoCount.Trim());

            if( MyString.CInt(m_szPhotoCount) == 0 )
            {
                string szToday = DateTime.Now.ToString("yyyy-MM-dd");
                MyApp.SetNodeValue("/AppData/StartDate", szToday);
            }
            
            MyApp.Save();

            string buf = MyApp.Path + @"Sys\Setup.xml";

            MyXML aXML = new MyXML();
            aXML.LoadFile(buf);

            aXML.SetValue("/Setup/PrintHeaderInfo/ShopName", txtShopName.Text.Trim());
            aXML.SetValue("/Setup/PrintHeaderInfo/CodeNo", txtCodeNo.Text.Trim());
            aXML.SetValue("/Setup/PrintHeaderInfo/MachineNo", txtDeviceID.Text.Trim());

            aXML.SaveFile(buf);
        }
        //---------------------------------------------------------------------
        private bool MachineEnabled(string szNo)
        {
            bool bFlag = true;
            MyWebPage myWebPage = new MyWebPage(MyApp.HostIP);
            if (myWebPage.PingHost())
            {
                string szURL = MyApp.WebSite + "/api/MachineEnabled.php";
                string param = "ANo=" + szNo;

                string buf = myWebPage.GetResponseString(szURL, param);
                if (buf == "")
                {
                    bFlag = false;
                }
            }
            else
            {
                bFlag = false;
            }
            return bFlag;
        }
        //---------------------------------------------------------------------
    }
}
