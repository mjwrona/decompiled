// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers.DataTransferStatistics
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using System.Threading;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers
{
  internal class DataTransferStatistics
  {
    private long m_totalPKs;
    private int m_batchPKs;
    private long m_totalEntities;
    private int m_batchEntities;

    internal long TotalPrimaryKeys => this.m_totalPKs;

    internal int PrimaryKeysFromLastBatch => this.m_batchPKs;

    internal long TotalEntities => this.m_totalEntities;

    internal int EntitiesFromLastBatch => this.m_batchEntities;

    internal void AddEntities(int countOfEntities)
    {
      Interlocked.Increment(ref this.m_totalPKs);
      Interlocked.Increment(ref this.m_batchPKs);
      Interlocked.Add(ref this.m_totalEntities, (long) countOfEntities);
      Interlocked.Add(ref this.m_batchEntities, countOfEntities);
    }

    internal void Reset()
    {
      this.m_batchPKs = 0;
      this.m_batchEntities = 0;
    }
  }
}
