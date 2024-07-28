// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload.ProgressStream
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload
{
  public abstract class ProgressStream : DelegatingStream
  {
    private readonly IStopwatch stopWatch;
    private readonly ITimeProvider timeProvider;
    private readonly long bytesInterval;
    private readonly List<TransferEntry> entries;
    private long currentIntervalBytes;
    private long lastTicks;
    private bool started;
    private DateTime startTime;

    protected ProgressStream(
      Stream stream,
      bool leaveOpen,
      IStopwatch stopWatch,
      ITimeProvider timeProvider,
      long bytesInterval)
      : base(stream, leaveOpen)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ArgumentUtility.CheckForNull<IStopwatch>(stopWatch, nameof (stopWatch));
      ArgumentUtility.CheckForNull<ITimeProvider>(timeProvider, nameof (timeProvider));
      this.stopWatch = stopWatch;
      this.timeProvider = timeProvider;
      this.bytesInterval = bytesInterval;
      this.currentIntervalBytes = 0L;
      this.lastTicks = 0L;
      this.started = false;
      this.entries = new List<TransferEntry>();
    }

    public DateTime GetStartTime() => this.startTime;

    public void RecordProgressEntry()
    {
      long num1 = Math.Max(this.stopWatch.Elapsed.Ticks, 0L);
      long num2 = Math.Max(num1 - this.lastTicks, 0L);
      this.lastTicks = num1;
      this.entries.Add(new TransferEntry(this.currentIntervalBytes, TimeSpan.FromTicks(num2)));
      this.currentIntervalBytes = 0L;
    }

    public IList<TransferEntry> GetProgressEntries() => (IList<TransferEntry>) this.entries;

    public TransferRateResults GetTransferRateResults() => new TransferRateResults(this.GetStartTime(), this.GetProgressEntries());

    protected int PerformWithTracking(Func<int> performer)
    {
      if (!this.started)
      {
        this.stopWatch.Start();
        this.startTime = this.timeProvider.Now;
        this.started = true;
      }
      int num = performer();
      this.currentIntervalBytes += (long) num;
      if (this.currentIntervalBytes >= this.bytesInterval)
        this.RecordProgressEntry();
      return num;
    }
  }
}
