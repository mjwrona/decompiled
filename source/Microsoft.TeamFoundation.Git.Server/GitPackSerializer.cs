// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitPackSerializer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Compression;
using Microsoft.TeamFoundation.Server.Core.Security;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Net;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitPackSerializer : IDisposable
  {
    public const int MaxPackObjectHeaderLength = 10;
    private readonly Stream m_rawStream;
    private readonly HashingStream<SHA1Cng2> m_stream;
    private readonly int m_objectCount;
    private readonly ByteArray m_buffer;
    private readonly byte[] m_tempObjectHeader;
    private bool m_completed;
    private bool m_disposed;
    private int m_objectsAdded;
    private static readonly byte[] s_packHeader = new byte[8]
    {
      (byte) 80,
      (byte) 65,
      (byte) 67,
      (byte) 75,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 2
    };
    public static readonly int PackHeaderLength = GitPackSerializer.s_packHeader.Length + 4;

    public GitPackSerializer(Stream stream, int objectCount, bool leaveOpen = false, int streamBufferSize = 4096)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ArgumentUtility.CheckForOutOfRange(objectCount, nameof (objectCount), 0);
      this.m_rawStream = stream;
      this.m_stream = new HashingStream<SHA1Cng2>();
      this.m_stream.Setup(this.m_rawStream, FileAccess.Write, leaveOpen: leaveOpen);
      this.m_objectCount = objectCount;
      this.m_buffer = new ByteArray(streamBufferSize);
      this.m_tempObjectHeader = new byte[10];
      this.WritePackHeader();
    }

    public void Dispose()
    {
      if (this.m_disposed)
        return;
      this.m_disposed = true;
      this.m_buffer.Dispose();
      this.m_stream.Dispose();
    }

    public void Complete()
    {
      if (this.m_objectCount != this.m_objectsAdded)
        throw new InvalidOperationException();
      byte[] hash = this.m_stream.Hash;
      this.m_rawStream.Write(hash, 0, hash.Length);
      this.m_completed = true;
    }

    public long AddRaw(Stream packEntry, bool leaveOpen = false)
    {
      ArgumentUtility.CheckForNull<Stream>(packEntry, nameof (packEntry));
      if (this.m_completed)
        throw new InvalidOperationException();
      long num = 0;
      int count;
      while ((count = packEntry.Read(this.m_buffer.Bytes, 0, this.m_buffer.SizeRequested)) != 0)
      {
        this.m_stream.Write(this.m_buffer.Bytes, 0, count);
        num += (long) count;
      }
      if (!leaveOpen)
        packEntry.Dispose();
      ++this.m_objectsAdded;
      return num;
    }

    public void AddMultipleRaw(
      Stream sourceStream,
      long offset,
      long length,
      int objectCount,
      bool leaveOpen = false)
    {
      ArgumentUtility.CheckForNull<Stream>(sourceStream, nameof (sourceStream));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0L);
      ArgumentUtility.CheckForOutOfRange(length, nameof (length), 1L);
      ArgumentUtility.CheckForOutOfRange(objectCount, nameof (objectCount), 0);
      if (this.m_completed)
        throw new InvalidOperationException();
      using (RestrictedStream restrictedStream = new RestrictedStream(sourceStream, offset, length, leaveOpen))
        restrictedStream.CopyTo((Stream) this.m_stream);
      this.m_objectsAdded += objectCount;
    }

    public void AddObject(TfsGitObject gitObj, out long deflatedSize)
    {
      ArgumentUtility.CheckForNull<TfsGitObject>(gitObj, nameof (gitObj));
      if (this.m_completed)
        throw new InvalidOperationException();
      GitPackObjectType packType = gitObj.ObjectType.GetPackType();
      using (Stream content = gitObj.GetContent())
        deflatedSize = this.AddInflatedStreamWithTypeAndSize(content, packType, content.Length);
    }

    public long AddInflatedStreamWithTypeAndSize(
      Stream sourceStream,
      GitPackObjectType objectType,
      long objectLength)
    {
      if (this.m_completed)
        throw new InvalidOperationException();
      using (LengthCountingStream forWrite = LengthCountingStream.CreateForWrite((Stream) this.m_stream, true))
      {
        this.WritePackObjectHeader((Stream) forWrite, objectType, objectLength);
        using (Stream stream = (Stream) new ZlibStream((Stream) forWrite, CompressionLevel.Fastest, true))
        {
          int count;
          while ((count = sourceStream.Read(this.m_buffer.Bytes, 0, this.m_buffer.SizeRequested)) != 0)
            stream.Write(this.m_buffer.Bytes, 0, count);
        }
        ++this.m_objectsAdded;
        return forWrite.Position;
      }
    }

    private void WritePackObjectHeader(Stream stream, GitPackObjectType type, long length)
    {
      int packObjectHeader = GitPackSerializer.CalculatePackObjectHeader(this.m_tempObjectHeader, type, length);
      stream.Write(this.m_tempObjectHeader, 0, packObjectHeader);
    }

    internal static int CalculatePackObjectHeader(
      byte[] buffer,
      GitPackObjectType type,
      long length)
    {
      int packObjectHeader = 1;
      buffer[0] = (byte) ((uint) type << 4);
      buffer[0] |= (byte) ((uint) (int) length & 15U);
      for (length >>= 4; length > 0L; length >>= 7)
      {
        buffer[packObjectHeader - 1] |= (byte) 128;
        buffer[packObjectHeader++] = (byte) ((uint) (int) length & (uint) sbyte.MaxValue);
      }
      return packObjectHeader;
    }

    internal static long CalculatePackSize(long chunkBodyLength) => chunkBodyLength + (long) GitPackSerializer.PackHeaderLength + 20L;

    private void WritePackHeader()
    {
      byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.m_objectCount));
      this.m_stream.Write(GitPackSerializer.s_packHeader, 0, GitPackSerializer.s_packHeader.Length);
      this.m_stream.Write(bytes, 0, bytes.Length);
    }
  }
}
