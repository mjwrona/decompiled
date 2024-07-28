// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation.Diagnostics
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System.Runtime.Serialization;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupValidation
{
  [DataContract]
  public class Diagnostics
  {
    private long m_cachedCount;
    private long m_validationCachedCount;
    private long m_enqueueTotal;

    [DataMember(Name = "dedupInfoCacheHit", EmitDefaultValue = true)]
    public long DedupInfoCacheHitCount
    {
      get => this.m_cachedCount;
      internal set => this.m_cachedCount = value;
    }

    [DataMember(Name = "validationCacheHit", EmitDefaultValue = true)]
    public long ValidationCacheHitCount
    {
      get => this.m_validationCachedCount;
      internal set => this.m_validationCachedCount = value;
    }

    [DataMember(Name = "enqueuedDedupsTotal", EmitDefaultValue = true)]
    public long EnqueueCount
    {
      get => this.m_enqueueTotal;
      internal set => this.m_enqueueTotal = value;
    }

    internal void IncrementMetrics(ValidationMetrics metrics, long value)
    {
      switch (metrics)
      {
        case ValidationMetrics.ValidationCacheHit:
          if (value > 0L)
          {
            Interlocked.Add(ref this.m_validationCachedCount, value);
            break;
          }
          Interlocked.Increment(ref this.m_validationCachedCount);
          break;
        case ValidationMetrics.DedupInfoCacheHit:
          if (value > 0L)
          {
            Interlocked.Add(ref this.m_cachedCount, value);
            break;
          }
          Interlocked.Increment(ref this.m_cachedCount);
          break;
        case ValidationMetrics.EnqueueTotal:
          if (value > 0L)
          {
            Interlocked.Add(ref this.m_enqueueTotal, value);
            break;
          }
          Interlocked.Increment(ref this.m_enqueueTotal);
          break;
      }
    }
  }
}
