// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TimeBoxedStream
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TimeBoxedStream : Stream
  {
    private TimeSpan _timeout;
    private CancellationTokenSource _cancellationTokenSource;

    public TimeBoxedStream(Stream stream) => this.Stream = stream;

    public Stream Stream { get; }

    public override bool CanRead
    {
      get
      {
        this.CheckTimer();
        return this.Stream.CanRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        this.CheckTimer();
        return this.Stream.CanSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        this.CheckTimer();
        return this.Stream.CanWrite;
      }
    }

    public override long Length
    {
      get
      {
        this.CheckTimer();
        return this.Stream.Length;
      }
    }

    public override long Position
    {
      get
      {
        this.CheckTimer();
        return this.Stream.Position;
      }
      set
      {
        this.CheckTimer();
        this.Stream.Position = value;
      }
    }

    public void SetTimer(TimeSpan timeout)
    {
      this._timeout = timeout.TotalMilliseconds > 0.0 && timeout.TotalMilliseconds <= (double) int.MaxValue ? timeout : throw new ArgumentOutOfRangeException("Timeout in milliseconds must be > 0 but < int.MaxValue");
      this._cancellationTokenSource?.Dispose();
      this._cancellationTokenSource = new CancellationTokenSource(timeout);
    }

    public void RemoveTimer() => this._cancellationTokenSource?.Dispose();

    private void CheckTimer()
    {
      if (this.Stream == null)
        throw new InvalidOperationException("The inner stream should be first initialized");
      if (this._cancellationTokenSource != null && this._cancellationTokenSource.IsCancellationRequested)
        throw new TimeoutException("Unable to complete the stream operation in the alloted time (" + this._timeout.ToString("c") + ")");
    }

    protected override void Dispose(bool disposing)
    {
      this._cancellationTokenSource?.Dispose();
      this.Stream?.Dispose();
      base.Dispose(disposing);
    }

    public override void Flush()
    {
      this.CheckTimer();
      this.Stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.CheckTimer();
      return this.Stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.CheckTimer();
      return this.Stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      this.CheckTimer();
      this.Stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.CheckTimer();
      this.Stream.Write(buffer, offset, count);
    }
  }
}
