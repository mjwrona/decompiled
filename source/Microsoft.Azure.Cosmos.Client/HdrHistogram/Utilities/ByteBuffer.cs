// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.ByteBuffer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.IO;
using System.Net;

namespace HdrHistogram.Utilities
{
  internal sealed class ByteBuffer
  {
    private readonly byte[] _internalBuffer;

    public static ByteBuffer Allocate(int bufferCapacity) => new ByteBuffer(bufferCapacity);

    public static ByteBuffer Allocate(byte[] source)
    {
      ByteBuffer byteBuffer = new ByteBuffer(source.Length);
      Buffer.BlockCopy((Array) source, 0, (Array) byteBuffer._internalBuffer, byteBuffer.Position, source.Length);
      return byteBuffer;
    }

    private ByteBuffer(int bufferCapacity)
    {
      this._internalBuffer = new byte[bufferCapacity];
      this.Position = 0;
    }

    public int Position { get; set; }

    public int Capacity() => this._internalBuffer.Length;

    public int Remaining() => this.Capacity() - this.Position;

    public int ReadFrom(Stream source, int length) => source.Read(this._internalBuffer, this.Position, length);

    public byte Get() => this._internalBuffer[this.Position++];

    public short GetShort()
    {
      int networkOrder = (int) IPAddress.HostToNetworkOrder(BitConverter.ToInt16(this._internalBuffer, this.Position));
      this.Position += 2;
      return (short) networkOrder;
    }

    public int GetInt()
    {
      int networkOrder = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(this._internalBuffer, this.Position));
      this.Position += 4;
      return networkOrder;
    }

    public long GetLong()
    {
      long networkOrder = IPAddress.HostToNetworkOrder(BitConverter.ToInt64(this._internalBuffer, this.Position));
      this.Position += 8;
      return networkOrder;
    }

    public double GetDouble()
    {
      double num = ByteBuffer.Int64BitsToDouble(ByteBuffer.ToInt64(this._internalBuffer, this.Position));
      this.Position += 8;
      return num;
    }

    private static double Int64BitsToDouble(long value) => BitConverter.Int64BitsToDouble(value);

    private static long ToInt64(byte[] value, int startIndex) => ByteBuffer.CheckedFromBytes(value, startIndex, 8);

    private static long CheckedFromBytes(byte[] value, int startIndex, int bytesToConvert)
    {
      ByteBuffer.CheckByteArgument(value, startIndex, bytesToConvert);
      return ByteBuffer.FromBytes(value, startIndex, bytesToConvert);
    }

    private static void CheckByteArgument(byte[] value, int startIndex, int bytesRequired)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (startIndex < 0 || startIndex > value.Length - bytesRequired)
        throw new ArgumentOutOfRangeException(nameof (startIndex));
    }

    private static long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
    {
      long num = 0;
      for (int index = 0; index < bytesToConvert; ++index)
        num = num << 8 | (long) buffer[startIndex + index];
      return num;
    }

    public void Put(byte value) => this._internalBuffer[this.Position++] = value;

    public void PutInt(int value)
    {
      byte[] bytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(value));
      Array.Copy((Array) bytes, 0, (Array) this._internalBuffer, this.Position, bytes.Length);
      this.Position += bytes.Length;
    }

    public void PutInt(int index, int value)
    {
      byte[] bytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(value));
      Array.Copy((Array) bytes, 0, (Array) this._internalBuffer, index, bytes.Length);
    }

    public void PutLong(long value)
    {
      byte[] bytes = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(value));
      Array.Copy((Array) bytes, 0, (Array) this._internalBuffer, this.Position, bytes.Length);
      this.Position += bytes.Length;
    }

    public void PutDouble(double value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      Array.Reverse((Array) bytes);
      Array.Copy((Array) bytes, 0, (Array) this._internalBuffer, this.Position, bytes.Length);
      this.Position += bytes.Length;
    }

    internal byte[] ToArray()
    {
      byte[] destinationArray = new byte[this._internalBuffer.Length];
      Array.Copy((Array) this._internalBuffer, (Array) destinationArray, this._internalBuffer.Length);
      return destinationArray;
    }

    internal void BlockCopy(Array src, int srcOffset, int dstOffset, int count)
    {
      Buffer.BlockCopy(src, srcOffset, (Array) this._internalBuffer, dstOffset, count);
      this.Position += count;
    }

    internal void BlockGet(Array target, int targetOffset, int sourceOffset, int count) => Buffer.BlockCopy((Array) this._internalBuffer, sourceOffset, target, targetOffset, count);
  }
}
