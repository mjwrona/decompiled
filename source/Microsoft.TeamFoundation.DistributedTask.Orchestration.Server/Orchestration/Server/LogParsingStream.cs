// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.LogParsingStream
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class LogParsingStream : Stream
  {
    private Stream m_stream;
    private long m_lineCount;
    private Decoder m_decoder;
    private char[] m_charBuffer;
    private Encoding m_encoding;
    private bool m_initialized;
    private bool m_pendingCarriageReturn;

    public LogParsingStream(Stream stream, Encoding encoding)
    {
      this.m_stream = stream;
      this.m_encoding = encoding;
      this.m_decoder = encoding.GetDecoder();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public long LineCount => this.m_lineCount;

    public override void Flush() => this.m_stream.Flush();

    public override long Length => this.m_stream.Length;

    public override long Position
    {
      get => this.m_stream.Position;
      set
      {
        this.m_stream.Position = value == 0L ? value : throw new NotSupportedException();
        this.m_initialized = false;
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int byteCount = this.m_stream.Read(buffer, offset, count);
      if (byteCount > 0)
      {
        if (!this.m_initialized)
        {
          this.m_initialized = true;
          this.m_lineCount = 1L;
        }
        int maxCharCount = this.m_encoding.GetMaxCharCount(count);
        if (this.m_charBuffer == null || this.m_charBuffer.Length < maxCharCount)
          this.m_charBuffer = new char[maxCharCount];
        int chars = this.m_decoder.GetChars(buffer, offset, byteCount, this.m_charBuffer, 0);
        if (chars == 0)
          return byteCount;
        int index = 0;
        if (this.m_pendingCarriageReturn)
        {
          if (this.m_charBuffer[0] == '\n')
            ++index;
          ++this.m_lineCount;
          this.m_pendingCarriageReturn = false;
        }
        for (; index < chars; ++index)
        {
          switch (this.m_charBuffer[index])
          {
            case '\n':
              ++this.m_lineCount;
              break;
            case '\r':
              if (index == chars - 1)
              {
                this.m_pendingCarriageReturn = true;
                break;
              }
              if (this.m_charBuffer[index + 1] == '\n')
                ++index;
              ++this.m_lineCount;
              break;
          }
        }
      }
      return byteCount;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
  }
}
