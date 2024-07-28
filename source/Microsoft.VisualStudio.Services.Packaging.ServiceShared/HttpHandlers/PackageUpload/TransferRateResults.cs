// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload.TransferRateResults
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload
{
  [DataContract]
  public class TransferRateResults
  {
    private TransferRateResults(TransferRateResults orig, bool includeEntries)
    {
      this.StartTime = orig.StartTime;
      this.EndTime = orig.EndTime;
      this.TotalTime = orig.TotalTime;
      this.TotalBytes = orig.TotalBytes;
      this.AvgBytesPerSecond = orig.AvgBytesPerSecond;
      this.MinBytesPerSecond = orig.MinBytesPerSecond;
      this.MaxBytesPerSecond = orig.MaxBytesPerSecond;
      this.NumEntries = orig.NumEntries;
      if (!includeEntries)
        return;
      this.Entries = orig.Entries;
    }

    public TransferRateResults(DateTime startTime, IList<TransferEntry> entries)
    {
      ArgumentUtility.CheckForNull<IList<TransferEntry>>(entries, nameof (entries));
      this.StartTime = startTime;
      this.Entries = (IEnumerable<TransferEntry>) entries;
      this.NumEntries = entries.Count;
      double? nullable1 = new double?();
      double? nullable2 = new double?();
      foreach (TransferEntry entry in this.Entries)
      {
        this.TotalBytes = this.TotalBytes + entry.CurrentIntervalBytes;
        this.TotalTime = this.TotalTime + entry.CurrentIntervalTime;
        double num1 = (double) entry.CurrentIntervalBytes / entry.CurrentIntervalTime.TotalSeconds;
        if (nullable1.HasValue)
        {
          double num2 = num1;
          double? nullable3 = nullable1;
          double valueOrDefault = nullable3.GetValueOrDefault();
          if (!(num2 < valueOrDefault & nullable3.HasValue))
            goto label_5;
        }
        nullable1 = new double?(num1);
label_5:
        if (nullable2.HasValue)
        {
          double num3 = num1;
          double? nullable4 = nullable2;
          double valueOrDefault = nullable4.GetValueOrDefault();
          if (!(num3 > valueOrDefault & nullable4.HasValue))
            continue;
        }
        nullable2 = new double?(num1);
      }
      this.EndTime = this.StartTime + this.TotalTime;
      this.AvgBytesPerSecond = (double) this.TotalBytes / this.TotalTime.TotalSeconds;
      this.MinBytesPerSecond = nullable1.GetValueOrDefault();
      this.MaxBytesPerSecond = nullable2.GetValueOrDefault();
    }

    [DataMember(Name = "startTime")]
    public DateTime StartTime { get; }

    [DataMember(Name = "endTime")]
    public DateTime EndTime { get; }

    [DataMember(Name = "totalTime")]
    public TimeSpan TotalTime { get; }

    [DataMember(Name = "totalBytes")]
    public long TotalBytes { get; }

    [DataMember(Name = "avgBytesPerSecond")]
    public double AvgBytesPerSecond { get; }

    [DataMember(Name = "minBytesPerSecond")]
    public double MinBytesPerSecond { get; }

    [DataMember(Name = "maxBytesPerSecond")]
    public double MaxBytesPerSecond { get; }

    [DataMember(Name = "numEntries")]
    public int NumEntries { get; }

    [DataMember(Name = "entries", EmitDefaultValue = false)]
    public IEnumerable<TransferEntry> Entries { get; }

    public TransferRateResults WithoutEntries() => new TransferRateResults(this, false);
  }
}
