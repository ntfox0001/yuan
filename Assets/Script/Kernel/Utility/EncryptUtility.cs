using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

public class EncryptUtility
{
    public enum Error
    {
        OK,
        InvalidFile,
        InvalidSize,
    }
    private const ulong FC_TAG = 0xFC010203040506CF;
    private const int BUFFER_SIZE = 128 * 1024;
    //检验两个Byte数组是否相同 
    private static bool CheckByteArrays(byte[] b1, byte[] b2)
    {
        if (b1.Length == b2.Length)
        {
            for (int i = 0; i < b1.Length; ++i)
            {
                if (b1[i] != b2[i])
                    return false;
            }
            return true;
        }
        return false;
    }
    /// <param name="password">密码</param> 
    /// <param name="salt"></param> 
    /// <returns>加密对象</returns> 
    private static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
    {
        PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt, "SHA256", 1000);
        SymmetricAlgorithm sma = Rijndael.Create();
        sma.KeySize = 256;
        sma.Key = pdb.GetBytes(32);
        sma.Padding = PaddingMode.PKCS7;
        return sma;
    }
    // 加密文件随机数生成 
    private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();
    // 生成指定长度的随机Byte数组 
    private static byte[] GenerateRandomBytes(int count)
    {
        byte[] bytes = new byte[count];
        rand.GetBytes(bytes);
        return bytes;
    }
    public static Error SHA_Encrypt(Stream inStream, Stream outStream, string password)
    {
        long lSize = inStream.Length; // 输入文件长度 
        int size = (int)lSize;
        byte[] bytes = new byte[BUFFER_SIZE]; // 缓存 
        int read = -1; // 输入文件读取数量 
        int value = 0;
        // 获取IV和salt 
        byte[] IV = GenerateRandomBytes(16);
        byte[] salt = GenerateRandomBytes(16);
        // 创建加密对象 
        SymmetricAlgorithm sma = CreateRijndael(password, salt);
        sma.IV = IV;
        // 在输出文件开始部分写入IV和salt 
        outStream.Write(IV, 0, IV.Length);
        outStream.Write(salt, 0, salt.Length);
        // 创建散列加密 
        HashAlgorithm hasher = SHA256.Create();
        using (CryptoStream cout = new CryptoStream(outStream, sma.CreateEncryptor(), CryptoStreamMode.Write),
        chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
        {
            BinaryWriter bw = new BinaryWriter(cout);
            bw.Write(lSize);
            bw.Write(FC_TAG);
            // 读写字节块到加密流缓冲区 
            while ((read = inStream.Read(bytes, 0, bytes.Length)) != 0)
            {
                cout.Write(bytes, 0, read);
                chash.Write(bytes, 0, read);
                value += read;
            }
            // 关闭加密流 
            chash.Flush();
            chash.Close();
            // 读取散列 
            byte[] hash = hasher.Hash;
            // 输入文件写入散列 
            cout.Write(hash, 0, hash.Length);
            // 关闭文件流 
            cout.Flush();
            cout.Close();
        }
        return Error.OK;
    }

    // 加密文件 
    public static Error SHA_Encrypt(string inFile, string outFile, string password)
    {
        Error err;
        using (FileStream fin = File.OpenRead(inFile),
        fout = File.OpenWrite(outFile))
        {
            err = SHA_Encrypt(fin, fout, password);
        }
        return err;
    }
    public static Error SHA_Dencrypt(Stream inStream, Stream outStream, string password)
    {
        int size = (int)inStream.Length;
        byte[] bytes = new byte[BUFFER_SIZE];
        int read = -1;
        int value = 0;
        int outValue = 0;
        byte[] IV = new byte[16];
        inStream.Read(IV, 0, 16);
        byte[] salt = new byte[16];
        inStream.Read(salt, 0, 16);
        SymmetricAlgorithm sma = CreateRijndael(password, salt);
        sma.IV = IV;
        value = 32;
        long lSize = -1;
        // 创建散列对象, 校验文件 
        HashAlgorithm hasher = SHA256.Create();
        try
        {
            using (CryptoStream cin = new CryptoStream(inStream, sma.CreateDecryptor(), CryptoStreamMode.Read),
            chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
            {
                // 读取文件长度 
                BinaryReader br = new BinaryReader(cin);
                lSize = br.ReadInt64();
                ulong tag = br.ReadUInt64();
                if (FC_TAG != tag)
                    return Error.InvalidFile;
                long numReads = lSize / BUFFER_SIZE;
                long slack = (long)lSize % BUFFER_SIZE;
                for (int i = 0; i < numReads; ++i)
                {
                    read = cin.Read(bytes, 0, bytes.Length);
                    outStream.Write(bytes, 0, read);
                    chash.Write(bytes, 0, read);
                    value += read;
                    outValue += read;
                }
                if (slack > 0)
                {
                    read = cin.Read(bytes, 0, (int)slack);
                    outStream.Write(bytes, 0, read);
                    chash.Write(bytes, 0, read);
                    value += read;
                    outValue += read;
                }
                chash.Flush();
                chash.Close();
                outStream.Flush();
                //outStream.Close(); // 不要关闭
                byte[] curHash = hasher.Hash;
                // 获取比较和旧的散列对象 
                byte[] oldHash = new byte[hasher.HashSize / 8];
                read = cin.Read(oldHash, 0, oldHash.Length);
                if ((oldHash.Length != read) || (!CheckByteArrays(oldHash, curHash)))
                    return Error.InvalidFile;
            }

        }
        catch (Exception)
        {
            return Error.InvalidFile;
        }
        if (outValue != lSize)
            return Error.InvalidSize;

        return Error.OK;
    }
    // 解密文件 
    public static Error SHA_Dencrypt(string inFile, string outFile, string password)
    {
        Error err;
        // 创建打开文件流 
        using (FileStream fin = File.OpenRead(inFile),
        fout = File.OpenWrite(outFile))
        {
            err = SHA_Dencrypt(fin, fout, password);
        }
        return err;
    }
}
