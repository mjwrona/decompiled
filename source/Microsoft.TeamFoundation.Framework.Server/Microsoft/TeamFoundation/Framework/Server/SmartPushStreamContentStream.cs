// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartPushStreamContentStream
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SmartPushStreamContentStream : Stream
  {
    private readonly Stream m_baseStream;
    private readonly BufferedStream m_bufferStream;
    private readonly PositionedStreamWrapper m_positionedStream;
    private const int c_optimalBufferSize = 65536;

    public SmartPushStreamContentStream(Stream baseStream)
    {
      this.m_baseStream = baseStream;
      this.m_bufferStream = new BufferedStream(baseStream, 65536);
      this.m_positionedStream = new PositionedStreamWrapper((Stream) this.m_bufferStream);
    }

    public override void Flush()
    {
    }

    protected override void Dispose(bool disposing)
    {
      this.m_positionedStream?.Dispose();
      this.m_bufferStream?.Dispose();
      this.m_baseStream?.Dispose();
      base.Dispose(disposing);
    }

    public override long Seek(long offset, SeekOrigin origin) => this.m_positionedStream.Seek(offset, origin);

    public override int Read(byte[] buffer, int offset, int count) => this.m_positionedStream.Read(buffer, offset, count);

    public override void Write(byte[] buffer, int offset, int count) => this.m_positionedStream.Write(buffer, offset, count);

    public override void SetLength(long value) => this.m_positionedStream.SetLength(value);

    public override bool CanRead => this.m_positionedStream.CanRead;

    public override bool CanSeek => this.m_positionedStream.CanSeek;

    public override bool CanWrite => this.m_positionedStream.CanWrite;

    public override long Length => this.m_positionedStream.Length;

    public override long Position
    {
      get => this.m_positionedStream.Position;
      set => this.m_positionedStream.Position = value;
    }
  }
}
