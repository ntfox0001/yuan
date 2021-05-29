using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StringFile : IDataFile<string>
{
    public virtual string Load(string filename)
    {
        if (File.Exists(filename))
        {
            return File.ReadAllText(filename, System.Text.Encoding.UTF8);
        }
        return null;
    }

    public virtual void Save(string filename, string data)
    {
        File.WriteAllText(filename, data, System.Text.Encoding.UTF8);
    }
}


public class StringEncryptFile : StringFile
{
    protected string mPassword;
    public StringEncryptFile(string pw)
    {
        mPassword = pw;
    }
    public override string Load(string filename)
    {
        if (!File.Exists(filename)) return null;
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        if (fs != null)
        {
            MemoryStream ms = new MemoryStream();

            if (EncryptUtility.SHA_Dencrypt(fs, ms, mPassword) == EncryptUtility.Error.OK)
            {
                ms.Position = 0;

                StreamReader sr = new StreamReader(ms);

                string content = sr.ReadToEnd();
                sr.Close();
                ms.Close();
                fs.Close();
                return content;
            }

            ms.Close();
            fs.Close();
        }
        return null;
    }
    public override void Save(string filename, string data)
    {
        MemoryStream ms = new MemoryStream();
        StreamWriter sw = new StreamWriter(ms);
        sw.WriteLine(data);
        sw.Flush();
        FileStream fs;
        try
        {
            fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        }
        catch (System.Exception e)
        {
            Debug.LogError("StringEncryptFile error:" + e.Message);
            return;
        }

        ms.Position = 0;

        EncryptUtility.SHA_Encrypt(ms, fs, mPassword);

        sw.Close();
        fs.Close();
    }
}