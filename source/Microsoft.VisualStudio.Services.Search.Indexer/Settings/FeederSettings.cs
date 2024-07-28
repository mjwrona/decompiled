// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Settings.FeederSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Settings
{
  public class FeederSettings
  {
    internal FeederSettings(IVssRequestContext requestContext, IEntityType entityType)
    {
      this.IntializeFeederSettingsCommon(requestContext);
      switch (entityType.Name)
      {
        case "Code":
          this.IntializeFeederSettingsForCode(requestContext);
          break;
        case "WorkItem":
          this.IntializeFeederSettingsForWorkItem(requestContext);
          break;
      }
    }

    private void IntializeFeederSettingsForCode(IVssRequestContext requestContext)
    {
      this.OptimalBatchSizeInMB = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchSizeInMB");
      if (this.OptimalBatchSizeInMB == 0)
        this.OptimalBatchSizeInMB = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchSizeInMB");
      this.OptimalBatchFileCount = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchFileCount");
      if (this.OptimalBatchFileCount != 0)
        return;
      this.OptimalBatchFileCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchFileCount");
    }

    private void IntializeFeederSettingsForWorkItem(IVssRequestContext requestContext)
    {
      this.OptimalBatchSizeInMB = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchSizeInMBForWorkItem");
      this.OptimalBatchFileCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchFileCountForWorkItem");
    }

    private void IntializeFeederSettingsCommon(IVssRequestContext requestContext)
    {
      this.MaxDocumentSizeToFeedInBytes = requestContext.GetConfigValue<long>("/Service/ALMSearch/Settings/MaxDocumentSizeToFeedInBytes", TeamFoundationHostType.Deployment, 10485760L);
      this.OptimalBatchSizeInMB = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchSizeInMB", TeamFoundationHostType.Deployment, 15);
      this.OptimalBatchFileCount = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/OptimalBatchFileCount", TeamFoundationHostType.Deployment, 500);
      this.FeedingBatchTimeBoundInMs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FeedingBatchTimeBoundInMs", TeamFoundationHostType.Deployment, 1800000);
      this.FeedOperationRetryIntervalInSec = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FeedOperationRetryIntervalInSec", TeamFoundationHostType.Deployment, 1);
    }

    public long MaxDocumentSizeToFeedInBytes { get; set; }

    public int OptimalBatchSizeInMB { get; set; }

    public int OptimalBatchFileCount { get; set; }

    public int FeedingBatchTimeBoundInMs { get; set; }

    public int FeedOperationRetryIntervalInSec { get; set; }
  }
}
