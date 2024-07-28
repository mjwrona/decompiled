// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Telemetry.TimeMeasuredStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Telemetry
{
  public class TimeMeasuredStream : Stream
  {
    private readonly Stream m_baseStream;
    private long m_elapsedTicks;
    private long m_bytes;

    public TimeMeasuredStream(Stream baseStream)
    {
      this.m_baseStream = baseStream ?? throw new ArgumentNullException(nameof (baseStream));
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool CanRead => this.m_baseStream.CanRead;

    public override bool CanSeek => this.m_baseStream.CanSeek;

    public override bool CanWrite => this.m_baseStream.CanWrite;

    public override long Length => this.m_baseStream.Length;

    public override long Position
    {
      get => this.m_baseStream.Position;
      set => this.m_baseStream.Position = value;
    }

    public long ElapsedTicks => this.m_elapsedTicks;

    public long Bytes => this.m_bytes;

    public override int Read(byte[] buffer, int offset, int count) => this.MeasureRead((Func<int>) (() => this.m_baseStream.Read(buffer, offset, count)));

    public override void Write(byte[] buffer, int offset, int count) => this.Measure((Action) (() => this.m_baseStream.Write(buffer, offset, count)), (long) count);

    public override void Flush() => this.Measure(new Action(this.m_baseStream.Flush));

    public override long Seek(long offset, SeekOrigin origin) => this.Measure<long>((Func<long>) (() => this.m_baseStream.Seek(offset, origin)));

    public override void SetLength(long value) => this.Measure((Action) (() => this.m_baseStream.SetLength(value)));

    public override void Close()
    {
      this.m_baseStream.Close();
      base.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.m_baseStream.Dispose();
      base.Dispose(disposing);
    }

    private void Measure(Action action, long bytes = 0)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      action();
      this.m_elapsedTicks += stopwatch.ElapsedTicks;
      this.m_bytes += bytes;
    }

    private int MeasureRead(Func<int> action)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      int num = action();
      this.m_elapsedTicks += stopwatch.ElapsedTicks;
      this.m_bytes += (long) num;
      return num;
    }

    private T Measure<T>(Func<T> action)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      T obj = action();
      this.m_elapsedTicks += stopwatch.ElapsedTicks;
      return obj;
    }
  }
}
