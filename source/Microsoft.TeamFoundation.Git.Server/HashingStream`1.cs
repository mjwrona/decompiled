// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.HashingStream`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class HashingStream<T> : Stream, IRewindableStream where T : HashAlgorithm, new()
  {
    private Stream m_stream;
    private FileAccess m_direction;
    private bool m_leaveOpen;
    private readonly HashAlgorithm m_hash;
    private CircularBufferLog m_log;
    private bool m_closed;
    private byte[] m_hashResult;

    public HashingStream() => this.m_hash = (HashAlgorithm) new T();

    public HashingStream(Stream stream, FileAccess direction, int rewindCapacity = 0, bool leaveOpen = false)
      : this()
    {
      this.Setup(stream, direction, rewindCapacity, leaveOpen);
    }

    protected override void Dispose(bool disposing)
    {
      if (this.m_hash != null)
        this.m_hash.Dispose();
      if (!this.m_leaveOpen && this.m_stream != null)
      {
        this.m_stream.Dispose();
        this.m_stream = (Stream) null;
      }
      base.Dispose(disposing);
    }

    public void PrepareForPool()
    {
      if (!this.m_hash.CanReuseTransform)
        throw new NotSupportedException("The hash must be resettable to pool the items!");
      if (!this.m_leaveOpen && this.m_stream != null)
        this.m_stream.Dispose();
      this.m_stream = (Stream) null;
    }

    public void Setup(Stream stream, FileAccess direction, int rewindCapacity = 0, bool leaveOpen = false)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ArgumentUtility.CheckForOutOfRange(rewindCapacity, nameof (rewindCapacity), 0);
      if (FileAccess.ReadWrite == direction)
        throw new ArgumentOutOfRangeException(nameof (direction));
      if (FileAccess.Read != direction && rewindCapacity > 0)
        throw new ArgumentOutOfRangeException(nameof (rewindCapacity));
      this.Reset();
      this.m_direction = direction;
      this.m_leaveOpen = leaveOpen;
      this.m_stream = stream;
      if (rewindCapacity > 0)
      {
        if (!stream.CanSeek)
        {
          if (!(stream is IRewindableStream rewindableStream) || rewindableStream.RewindCapacity <= 0)
            throw new InvalidOperationException();
          rewindCapacity = Math.Min(rewindableStream.RewindCapacity, rewindCapacity);
        }
        if (this.m_log == null || this.m_log.Capacity != rewindCapacity)
        {
          this.m_log = new CircularBufferLog(rewindCapacity);
          this.m_log.BytesDisplaced += new CircularBufferLog.BytesDisplacedEventHandler(this.m_log_BytesDisplaced);
        }
        else
          this.m_log.Reset();
      }
      else
        this.m_log = (CircularBufferLog) null;
    }

    public void Reset()
    {
      this.m_hashResult = (byte[]) null;
      this.m_hash.Initialize();
      this.m_closed = false;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.FinishHash();
        this.m_closed = true;
        if (!this.m_leaveOpen && this.m_stream != null)
          this.m_stream.Close();
      }
      base.Close();
    }

    public void AddToHash(byte[] buffer) => this.AddToHash(buffer, 0, buffer.Length);

    public void AddToHash(byte[] buffer, int offset, int count)
    {
      if (count <= 0)
        return;
      this.m_hash.TransformBlock(buffer, offset, count, (byte[]) null, 0);
    }

    public byte[] Hash
    {
      get
      {
        this.FinishHash();
        return this.m_hashResult;
      }
    }

    private void FinishHash()
    {
      if (this.m_hashResult != null)
        return;
      if (this.m_log != null)
        this.m_log.Flush();
      this.m_hash.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
      this.m_hashResult = this.m_hash.Hash;
    }

    public override bool CanRead => FileAccess.Read == this.m_direction;

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (FileAccess.Read != this.m_direction)
        throw new NotImplementedException();
      int num = this.m_stream.Read(buffer, offset, count);
      if (this.m_log == null)
      {
        if (num > 0)
          this.m_hash.TransformBlock(buffer, offset, num, (byte[]) null, 0);
      }
      else
        this.m_log.Write(buffer, offset, num);
      return num;
    }

    private void m_log_BytesDisplaced(object sender, byte[] buffer, int offset, int count)
    {
      if (count <= 0)
        return;
      this.m_hash.TransformBlock(buffer, offset, count, (byte[]) null, 0);
    }

    public int RewindCapacity => this.m_log != null ? this.m_log.Capacity : 0;

    public long Rewind(int positiveOffset)
    {
      if (this.m_log == null)
        throw new NotSupportedException();
      ArgumentUtility.CheckForOutOfRange(positiveOffset, nameof (positiveOffset), 0, this.m_log.Count);
      this.m_log.Erase(positiveOffset);
      if (this.m_stream.CanSeek)
        this.m_stream.Seek((long) -positiveOffset, SeekOrigin.Current);
      else
        ((IRewindableStream) this.m_stream).Rewind(positiveOffset);
      return this.Position;
    }

    public override long Length => this.m_stream.Length;

    public override long Position
    {
      get => this.m_stream.Position;
      set => throw new NotImplementedException();
    }

    public override bool CanWrite => FileAccess.Write == this.m_direction;

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (FileAccess.Write != this.m_direction)
        throw new NotImplementedException();
      if (count > 0)
        this.m_hash.TransformBlock(buffer, offset, count, (byte[]) null, 0);
      this.m_stream.Write(buffer, offset, count);
    }

    public override void Flush()
    {
      if (FileAccess.Write != this.m_direction)
        throw new NotImplementedException();
      this.m_stream.Flush();
    }

    public override bool CanSeek => false;

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotSupportedException();
  }
}
