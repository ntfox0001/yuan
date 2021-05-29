using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class WaveInfo
{
    public class WaveHeader
    {
        public RIFF_WAVE_Chunk _Header = new RIFF_WAVE_Chunk();
        public Format_Chunk _Format = new Format_Chunk();
        public Fact_Chunk _Fact = new Fact_Chunk();
        public Data_Chunk _Data = new Data_Chunk();
    };

    public static void WriteHeader(FileStream fileStream, int frequency, int channel, int samples) 
    {  
        fileStream.Seek(0, SeekOrigin.Begin);  
  
        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");  
        fileStream.Write(riff, 0, 4);  
  
        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);  
        fileStream.Write(chunkSize, 0, 4);  
  
        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");  
        fileStream.Write(wave, 0, 4);  
  
        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");  
        fileStream.Write(fmt, 0, 4);  
  
        Byte[] subChunk1 = BitConverter.GetBytes(16);  
        fileStream.Write(subChunk1, 0, 4);  
  
        UInt16 two = 2;  
        UInt16 one = 1;  
  
        Byte[] audioFormat = BitConverter.GetBytes(one);  
        fileStream.Write(audioFormat, 0, 2);  
  
        Byte[] numChannels = BitConverter.GetBytes(channel);  
        fileStream.Write(numChannels, 0, 2);  
  
        Byte[] sampleRate = BitConverter.GetBytes(frequency);  
        fileStream.Write(sampleRate, 0, 4);  
  
        Byte[] byteRate = BitConverter.GetBytes(frequency * channel * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2  
        fileStream.Write(byteRate, 0, 4);  
  
        UInt16 blockAlign = (ushort) (channel * 2);  
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);  
  
        UInt16 bps = 16;  
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);  
        fileStream.Write(bitsPerSample, 0, 2);  
  
        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");  
        fileStream.Write(datastring, 0, 4);  
  
        Byte[] subChunk2 = BitConverter.GetBytes(samples * channel * 2);  
        fileStream.Write(subChunk2, 0, 4);  
    }
    public static WaveHeader LoadWave(FileStream wavFile)
    {
        WaveHeader head = new WaveHeader();
        #region RIFF_WAVE_Chunk
        byte[] _Temp4 = new byte[4];
        byte[] _Temp2 = new byte[2];
        wavFile.Read(_Temp4, 0, 4);
        if (_Temp4[0] != head._Header.szRiffID[0] || _Temp4[1] != head._Header.szRiffID[1] || _Temp4[2] != head._Header.szRiffID[2] || _Temp4[3] != head._Header.szRiffID[3]) return null;
        wavFile.Read(_Temp4, 0, 4);
        head._Header.dwRiffSize = BitConverter.ToUInt32(_Temp4, 0);
        wavFile.Read(_Temp4, 0, 4);
        if (_Temp4[0] != head._Header.szRiffFormat[0] || _Temp4[1] != head._Header.szRiffFormat[1] || _Temp4[2] != head._Header.szRiffFormat[2] || _Temp4[3] != head._Header.szRiffFormat[3]) return null;

        #endregion
        #region Format_Chunk
        wavFile.Read(_Temp4, 0, 4);
        if (_Temp4[0] != head._Format.ID[0] || _Temp4[1] != head._Format.ID[1] || _Temp4[2] != head._Format.ID[2]) return null;
        wavFile.Read(_Temp4, 0, 4);
        head._Format.Size = BitConverter.ToUInt32(_Temp4, 0);
        long _EndWave = head._Format.Size + wavFile.Position;
        wavFile.Read(_Temp2, 0, 2);
        head._Format.FormatTag = BitConverter.ToUInt16(_Temp2, 0);
        wavFile.Read(_Temp2, 0, 2);
        head._Format.Channels = BitConverter.ToUInt16(_Temp2, 0);
        wavFile.Read(_Temp4, 0, 4);
        head._Format.SamlesPerSec = BitConverter.ToUInt32(_Temp4, 0);
        wavFile.Read(_Temp4, 0, 4);
        head._Format.AvgBytesPerSec = BitConverter.ToUInt32(_Temp4, 0);
        wavFile.Read(_Temp2, 0, 2);
        head._Format.BlockAlign = BitConverter.ToUInt16(_Temp2, 0);
        wavFile.Read(_Temp2, 0, 2);
        head._Format.BitsPerSample = BitConverter.ToUInt16(_Temp2, 0);
        wavFile.Position += _EndWave - wavFile.Position;
        #endregion
        wavFile.Read(_Temp4, 0, 4);
        if (_Temp4[0] == head._Fact.ID[0] && _Temp4[1] == head._Fact.ID[1] && _Temp4[2] == head._Fact.ID[2] && _Temp4[3] == head._Fact.ID[3])
        {
            #region  Fact_Chunk
            wavFile.Read(_Temp4, 0, 4);
            head._Fact.Size = BitConverter.ToUInt32(_Temp4, 0);
            wavFile.Position += head._Fact.Size;
            #endregion
            wavFile.Read(_Temp4, 0, 4);
        }
        if (_Temp4[0] == head._Data.ID[0] && _Temp4[1] == head._Data.ID[1] && _Temp4[2] == head._Data.ID[2] && _Temp4[3] == head._Data.ID[3])
        {
            #region Data_Chunk
            wavFile.Read(_Temp4, 0, 4);
            head._Data.Size = BitConverter.ToUInt32(_Temp4, 0);
            head._Data.FileBeginIndex = wavFile.Position;
            head._Data.FileOverIndex = wavFile.Position + head._Data.Size;
            #endregion
        }

        return head;
    }
    #region 文件定义
    /// <summary>
    /// 文件头
    /// </summary>
    public class RIFF_WAVE_Chunk
    {
        /// <summary>
        /// 文件前四个字节 为RIFF
        /// </summary>
        public byte[] szRiffID = new byte[] { 0x52, 0x49, 0x46, 0x46 };   // 'R','I','F','F'
        /// <summary>
        /// 数据大小 这个数字等于+8 =文件大小
        /// </summary>
        public uint dwRiffSize = 0;
        /// <summary>
        ///WAVE文件定义 为WAVE
        /// </summary>
        public byte[] szRiffFormat = new byte[] { 0x57, 0x41, 0x56, 0x45 }; // 'W','A','V','E'         
    }
    /// <summary>
    /// 声音内容定义
    /// </summary>
    public class Format_Chunk
    {
        /// <summary>
        /// 固定为  是"fmt "字后一位为0x20
        /// </summary>
        public byte[] ID = new byte[] { 0x66, 0x6D, 0x74, 0x20 };
        /// <summary>
        /// 区域大小
        /// </summary>
        public uint Size = 0;
        /// <summary>
        /// 记录着此声音的格式代号，例如1-WAVE_FORMAT_PCM， 2-WAVE_F0RAM_ADPCM等等。 
        /// </summary>
        public ushort FormatTag = 1;
        /// <summary>
        /// 声道数目，1--单声道；2--双声道
        /// </summary>
        public ushort Channels = 2;
        /// <summary>
        /// 采样频率  一般有11025Hz（11kHz）、22050Hz（22kHz）和44100Hz（44kHz）三种
        /// </summary>
        public uint SamlesPerSec = 0;
        /// <summary>
        /// 每秒所需字节数
        /// </summary>
        public uint AvgBytesPerSec = 0;
        /// <summary>
        /// 数据块对齐单位(每个采样需要的字节数)
        /// </summary>
        public ushort BlockAlign = 0;
        /// <summary>
        /// 音频采样大小 
        /// </summary>
        public ushort BitsPerSample = 0;
        /// <summary>
        /// ???
        /// </summary>
        public byte[] Temp = new byte[2];
    }
    /// <summary>
    /// FACT
    /// </summary>
    public class Fact_Chunk
    {
        /// <summary>
        /// 文件前四个字节 为fact
        /// </summary>
        public byte[] ID = new byte[] { 0x66, 0x61, 0x63, 0x74 };   // 'f','a','c','t'
        /// <summary>
        /// 数据大小
        /// </summary>
        public uint Size = 0;
        /// <summary>
        /// 临时数据
        /// </summary>
        public byte[] Temp;
    }
    /// <summary>
    /// 数据区
    /// </summary>
    public class Data_Chunk
    {
        /// <summary>
        /// 文件前四个字节 为RIFF
        /// </summary>
        public byte[] ID = new byte[] { 0x64, 0x61, 0x74, 0x61 };   // 'd','a','t','a'
        /// <summary>
        /// 大小
        /// </summary>
        public uint Size = 0;
        /// <summary>
        /// 开始播放的位置
        /// </summary>
        public long FileBeginIndex = 0;
        /// <summary>
        /// 结束播放的位置
        /// </summary>
        public long FileOverIndex = 0;
    }
    #endregion
}
