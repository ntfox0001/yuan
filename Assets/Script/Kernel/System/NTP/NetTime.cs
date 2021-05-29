using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetTime
{
    /// <summary>
    /// 返回毫秒
    /// </summary>
    public long LastRawTime
    {
        get
        {
            mWriteTimeMutex.WaitOne();
            long time = mLastRawTime;
            mWriteTimeMutex.ReleaseMutex();
            return time;
        }
    }
    public DateTime LastDateTime
    {
        get
        {
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(LastRawTime);
            return networkDateTime.ToLocalTime();
        }
    }
    public long LastTimeStamp
    {
        get
        {
            return DateTimeUtil.GetTimeStamp(LastDateTime);
        }
    }
    public string CurrentServer { get; private set; }
    Mutex mWriteTimeMutex = new Mutex();
    // NTP message size - 16 bytes of the digest (RFC 2030)
    byte[] gNtpData = new byte[48];
    long mLastRawTime = 0;
    int mTimeServerIndex = 0;
    public CustomYieldInstruction GetTime()
    {
        mLastRawTime = 0;

        //default Windows time server
        CurrentServer = NTPSystem.gNtpServers[mTimeServerIndex];
        mTimeServerIndex++;

        if (mTimeServerIndex >= NTPSystem.gNtpServers.Length)
        {
            mTimeServerIndex = 0;
        }

        //Setting the Leap Indicator, Version Number and Mode values
        gNtpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

        var addresses = Dns.GetHostEntry(CurrentServer).AddressList;

        //The UDP port number assigned to NTP is 123
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        //NTP uses UDP

        BeginConnect(ipEndPoint);

        return new WaitUntil(() => { return mLastRawTime != 0; });
    }
    void BeginConnect(IPEndPoint ipEndPoint)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.BeginConnect(ipEndPoint, ConnectCallback, socket);
    }
    void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket socket = (Socket)ar.AsyncState;
            // Complete the connection.  
            socket.EndConnect(ar);
            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.BeginSend(gNtpData, 0, gNtpData.Length, SocketFlags.None, SendCallback, socket);

        }
        catch (Exception e)
        {

        }  
    }

    void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket socket = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = socket.EndSend(ar);

            socket.BeginReceive(gNtpData, 0, gNtpData.Length, SocketFlags.None, ReceiveCallback, socket);
        }
        catch (Exception e)
        {

        }
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.  
            Socket socket = (Socket)ar.AsyncState;

            // Read data from the remote device.  
            int bytesRead = socket.EndReceive(ar);

            if (bytesRead > 0)
            {
                mWriteTimeMutex.WaitOne();
                mLastRawTime = ParseTime();
                mWriteTimeMutex.ReleaseMutex();
            }
        }
        catch (Exception e)
        {

        }
    }

    long ParseTime()
    {
        //Offset to get to the "Transmit Timestamp" field (time at which the reply 
        //departed the server for the client, in 64-bit timestamp format."
        const byte serverReplyTime = 40;

        //Get the seconds part
        long intPart = BitConverter.ToUInt32(gNtpData, serverReplyTime);

        //Get the seconds fraction
        long fractPart = BitConverter.ToUInt32(gNtpData, serverReplyTime + 4);

        //Convert From big-endian to little-endian
        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

        //**UTC** time
        //var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

        return milliseconds;
    }
    // stackoverflow.com/a/3294698/162671
    static uint SwapEndianness(long x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }
}
