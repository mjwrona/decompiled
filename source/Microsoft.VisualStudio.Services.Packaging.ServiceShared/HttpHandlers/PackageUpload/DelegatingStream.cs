// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload.DelegatingStream
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload
{
  public abstract class DelegatingStream : Stream
  {
    private readonly bool leaveOpen;

    protected DelegatingStream(Stream stream, bool leaveOpen)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      this.BaseStream = stream;
      this.leaveOpen = leaveOpen;
    }

    public override bool CanRead => this.BaseStream.CanRead;

    public override bool CanSeek => this.BaseStream.CanSeek;

    public override bool CanTimeout => this.BaseStream.CanTimeout;

    public override bool CanWrite => this.BaseStream.CanWrite;

    public override long Length => this.BaseStream.Length;

    public override long Position
    {
      get => this.BaseStream.Position;
      set => this.BaseStream.Position = value;
    }

    public override int ReadTimeout
    {
      get => this.BaseStream.ReadTimeout;
      set => this.BaseStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.BaseStream.WriteTimeout;
      set => this.BaseStream.WriteTimeout = value;
    }

    protected Stream BaseStream { get; }

    public override void Close()
    {
      if (this.leaveOpen)
        return;
      this.BaseStream.Close();
    }

    public override void Flush() => this.BaseStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => this.BaseStream.Read(buffer, offset, count);

    public override int ReadByte() => this.BaseStream.ReadByte();

    public override long Seek(long offset, SeekOrigin origin) => this.BaseStream.Seek(offset, origin);

    public override void SetLength(long value) => this.BaseStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => this.BaseStream.Write(buffer, offset, count);

    public override void WriteByte(byte value) => this.BaseStream.WriteByte(value);
  }
}
