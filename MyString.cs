using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JasperLIB
{
    class MyString
    {
        //---------------------------------------------------------------------
        public static string StrLeft(string szValue, int nLen)
        {
            if (szValue == "") return "";
            if (nLen < 1) return "";
            if (nLen >= szValue.Length) return szValue;
            return szValue.Substring(0, nLen);
        }
        //---------------------------------------------------------------------
        public static string StrMID(string szValue, int nStart, int nLen)
        {
            if (szValue == "") return "";
            if (nLen < 1) return "";
            if (nStart < 0) nStart = 0;
            int nCX = szValue.Length - nStart;
            if (nLen > nCX) nLen = nCX;

            return szValue.Substring(nStart, nLen);
        }
        //---------------------------------------------------------------------
        public static string StrRight(string szValue, int nLen)
        {
            if (szValue == "") return "";
            if (nLen < 1) return "";
            if (nLen >= szValue.Length) return szValue;
            int nStart = szValue.Length - nLen;
            return szValue.Substring(nStart, nLen);
        }
        //---------------------------------------------------------------------
        public static string CPath(string szPath)
        {
            string buf = szPath;

            if( MyString.StrRight(szPath, 1) != @"\") 
            {
                buf = szPath + @"\";
            }

            return buf;
        }
        //---------------------------------------------------------------------
        public static Int32 CInt(string szValue)
        {
            if (szValue == null || szValue == "") return 0;
            try
            {
                int i = szValue.IndexOf(".");
                if (i > 0)
                {
                    szValue = szValue.Substring(0, i);
                }
                Int32 nCX = Int32.Parse(szValue);
                return nCX;
            }
            catch
            {
                return 0;
            }
        }
        //---------------------------------------------------------------------
    }
}
