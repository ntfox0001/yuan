using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// only support single reader and single writer
class RWLockStream : Stream
{
    public class RWLockNode
    {
        byte[] mBuffer;

        int mReadPosition = 0;    //读取位置
        int mWritePosition = 0;    //写入位置

        public RWLockNode(int size)
        {
            mBuffer = new byte[size];
        }
        public void Reset()
        {
            mReadPosition = 0;
            mWritePosition = 0;
        }
        public int Write(byte[] buff, int offset, int len)
        {
            int availableWriteLen = mBuffer.Length - mWritePosition - 1;
            int writeLen = availableWriteLen > len ? len : availableWriteLen;
            Array.Copy(buff, offset, mBuffer, mWritePosition, writeLen);

            mWritePosition += writeLen;
            return writeLen;
        }
        public int Read(byte[] buff, int offset, int len)
        {
            int availableReadLen = mWritePosition - mReadPosition;
            if (availableReadLen == 0) return 0;
            int readLen = availableReadLen > len ? len : availableReadLen;
            Array.Copy(mBuffer, mReadPosition, buff, offset, readLen);

            mReadPosition += readLen;
            return readLen;
        }
        public byte[] Buffer { get { return  mBuffer; } }
        public int Offset { get { return mReadPosition; } }
        public int Length { get { return  mWritePosition; } }
    }
    List<RWLockNode> mBufferList = new List<RWLockNode>();
    List<RWLockNode> mCacheList = new List<RWLockNode>();

    int mLength = 0;
    int mCacheCount = 10;
    int mNodeSize = 256;

    object mLockObject = new object();
    string mName;

    public RWLockStream(int nodeSize, int cacheCount)
    {
        mNodeSize = nodeSize;
        mCacheCount = cacheCount;
    }
    public RWLockStream(String name) {
        mName = name;

    }

    public override bool CanRead
    {
        get { return true; }
    }
    public override bool CanSeek { get { return false; } }

    public override bool CanWrite { get { return true; } }

    public override long Length { get { return mLength; } }
    public override long Position { get { return 0; } set { } }
    public string Name { get { return mName; } set { mName = value; } }

    public override void Flush()
    {
        //throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int readSize = 0;
        lock (mLockObject)
        {
            readSize = Math.Min(count, mLength);
        }
                    
        if (readSize == 0) return 0;
        int rtSize = readSize;
        int removeItemCount = 0;
        while (readSize > 0)
        {
            int rnSize = mBufferList[removeItemCount].Read(buffer, offset, readSize);
            readSize -= rnSize;
            offset += rnSize;

            if (readSize > 0)
            {
                removeItemCount++;
            }
        }
        lock(mLockObject)
        {
            for (int i = 0; i < removeItemCount; i++)
            {
                DiscardNode(mBufferList[0]);
                mBufferList.RemoveAt(0);
            }
            mLength = mLength - rtSize;
        }
                
        return rtSize;
            

    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        int writeSize = count;
        List<RWLockNode> writeNodeList = new List<RWLockNode>();
        while (writeSize > 0)
        {
            RWLockNode node = GainNode();
            int nwSize = node.Write(buffer, offset, writeSize);
            writeNodeList.Add(node);

            offset += nwSize;
            writeSize -= nwSize;
        }

        lock(mLockObject)
        {
            foreach (RWLockNode n in writeNodeList)
            {
                mBufferList.Add(n);
            }
            mLength = mLength + count;
        }
    }
    RWLockNode GainNode()
    {
        if (mCacheList.Count > 0)
        {
            RWLockNode node = mCacheList[0];
            mCacheList.RemoveAt(0);
            node.Reset();
            return node;
        }
        else
        {
            RWLockNode node = new RWLockNode(mNodeSize);
            return node;
        }
    }
    void DiscardNode(RWLockNode node)
    {
        if (mCacheList.Count >= mCacheCount)
        {
            node = null;
        }
        else
        {
            mCacheList.Add(node);
        }
    }
}
