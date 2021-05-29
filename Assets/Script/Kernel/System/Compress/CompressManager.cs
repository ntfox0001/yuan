using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;

public class CompressManager : MonoBehaviour 
{
    static public void Compress7zip(Stream input, Stream output)
    {
        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
        coder.WriteCoderProperties(output);

        output.Write(System.BitConverter.GetBytes(input.Length), 0, 8);
        coder.Code(input, output, input.Length, -1, null);

    }
    static public void Decompress7zip(Stream input, Stream output)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);

        // Read in the decompress file size.
        byte[] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = System.BitConverter.ToInt64(fileLengthBytes, 0);

        // Decompress the file.
        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
    }
    /// <summary>
    ///  compress zlib
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output">close in function</param>
    static public void Compresszlib(Stream input, Stream output)
    {
        ComponentAce.Compression.Libs.zlib.ZOutputStream outZStream =
            new ComponentAce.Compression.Libs.zlib.ZOutputStream(output, ComponentAce.Compression.Libs.zlib.zlibConst.Z_DEFAULT_COMPRESSION);

        try
        {
            CopyStream(input, outZStream);
        }
        finally
        {
            outZStream.Close();
        }

    }
    /// <summary>
    /// decompress zlib
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output">close in function</param>
    static public void Decompresszlib(Stream input, Stream output)
    {
        ComponentAce.Compression.Libs.zlib.ZOutputStream outZStream = new ComponentAce.Compression.Libs.zlib.ZOutputStream(output);
        try
        {
            CopyStream(input, outZStream);
        }
        finally
        {
            outZStream.Close();
        }
    }

    public static void CopyStream(Stream src, Stream dest)
    {
        byte[] buffer = new byte[4096];
        int len;
        while ((len = src.Read(buffer, 0, buffer.Length)) > 0)
        {
            dest.Write(buffer, 0, len);
        }
    }

    public static void CompressGzip(Stream input, Stream output)
    {
        using (GZipStream compressionStream = new GZipStream(output, CompressionMode.Compress))
        {
            CopyStream(input, compressionStream);
        }
    }

    public static void DecompressGzip(Stream input, Stream output)
    {
        using (GZipStream decompressionStream = new GZipStream(input, CompressionMode.Decompress))
        {
            CopyStream(decompressionStream, output);
        }
    }

    public static void CompressFile(string content, string file, System.Action<Stream, Stream> compressFunc)
    {
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write(content);
        
        ms.Position = 0;

        FileStream fs = new FileStream(file, FileMode.Create);
        compressFunc(ms, fs);
        ms.Close();
        fs.Close();
        bw.Close();
    }
    public static void DecompressFile(string file, out string content, System.Action<Stream, Stream> decompressFunc)
    {
        FileStream fs = new FileStream(file, FileMode.Open);
        MemoryStream ms = new MemoryStream();
        decompressFunc(fs, ms);

        fs.Close();

        ms.Position = 0;
        BinaryReader br = new BinaryReader(ms);
        content = br.ReadString();
        br.Close();
        ms.Close();
    }

    public static void CompressString(string content, out byte[] buffer, System.Action<Stream, Stream> compressFunc)
    {
        MemoryStream inputms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(inputms);
        bw.Write(System.Text.Encoding.UTF8.GetBytes(content));
        inputms.Position = 0;

        MemoryStream outputms = new MemoryStream();

        compressFunc(inputms, outputms);

        bw.Close();
        inputms.Close();

        outputms.Position = 0;
        BinaryReader br = new BinaryReader(outputms);
        buffer = br.ReadBytes((int)outputms.Length);
        br.Close();
        outputms.Close();
    }
    public static void DecompressString(byte[] buffer, out string content, System.Action<Stream, Stream> decompressFunc)
    {
        MemoryStream inputms = new MemoryStream();
        inputms.Write(buffer, 0, buffer.Length);
        inputms.Position = 0;

        MemoryStream outputms = new MemoryStream();

        decompressFunc(inputms, outputms);

        inputms.Close();

        outputms.Position = 0;
        BinaryReader br = new BinaryReader(outputms);
        var buf = br.ReadBytes((int)outputms.Length);
        content = System.Text.Encoding.UTF8.GetString(buf, 0, buf.Length);
        br.Close();
        outputms.Close();
    }
}
