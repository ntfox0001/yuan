using System.IO;
using System.Text;
using System;
using System.Security.Cryptography;

public class MD5Utility
{
    /// <summary>
    /// 获取文件MD5值
    /// </summary>
    /// <param name="fileName">文件绝对路径</param>
    /// <returns>MD5值</returns>
    public static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }
    }
    /// <summary>
    /// 获取文件SHA1值
    /// </summary>
    /// <param name="fileName"></param>
    public static string GetSHA1FromFile(string fileName)
    {
        try
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retVal = sha1.ComputeHash(file);
            file.Close();

            StringBuilder sc = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sc.Append(retVal[i].ToString("x2"));
            }
            return sc.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetSHA1FromFile() fail,error:" + ex.Message);
        }
    }
    /// <summary>
    /// 快速获得md5
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="skip"></param>
    /// <returns></returns>
    public static string GetFastMD5HashFromFile(string fileName, int skip = 200)
    {
        try
        {
            FileInfo fi = new FileInfo(fileName);
            if (fi.Length < 1024 * 1024 * 3)
            {
                return GetMD5HashFromFile(fileName);
            }
            FileStream file = new FileStream(fileName, FileMode.Open);
            MemoryStream ms = new MemoryStream();
            byte[] buff = new byte[1];

            int rc = 0;
            while (true)
            {
                rc = file.Read(buff, 0, 1);

                if (rc == 1)
                {
                    file.Seek(skip, SeekOrigin.Current);
                    ms.WriteByte(buff[0]);
                }
                else
                {
                    break;
                }
            }
            file.Close();
            ms.Seek(0, SeekOrigin.Begin);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(ms);
            ms.Close();
            ms = null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetFastMD5HashFromFile() fail,error:" + ex.Message);
        }
    }

    public static string GetFastSHA1HashFromFile(string fileName, int skip = 200)
    {
        try
        {
            FileInfo fi = new FileInfo(fileName);
            if (fi.Length < 1024 * 1024 * 3)
            {
                return GetSHA1FromFile(fileName);
            }
            FileStream file = new FileStream(fileName, FileMode.Open);
            MemoryStream ms = new MemoryStream();
            byte[] buff = new byte[1];

            int rc = 0;
            while (true)
            {
                rc = file.Read(buff, 0, 1);

                if (rc == 1)
                {
                    file.Seek(skip, SeekOrigin.Current);
                    ms.WriteByte(buff[0]);
                }
                else
                {
                    break;
                }
            }
            file.Close();
            ms.Seek(0, SeekOrigin.Begin);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retVal = sha1.ComputeHash(ms);
            ms.Close();
            ms = null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetFastSHA1HashFromFile() fail,error:" + ex.Message);
        }
    }
}
