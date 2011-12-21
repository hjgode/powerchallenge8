using System;

using System.Collections.Generic;
using System.Text;

namespace PowerChallenge
{
    public static class ssAPIhelper
    {
        public static string getStrSetting(StringBuilder sb, String sField)
        {
            string sVal = sb.ToString();
            string sFieldString = "<Field Name=\"" + sField + "\">";
            int iIdx = sVal.IndexOf(sFieldString);
            string sResult = "";
            if (iIdx >= 0)
            {
                string fieldStr = sVal.Substring(iIdx);
                string ssidStr = fieldStr.Remove(0, sFieldString.Length);
                iIdx = ssidStr.IndexOf("<");
                sResult = ssidStr.Substring(0, iIdx);
            }
            return sResult;
        }

        public static int getIntSetting(StringBuilder sb, String sField)
        {
            string sVal = sb.ToString();
            string sFieldString = "<Field Name=\"" + sField + "\">";
            int iIdx = sVal.IndexOf(sFieldString);
            int iRes = -1;
            if (iIdx >= 0)
            {
                string fieldStr = sVal.Substring(iIdx);
                string ssidStr = fieldStr.Remove(0, sFieldString.Length);
                iIdx = ssidStr.IndexOf("<");
                string sResult = ssidStr.Substring(0, iIdx);
                try
                {
                    iRes = Convert.ToInt16(sResult);
                }
                catch (Exception)
                {
                }
            }
            return iRes;
        }
        public static bool getBoolSetting(StringBuilder sb, String sField)
        {
            string sVal = sb.ToString();
            string sFieldString = "<Field Name=\"" + sField + "\">";
            int iIdx = sVal.IndexOf(sFieldString);
            if (iIdx >= 0)
            {
                string fieldStr = sVal.Substring(iIdx);
                string ssidStr = fieldStr.Remove(0, sFieldString.Length);
                iIdx = ssidStr.IndexOf("<");
                string sResult = ssidStr.Substring(0, iIdx);
                if (sResult.Equals("1"))
                    return true;
            }
            return false;
        }
    }
}
