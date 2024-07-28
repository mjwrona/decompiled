// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.PipelineSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class PipelineSettings
  {
    public PipelineSettings()
    {
    }

    public PipelineSettings(IVssRequestContext requestContext)
    {
      this.CrawlTreeTraversalTimeBoundInMs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CrawlTreeTraversalTimeBoundInMs");
      this.CrawlOperationRetryLimit = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CrawlOperationRetryLimit");
      this.CrawlOperationRetryIntervalInSec = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/CrawlOperationRetryIntervalInSec", true, 60);
      this.MaxWIQLResults = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxWIQLResults");
      this.WorkItemBICrawlDelayInMs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemBICrawlDelayInMs");
      this.WorkItemCICrawlDelayInMs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemCICrawlDelayInMs");
      this.WorkItemCIJobDelayInSec = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/WorkItemCIJobDelayInSec", true, 1);
      this.WorkItemFetchBatchSize = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemFetchBatchSize");
      this.ParserTimeBoundInMs = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/ParserTimeBoundInMs");
      this.BatchCountForJobYieldContent = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForJobYieldContent");
      this.MaxNoOfBlobIdsSupportedByGetBlobSizeApi = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxNoOfBlobIdsSupportedByGetBlobSizeApi");
      this.MaxNoOfBlobsToDownloadAtATime = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxNoOfBlobsToDownloadAtATime");
      this.MaxContentSizeToDownloadAtATime = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxContentSizeToDownloadAtATime");
      this.AcceptableMaxFractionOfFailedDocs = requestContext.GetConfigValue<float>("/Service/ALMSearch/Settings/AcceptableMaxFractionOfFailedDocs");
      this.MaxNumberOfParallelFeed = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxNumberOfParallelFeed");
    }

    public int CrawlTreeTraversalTimeBoundInMs { get; set; }

    public int CrawlOperationRetryLimit { get; set; }

    public int CrawlOperationRetryIntervalInSec { get; set; }

    public int MaxWIQLResults { get; set; }

    public int WorkItemBICrawlDelayInMs { get; set; }

    public int WorkItemCICrawlDelayInMs { get; set; }

    public int WorkItemCIJobDelayInSec { get; set; }

    public int WorkItemFetchBatchSize { get; set; }

    public int ParserTimeBoundInMs { get; set; }

    public int BatchCountForJobYieldContent { get; set; }

    public int MaxNoOfBlobIdsSupportedByGetBlobSizeApi { get; set; }

    public int MaxNoOfBlobsToDownloadAtATime { get; set; }

    public int MaxContentSizeToDownloadAtATime { get; set; }

    public float AcceptableMaxFractionOfFailedDocs { get; set; }

    public int MaxNumberOfParallelFeed { get; set; }
  }
}
