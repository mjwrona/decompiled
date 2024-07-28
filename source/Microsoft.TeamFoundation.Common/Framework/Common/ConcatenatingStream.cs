// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ConcatenatingStream
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
  public class ConcatenatingStream : Stream
  {
    private readonly Stream m_input1;
    private readonly Stream m_input2;
    private readonly bool m_leaveInput1Open;
    private readonly bool m_leaveInput2Open;
    private bool m_closed;
    private bool m_drainedInput1;
    private long m_position;

    public ConcatenatingStream(
      Stream input1,
      Stream input2,
      bool leaveInput1Open = false,
      bool leaveInput2Open = false)
    {
      ArgumentUtility.CheckForNull<Stream>(input1, nameof (input1));
      ArgumentUtility.CheckForNull<Stream>(input2, nameof (input2));
      this.m_input1 = input1;
      this.m_input2 = input2;
      this.m_leaveInput1Open = leaveInput1Open;
      this.m_leaveInput2Open = leaveInput2Open;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        if (!this.m_leaveInput1Open)
          this.m_input1.Close();
        if (!this.m_leaveInput2Open)
          this.m_input2.Close();
      }
      base.Close();
    }

    public override bool CanRead => true;

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num = 0;
      if (!this.m_drainedInput1)
      {
        num = this.m_input1.Read(buffer, offset, count);
        if (num == 0 && count > 0)
          this.m_drainedInput1 = true;
      }
      if (num < count)
        num += this.m_input2.Read(buffer, offset + num, count - num);
      this.m_position += (long) num;
      return num;
    }

    public override long Length => this.m_input1.Length + this.m_input2.Length;

    public override bool CanSeek => this.m_input1.CanSeek && this.m_input2.CanSeek;

    public override long Position
    {
      get => this.m_position;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          if (offset < this.m_input1.Length)
          {
            this.m_drainedInput1 = false;
            this.m_input1.Seek(offset, SeekOrigin.Begin);
            this.m_input2.Seek(0L, SeekOrigin.Begin);
          }
          else
          {
            this.m_drainedInput1 = true;
            this.m_input1.Seek(0L, SeekOrigin.End);
            this.m_input2.Seek(offset - this.m_input1.Length, SeekOrigin.Begin);
          }
          this.m_position = Math.Min(offset, this.Length);
          break;
        case SeekOrigin.Current:
          return this.Seek(this.Position + offset, SeekOrigin.Begin);
        case SeekOrigin.End:
          return this.Seek(this.Length - offset, SeekOrigin.Begin);
      }
      return this.m_position;
    }

    public override bool CanWrite => false;

    public override void Flush() => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
  }
}
