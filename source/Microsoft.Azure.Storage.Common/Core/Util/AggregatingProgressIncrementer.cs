// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.AggregatingProgressIncrementer
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.IO;
using System.Threading;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal sealed class AggregatingProgressIncrementer : IProgress<long>
  {
    private long currentValue;
    private bool currentValueHasValue;
    private IProgress<StorageProgress> innerHandler;
    private static readonly AggregatingProgressIncrementer nullHandler = new AggregatingProgressIncrementer((IProgress<StorageProgress>) null);

    public Stream CreateProgressIncrementingStream(Stream stream) => this.innerHandler == null ? stream : (Stream) new ProgressIncrementingStream(stream, this);

    public AggregatingProgressIncrementer(IProgress<StorageProgress> innerHandler) => this.innerHandler = innerHandler;

    public void Report(long bytes)
    {
      Interlocked.Add(ref this.currentValue, bytes);
      Volatile.Write(ref this.currentValueHasValue, true);
      if (this.innerHandler == null)
        return;
      StorageProgress current = this.Current;
      if (current == null)
        return;
      this.innerHandler.Report(current);
    }

    public void Reset() => this.Report(-Volatile.Read(ref this.currentValue));

    public static AggregatingProgressIncrementer None => AggregatingProgressIncrementer.nullHandler;

    public StorageProgress Current
    {
      get
      {
        StorageProgress current = (StorageProgress) null;
        if (this.currentValueHasValue)
          current = new StorageProgress(Volatile.Read(ref this.currentValue));
        return current;
      }
    }
  }
}
