using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using Org.Mentalis.Security;

namespace JasperLIB
{
    public class SFTPHelper
    {
        private Session m_session;
        private Channel m_channel;
        private ChannelSftp m_sftp;
        //---------------------------------------------------------------------
        public SFTPHelper(string host, string user, string pwd)
        {
            string[] arr = host.Split(':');
            string ip = arr[0];
            int port = 22;
            if (arr.Length > 1) port = Int32.Parse(arr[1]);

            JSch jsch = new JSch();
            m_session = jsch.getSession(user, ip, port);
            ShellUserInfo ui = new ShellUserInfo();
            ui.setPassword(pwd);
            m_session.setUserInfo(ui);
        }
        //---------------------------------------------------------------------
        public bool Connected
        {
            get { return m_session.isConnected(); }
        }
        //---------------------------------------------------------------------
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    m_session.connect();
                    m_channel = m_session.openChannel("sftp");
                    m_channel.connect();
                    m_sftp = (ChannelSftp)m_channel;
                }
                return true;
            }
            catch
            {
                 return false;
            }
        }
        //---------------------------------------------------------------------
        public void Disconnect()
        {
            if (Connected)
            {
                m_channel.disconnect();
                m_session.disconnect();
            }
        }
        //---------------------------------------------------------------------
        public string Put(string localPath, string remotePath)
        {
            string error = "";
            try
            {
                Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(localPath);
                Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(remotePath);
                m_sftp.put(src, dst);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return error;
        }
        //---------------------------------------------------------------------
        public bool Get(string remotePath, string localPath)
        {
            try
            {
                Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(remotePath);
                Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(localPath);
                m_sftp.get(src, dst);

                return true;
            }
            catch
            {
                return false;
            }
        }
        //---------------------------------------------------------------------
        public bool Delete(string remoteFile)
        {
            try
            {
                m_sftp.rm(remoteFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //---------------------------------------------------------------------
        public ArrayList GetFileList(string remotePath)
        {
            try
            {
                Tamir.SharpSsh.java.util.Vector vvv = m_sftp.ls(remotePath);
                ArrayList objList = new ArrayList();
                foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry qqq in vvv)
                {
                    string sss = qqq.getFilename();
                    objList.Add(sss);
                }
                return objList;
            }
            catch
            {
                return null;
            }
        }
        //---------------------------------------------------------------------
        public void Mkdir(string parentDir, string dirName)
        {
            if (dirName == "") return;

            //  去除最右邊的 '/'。
            Int32 len = parentDir.Length;
            if (parentDir[len - 1] == '/')
            {
                parentDir.Remove(len - 1);
            }

            ArrayList aList = this.GetFileList(parentDir);

            if (aList.Contains(dirName)) return;
            m_sftp.mkdir(parentDir + "/" + dirName);
        }
    }
    //-------------------------------------------------------------------------
    //  Class ShellUserInfo 登录验证信息 
    //-------------------------------------------------------------------------
    class ShellUserInfo : UserInfo
    {
        String passwd;
        public String getPassword() { return passwd; }
        public void setPassword(String passwd) { this.passwd = passwd; }

        public String getPassphrase() { return null; }
        public bool promptPassphrase(String message) { return true; }

        public bool promptPassword(String message) { return true; }
        public bool promptYesNo(String message) { return true; }
        public void showMessage(String message) { }
    }
    //---------------------------------------------------------------------
}
