// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PositionedStreamWrapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class PositionedStreamWrapper : Stream
  {
    private readonly Stream m_baseStream;
    private long m_currentPosition;

    public PositionedStreamWrapper(Stream baseStream) => this.m_baseStream = baseStream;

    public override long Position
    {
      get => this.m_currentPosition;
      set => throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num = this.m_baseStream.Read(buffer, offset, count);
      this.m_currentPosition += (long) num;
      return num;
    }

    public override int ReadByte()
    {
      int num = this.m_baseStream.ReadByte();
      ++this.m_currentPosition;
      return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.m_baseStream.Write(buffer, offset, count);
      this.m_currentPosition += (long) count;
    }

    public override void WriteByte(byte value)
    {
      this.m_baseStream.WriteByte(value);
      ++this.m_currentPosition;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long num = this.m_baseStream.Seek(offset, origin);
      this.m_currentPosition = num;
      return num;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.m_baseStream.Dispose();
      base.Dispose(disposing);
    }

    public override bool CanRead => this.m_baseStream.CanRead;

    public override bool CanSeek => this.m_baseStream.CanSeek;

    public override bool CanWrite => this.m_baseStream.CanWrite;

    public override void Flush() => this.m_baseStream.Flush();

    public override long Length => this.m_baseStream.Length;

    public override void SetLength(long value) => this.m_baseStream.SetLength(value);
  }
}
