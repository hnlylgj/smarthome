using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ALiYunOSSOTSAPILib
{
    public class ALiYunOSSOTSLogin
    {

        public string MyAccessID;
        public string MyAccessKey;
        public string MyBucketName;
        public string PrefixStr;
        public string FileKeyName;

        public ALiYunOSSOTSLogin()
        {
            MyAccessID = "7je124e077a86vzr6zqtpazy";
            MyAccessKey = "SQYaadscvGhFuTnm3U99JugIpQw=";
            MyBucketName = "hnlylgj";
            PrefixStr = "A9765432BCDA828";
            PrefixStr = PrefixStr + "/";
            FileKeyName = "";

        }

        public ALiYunOSSOTSLogin(string InPrefixStr)
        {
            MyAccessID = "7je124e077a86vzr6zqtpazy";
            MyAccessKey = "SQYaadscvGhFuTnm3U99JugIpQw=";
            MyBucketName = "hnlylgj";
            PrefixStr = InPrefixStr;
            PrefixStr = PrefixStr + "/";
            FileKeyName = "";

        }

    }
}
