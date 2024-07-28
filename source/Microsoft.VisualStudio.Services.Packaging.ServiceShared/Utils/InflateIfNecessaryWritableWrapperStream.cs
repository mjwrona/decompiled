// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.InflateIfNecessaryWritableWrapperStream
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  internal class InflateIfNecessaryWritableWrapperStream : Stream
  {
    private readonly Stream target;
    private Stream writingStream;
    private WritableInflateStream inflateStream;
    private readonly Func<bool> shouldInflateFunc;

    public bool? DetectedZLibHeader { get; private set; }

    public bool? ShouldInflateResult { get; private set; }

    public InflateIfNecessaryWritableWrapperStream(Stream target, Func<bool> shouldInflateIf)
    {
      this.target = target ?? throw new ArgumentNullException(nameof (target));
      this.shouldInflateFunc = shouldInflateIf;
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (count == 0)
        return;
      this.EnsureWritingStreamSet(buffer, offset, count);
      this.writingStream.Write(buffer, offset, count);
    }

    public override async Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      if (count == 0)
        return;
      this.EnsureWritingStreamSet(buffer, offset, count);
      await this.writingStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    private void EnsureWritingStreamSet(byte[] buffer, int offset, int count)
    {
      if (this.writingStream != null)
        return;
      byte num1 = buffer[offset];
      if (num1 == (byte) 120)
      {
        if (count > 1)
        {
          byte num2 = buffer[offset + 1];
          this.DetectedZLibHeader = new bool?(((int) num2 >> 5 & 1) == 0 && ((int) num1 * 256 + (int) num2) % 31 == 0);
        }
        else
          this.DetectedZLibHeader = new bool?(true);
      }
      else
        this.DetectedZLibHeader = new bool?(false);
      this.ShouldInflateResult = new bool?(this.shouldInflateFunc());
      bool? shouldInflateResult1 = this.ShouldInflateResult;
      bool flag1 = true;
      if (shouldInflateResult1.GetValueOrDefault() == flag1 & shouldInflateResult1.HasValue)
      {
        bool? detectedZlibHeader = this.DetectedZLibHeader;
        bool flag2 = false;
        if (detectedZlibHeader.GetValueOrDefault() == flag2 & detectedZlibHeader.HasValue)
          throw new InvalidOperationException(Resources.Error_ShouldInflateButNoZlibHeader());
      }
      bool? shouldInflateResult2 = this.ShouldInflateResult;
      bool flag3 = true;
      if (!(shouldInflateResult2.GetValueOrDefault() == flag3 & shouldInflateResult2.HasValue))
      {
        bool? detectedZlibHeader = this.DetectedZLibHeader;
        bool flag4 = true;
        if (!(detectedZlibHeader.GetValueOrDefault() == flag4 & detectedZlibHeader.HasValue))
        {
          this.writingStream = this.target;
          return;
        }
      }
      this.inflateStream = new WritableInflateStream(this.target);
      this.writingStream = (Stream) this.inflateStream;
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

    public override void Flush()
    {
      if (this.writingStream != null)
        this.writingStream.Flush();
      else
        this.target.Flush();
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
      if (this.writingStream != null)
        await this.writingStream.FlushAsync(cancellationToken);
      else
        await this.target.FlushAsync(cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
      this.inflateStream?.Dispose();
      base.Dispose(disposing);
    }

    public override long Length => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }
  }
}
