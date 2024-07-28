// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamMetadataRefreshJobSplittingConfiguration
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class UpstreamMetadataRefreshJobSplittingConfiguration
  {
    private readonly IFrotocolLevelPackagingSetting<int> maxJobCountFactory;
    private readonly IFrotocolLevelPackagingSetting<int> higherLimitOfWorkFactory;

    public UpstreamMetadataRefreshJobSplittingConfiguration(
      IFrotocolLevelPackagingSetting<int> maxJobCountFactory,
      IFrotocolLevelPackagingSetting<int> higherLimitOfWorkFactory)
    {
      this.maxJobCountFactory = maxJobCountFactory;
      this.higherLimitOfWorkFactory = higherLimitOfWorkFactory;
    }

    public int GetMaxNumberOfJobs(IFeedRequest feedRequest) => this.maxJobCountFactory.Get(feedRequest);

    public int GetHigherLimitOfWork(IFeedRequest feedRequest) => this.higherLimitOfWorkFactory.Get(feedRequest);
  }
}
