using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace SearchDuplicates
{
    static class Util
    {
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        public static string GetHash(FileStream fs)
        {
            if (fs == null) throw new ArgumentNullException("fs");
            MD5 md5 = new MD5CryptoServiceProvider();
            var fileData = new byte[fs.Length];
            fs.Read(fileData, 0, (int)fs.Length);
            byte[] checkSum = md5.ComputeHash(fileData);
            string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
            return result;
        }

        private static void AddFileSecurity(string dirName, string account, FileSystemRights rights, AccessControlType controlType)
        {   
            DirectorySecurity dSecurity = Directory.GetAccessControl(dirName);
            
            dSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));
            Directory.SetAccessControl(dirName, dSecurity);
        }
        public static void SetAccessRights(string dir)
        {
            System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (wi != null)
            {
                string user = wi.Name;
                AddFileSecurity(dir, @user,
                                     FileSystemRights.FullControl, AccessControlType.Allow);
            }
        }
    }
}
