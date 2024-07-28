// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.FileStreamWrapper
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class FileStreamWrapper : Stream
  {
    private Stream m_baseStream;
    private long m_offset;

    public FileStreamWrapper(Stream baseStream)
    {
      ArgumentUtility.CheckForNull<Stream>(baseStream, nameof (baseStream));
      this.m_baseStream = baseStream;
      this.m_offset = baseStream.Position;
    }

    public override bool CanRead => this.m_baseStream.CanRead;

    public override bool CanSeek => this.m_baseStream.CanSeek;

    public override bool CanWrite => false;

    public override long Length => this.m_baseStream.Length - this.m_offset;

    public override long Position
    {
      get => this.m_baseStream.Position - this.m_offset;
      set => this.m_baseStream.Position = value + this.m_offset;
    }

    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count) => this.m_baseStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin)
    {
      long num;
      if (origin == SeekOrigin.Begin)
      {
        num = this.m_baseStream.Seek(offset + this.m_offset, origin);
      }
      else
      {
        num = this.m_baseStream.Seek(offset, origin);
        if (num < this.m_offset)
          num = this.m_baseStream.Seek(this.m_offset, SeekOrigin.Begin);
      }
      return num - this.m_offset;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing)
        return;
      this.m_baseStream.Dispose();
    }
  }
}
