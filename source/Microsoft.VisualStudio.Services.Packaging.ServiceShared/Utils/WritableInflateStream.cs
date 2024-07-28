// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.WritableInflateStream
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  internal class WritableInflateStream : Stream
  {
    private readonly Stream target;
    private readonly Inflater inflater = new Inflater();
    private readonly byte[] inflateBuffer;

    public WritableInflateStream(Stream target, int bufferSize = 32768)
    {
      this.target = target ?? throw new ArgumentNullException(nameof (target));
      this.inflateBuffer = new byte[bufferSize];
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.CheckInflaterStateBeforeWrite();
      this.inflater.SetInput(buffer, offset, count);
      while (true)
      {
        int count1 = this.inflater.Inflate(this.inflateBuffer);
        if (count1 > 0)
          this.target.Write(this.inflateBuffer, 0, count1);
        else
          break;
      }
      this.CheckInflaterStateAfterWrite();
    }

    public override async Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.CheckInflaterStateBeforeWrite();
      this.inflater.SetInput(buffer, offset, count);
      while (true)
      {
        int count1 = this.inflater.Inflate(this.inflateBuffer);
        if (count1 > 0)
          await this.target.WriteAsync(this.inflateBuffer, 0, count1, cancellationToken);
        else
          break;
      }
      this.CheckInflaterStateAfterWrite();
    }

    private void CheckInflaterStateBeforeWrite()
    {
      if (!this.inflater.IsNeedingInput)
        throw new InvalidOperationException("Because it always consumes its entire input, the Inflater should always need input when Write is called.");
      if (this.inflater.IsFinished)
        throw new InvalidOperationException("Inflater is already finished");
      this.CheckInflaterDoesNotNeedDictionary();
    }

    private void CheckInflaterDoesNotNeedDictionary()
    {
      if (this.inflater.IsNeedingDictionary)
        throw new InvalidOperationException("Inflater needs an initial dictionary. This is unexpected because we don't write deflated content using an initial dictionary.");
    }

    private void CheckInflaterStateAfterWrite()
    {
      if (!this.inflater.IsNeedingInput && !this.inflater.IsFinished)
        throw new InvalidOperationException("Write should have consumed all input, so Inflater should either need more input or be finished.");
      this.CheckInflaterDoesNotNeedDictionary();
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      throw new NotSupportedException();
    }

    public override void Flush() => this.target.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => this.target.FlushAsync(cancellationToken);

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
  }
}
