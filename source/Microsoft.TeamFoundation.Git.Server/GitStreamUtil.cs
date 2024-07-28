// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitStreamUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitStreamUtil
  {
    public static readonly int StreamReaderDefaultBufferSize = 1024;
    public static readonly int StreamWriterDefaultBufferSize = 1024;
    public static readonly int OptimalBufferSize = 65536;
    public static readonly CompressionLevel OptimalCompressionLevel = CompressionLevel.Fastest;
    private static readonly Lazy<byte[]> s_zeroBuf = new Lazy<byte[]>((Func<byte[]>) (() => new byte[GitStreamUtil.OptimalBufferSize]), LazyThreadSafetyMode.None);

    internal static void EnsureDrained(Stream stream, byte[] buf)
    {
      if (buf.Length == 0 && stream.ReadByte() != -1)
        buf = new byte[GitStreamUtil.OptimalBufferSize];
      do
        ;
      while (stream.Read(buf, 0, buf.Length) > 0);
    }

    internal static void MakeStreamSeekable(
      IBufferStreamFactory bufferStreamFactory,
      ref Stream stream)
    {
      if (stream.CanSeek)
        return;
      long length = stream.Length;
      GitStreamUtil.ReplaceStream(ref stream, (Func<Stream>) (() => bufferStreamFactory.GetBufferStream(length)));
    }

    public static void ReadGreedy(Stream stream, byte[] buf, int offset, int count)
    {
      int num = GitStreamUtil.TryReadGreedy(stream, buf, offset, count);
      if (num > count)
        throw new EndOfStreamException();
      if (num < count)
        throw new IOException(string.Format("The amount of data read from the stream is less than expected. Read: {0}. Expected: {1}", (object) num, (object) count));
    }

    public static int TryReadGreedy(Stream stream, byte[] buf, int offset, int count)
    {
      int num1;
      int num2;
      for (num1 = 0; num1 < count; num1 += num2)
      {
        num2 = stream.Read(buf, offset + num1, count - num1);
        if (num2 == 0)
          break;
      }
      return num1;
    }

    public static void ReplaceStream(ref Stream stream, Func<Stream> createNewStream)
    {
      Stream destination = (Stream) null;
      try
      {
        destination = createNewStream();
        GitStreamUtil.SmartCopyTo(stream, destination);
        destination.Seek(0L, SeekOrigin.Begin);
        stream.Dispose();
        stream = destination;
        destination = (Stream) null;
      }
      finally
      {
        destination?.Dispose();
      }
    }

    public static void SafeForwardSeek(Stream stream, long offset, bool allowWrite)
    {
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 1L);
      if (stream.CanSeek)
      {
        stream.Seek(offset, SeekOrigin.Current);
      }
      else
      {
        if (stream.CanRead)
        {
          using (ByteArray byteArray = new ByteArray((int) Math.Min(offset, (long) GitStreamUtil.OptimalBufferSize)))
          {
            byte[] bytes = byteArray.Bytes;
            int num;
            for (; offset != 0L; offset -= (long) num)
            {
              num = stream.Read(bytes, 0, (int) Math.Min(offset, (long) bytes.Length));
              if (num == 0)
                break;
            }
          }
          if (offset == 0L)
            return;
        }
        if (!allowWrite || !stream.CanWrite)
          throw new EndOfStreamException();
        byte[] buffer = GitStreamUtil.s_zeroBuf.Value;
        int count;
        for (; offset != 0L; offset -= (long) count)
        {
          count = (int) Math.Min(offset, (long) buffer.Length);
          stream.Write(buffer, 0, count);
        }
      }
    }

    public static void SmartCopyTo(Stream source, Stream destination, bool trustStreamLength = false)
    {
      using (ByteArray byteArray = new ByteArray(GitStreamUtil.GetBufferSize(source, trustStreamLength)))
      {
        byte[] bytes = byteArray.Bytes;
        int count;
        while ((count = source.Read(bytes, 0, bytes.Length)) != 0)
          destination.Write(bytes, 0, count);
      }
    }

    public static int GetBufferSize(Stream source, bool trustStreamLength = false)
    {
      int bufferSize = GitStreamUtil.OptimalBufferSize;
      if (trustStreamLength || source.CanSeek)
      {
        try
        {
          long num = source.Length - source.Position;
          if (num < (long) bufferSize)
            bufferSize = (int) num;
        }
        catch (NotSupportedException ex)
        {
        }
      }
      return bufferSize;
    }

    public static ArraySegment<byte> GetBuffer(Stream stream, long length)
    {
      ArraySegment<byte> buffer;
      if (stream is MemoryStream memoryStream && memoryStream.TryGetBuffer(out buffer))
      {
        if (length + memoryStream.Position > memoryStream.Length)
          throw new EndOfStreamException("length is too large.");
        return new ArraySegment<byte>(buffer.Array, checked ((int) ((long) buffer.Offset + memoryStream.Position)), checked ((int) length));
      }
      byte[] numArray = new byte[length];
      GitStreamUtil.ReadGreedy(stream, numArray, 0, checked ((int) length));
      return new ArraySegment<byte>(numArray);
    }

    internal static T[] ReadArray<T>(Stream stream, int sizeOfValue, int numValues, byte[] buf) where T : struct
    {
      int num = checked (sizeOfValue * numValues);
      T[] dst = new T[numValues];
      int count1;
      for (int dstOffset = 0; dstOffset < num; dstOffset += count1)
      {
        int count2 = Math.Min(buf.Length, num - dstOffset);
        count1 = stream.Read(buf, 0, count2);
        if (count1 == 0)
          throw new EndOfStreamException();
        Buffer.BlockCopy((Array) buf, 0, (Array) dst, dstOffset, count1);
      }
      return dst;
    }

    public static void WriteArray<T>(
      Stream stream,
      T[] values,
      int sizeOfValue,
      int offset,
      int count,
      byte[] buf)
      where T : struct
    {
      int srcOffset = checked (sizeOfValue * offset);
      int num = checked (sizeOfValue * count + srcOffset);
      while (srcOffset < num)
      {
        int count1 = Math.Min(buf.Length, checked (num - srcOffset));
        Buffer.BlockCopy((Array) values, srcOffset, (Array) buf, 0, count1);
        stream.Write(buf, 0, count1);
        checked { srcOffset += count1; }
      }
    }
  }
}
