// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.RestrictedStream
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class RestrictedStream : Stream
  {
    private readonly Stream m_stream;
    private readonly long m_length;
    private readonly bool m_leaveOpen;
    private long m_position;
    private bool m_closed;

    public RestrictedStream(Stream stream, long offset, long length, bool leaveOpen = false)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0L);
      ArgumentUtility.CheckForOutOfRange(length, nameof (length), 0L);
      this.m_stream = stream;
      this.m_length = length;
      this.m_leaveOpen = leaveOpen;
      if (offset == 0L)
        return;
      if (!this.m_stream.CanSeek)
        throw new InvalidOperationException();
      this.m_stream.Seek(offset, SeekOrigin.Current);
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        if (!this.m_leaveOpen)
          this.m_stream.Close();
      }
      base.Close();
    }

    public override bool CanRead => true;

    public override bool CanSeek => this.m_stream.CanSeek;

    public override bool CanWrite => false;

    public override long Length => this.m_length;

    public override long Position
    {
      get => this.m_position;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
      int count1 = (int) (Math.Min(this.m_position + (long) count, this.m_length) - this.m_position);
      if (count1 == 0)
        return 0;
      int num = this.m_stream.Read(buffer, offset, count1);
      this.m_position += (long) num;
      return num;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (!this.m_stream.CanSeek)
        throw new InvalidOperationException();
      long val2;
      switch (origin)
      {
        case SeekOrigin.Begin:
          val2 = offset;
          break;
        case SeekOrigin.Current:
          val2 = this.m_position + offset;
          break;
        case SeekOrigin.End:
          val2 = this.m_length + offset;
          break;
        default:
          throw new InvalidOperationException();
      }
      long num = Math.Max(Math.Min(this.m_length, val2), 0L);
      this.m_stream.Seek(num - this.m_position, SeekOrigin.Current);
      this.m_position = num;
      return num;
    }

    public override void Flush() => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
  }
}
