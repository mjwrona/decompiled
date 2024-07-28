// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RedirectingStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class RedirectingStream : Stream
  {
    private readonly Stream m_input;
    private readonly Stream m_output;
    private readonly bool m_leaveInputOpen;
    private readonly bool m_leaveOutputOpen;
    private bool m_closed;

    public RedirectingStream(
      Stream input,
      Stream output,
      bool leaveInputOpen = false,
      bool leaveOutputOpen = false)
    {
      this.m_input = input;
      this.m_output = output;
      this.m_leaveInputOpen = leaveInputOpen;
      this.m_leaveOutputOpen = leaveOutputOpen;
    }

    public override void Close()
    {
      if (!this.m_closed)
      {
        this.m_closed = true;
        if (!this.m_leaveInputOpen)
          this.m_input.Close();
        if (!this.m_leaveOutputOpen)
          this.m_output.Close();
      }
      base.Close();
    }

    public override bool CanRead => true;

    public override long Length => this.m_input.Length;

    public override long Position
    {
      get => this.m_input.Position;
      set => throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int count1 = this.m_input.Read(buffer, offset, count);
      if (count1 > 0)
        this.m_output.Write(buffer, offset, count1);
      return count1;
    }

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override void Flush() => throw new NotImplementedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
  }
}
