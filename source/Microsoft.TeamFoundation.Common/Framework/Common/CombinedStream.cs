// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CombinedStream
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public class CombinedStream : Stream
  {
    private Stream[] m_subStreams;
    private bool m_canSeek;
    private bool m_canRead;
    private long[] m_subLengths;
    private long m_length;
    private long m_position;
    private int m_idx;
    private bool m_disposed;
    private byte[] m_buffer;
    private static int s_buffSize = 1048576;

    public CombinedStream(params Stream[] subStreams)
    {
      if (subStreams == null)
        throw new ArgumentNullException(nameof (subStreams));
      this.Init(subStreams);
    }

    public CombinedStream(IEnumerable<Stream> subStreams)
    {
      if (subStreams == null)
        throw new ArgumentNullException(nameof (subStreams));
      this.Init(subStreams.ToArray<Stream>());
    }

    public override bool CanRead => this.m_canRead;

    public override bool CanSeek => this.m_canSeek;

    public override bool CanWrite => false;

    public override long Length
    {
      get
      {
        if (this.CanSeek && this.m_length == 0L)
          this.m_length = ((IEnumerable<Stream>) this.m_subStreams).Select<Stream, long>((Func<Stream, long>) (s => s.Length)).Sum();
        return this.m_length;
      }
    }

    public override long Position
    {
      get => this.m_position;
      set
      {
        if (!this.CanSeek)
          throw new NotSupportedException();
        this.Seek(value, SeekOrigin.Begin);
      }
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (!this.CanSeek)
        throw new NotSupportedException();
      if (origin == SeekOrigin.Begin && 0L <= offset && offset <= this.Length)
      {
        this.m_position = 0L;
        for (int idx = 0; idx < this.m_subStreams.Length; ++idx)
        {
          long num = this.SubLength(idx);
          if (offset <= num)
          {
            this.m_position += this.m_subStreams[idx].Seek(offset, origin);
            this.m_idx = idx;
            break;
          }
          offset -= num;
          this.m_position += num;
        }
      }
      else if (origin == SeekOrigin.End && 0L >= offset && offset >= -this.Length)
      {
        this.m_position = this.Length;
        for (int idx = this.m_subStreams.Length - 1; idx > -1; --idx)
        {
          long num = this.SubLength(idx);
          this.m_position -= num;
          if (-offset <= num)
          {
            this.m_position += this.m_subStreams[idx].Seek(offset, origin);
            this.m_idx = idx;
            break;
          }
          offset += num;
        }
      }
      else
      {
        if (origin == SeekOrigin.Current && offset + this.Position >= 0L && offset + this.Position <= this.Length)
          return this.Seek(this.Position + offset, SeekOrigin.Begin);
        throw new ArgumentException();
      }
      for (int index = this.m_idx + 1; index < this.m_subStreams.Length; ++index)
        this.m_subStreams[index].Seek(0L, SeekOrigin.Begin);
      return this.m_position;
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (buffer == null)
        throw new ArgumentNullException(nameof (buffer));
      if (offset < 0 || offset >= buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0 || offset + count > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (count));
      int num1 = 0;
      for (int idx = this.m_idx; idx < this.m_subStreams.Length; ++idx)
      {
        int num2 = this.m_subStreams[this.m_idx].Read(buffer, offset, count);
        num1 += num2;
        offset += num2;
        count -= num2;
        this.m_position += (long) num2;
        if (count == 0 || ++this.m_idx == this.m_subStreams.Length)
          break;
      }
      return num1;
    }

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("Combined streams are not writable");

    protected override void Dispose(bool disposing)
    {
      if (this.m_disposed)
        return;
      if (disposing)
      {
        foreach (Stream subStream in this.m_subStreams)
          subStream.Dispose();
      }
      this.m_disposed = true;
      this.Dispose();
    }

    internal static void SetBufferSize(int buffSize) => CombinedStream.s_buffSize = buffSize;

    private void Init(Stream[] streams)
    {
      if (streams.Length == 0 || ((IEnumerable<Stream>) streams).Any<Stream>((Func<Stream, bool>) (s => s == null)) || !((IEnumerable<Stream>) streams).All<Stream>((Func<Stream, bool>) (s => s.CanRead)))
        throw new ArgumentException("The sub-stream collection is either empty or contains unreadable files");
      this.m_subStreams = streams;
      this.MakeSeekable();
      this.m_canRead = !((IEnumerable<Stream>) this.m_subStreams).Any<Stream>((Func<Stream, bool>) (s => !s.CanRead));
      this.m_canSeek = !((IEnumerable<Stream>) this.m_subStreams).Any<Stream>((Func<Stream, bool>) (s => !s.CanSeek));
      this.m_subLengths = new long[this.m_subStreams.Length];
      this.m_length = 0L;
      this.m_position = 0L;
      this.m_idx = 0;
    }

    private void MakeSeekable()
    {
      for (int index = 0; index < this.m_subStreams.Length; ++index)
      {
        Stream subStream = this.m_subStreams[index];
        if (subStream == null)
          this.m_subStreams[index] = (Stream) new MemoryStream();
        else if (subStream.CanRead && (!subStream.CanSeek || this.IsDuplicate(subStream, index - 1)))
        {
          byte[] buffer = this.GetBuffer();
          int num = subStream.Read(buffer, 0, CombinedStream.s_buffSize);
          Stream stream;
          if (num < CombinedStream.s_buffSize)
          {
            stream = (Stream) new MemoryStream(num);
            stream.Write(buffer, 0, num);
          }
          else
          {
            stream = (Stream) File.Create(Path.Combine(FileSpec.GetTempDirectory(), Guid.NewGuid().ToString("N")), CombinedStream.s_buffSize, FileOptions.DeleteOnClose);
            do
            {
              stream.Write(buffer, 0, num);
              num = subStream.Read(buffer, 0, CombinedStream.s_buffSize);
            }
            while (num > 0);
          }
          this.m_subStreams[index] = stream;
        }
        this.m_subStreams[index].Seek(0L, SeekOrigin.Begin);
      }
    }

    private bool IsDuplicate(Stream stream, int n)
    {
      for (int index = 0; index < n; ++index)
      {
        if (stream == this.m_subStreams[index])
          return true;
      }
      return false;
    }

    private long SubLength(int idx)
    {
      if (this.CanSeek && this.m_subLengths[idx] == 0L)
        this.m_subLengths[idx] = this.m_subStreams[idx].Length;
      return this.m_subLengths[idx];
    }

    private byte[] GetBuffer()
    {
      if (this.m_buffer == null)
        this.m_buffer = new byte[CombinedStream.s_buffSize];
      return this.m_buffer;
    }
  }
}
