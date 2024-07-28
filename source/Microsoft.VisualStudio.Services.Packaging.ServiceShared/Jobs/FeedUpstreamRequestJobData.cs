// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.FeedUpstreamRequestJobData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  [DataContract]
  public class FeedUpstreamRequestJobData
  {
    public FeedUpstreamRequestJobData()
    {
    }

    public FeedUpstreamRequestJobData(
      UpstreamPackagesToRefreshInformation upstreamPackagesToRefresh,
      Guid correlationId,
      int numSplitJobs)
    {
      this.UpstreamPackagesToRefresh = upstreamPackagesToRefresh;
      this.CorrelationId = correlationId;
      this.NumSplitJobs = numSplitJobs;
    }

    [DataMember]
    public UpstreamPackagesToRefreshInformation UpstreamPackagesToRefresh { get; set; }

    [DataMember]
    public Guid CorrelationId { get; set; }

    [DataMember]
    public int NumSplitJobs { get; set; }
  }
}
