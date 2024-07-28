// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.FailedItemsPatchDescriptionCreatorFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class FailedItemsPatchDescriptionCreatorFactory
  {
    public IFailedItemsPatchDescriptionCreator GetPatchDescriptorCreator(
      IndexingExecutionContext indexingExecutionContext,
      CodeCrawlSpec codeCrawlSpec,
      TraceMetaData traceMetaData)
    {
      switch (codeCrawlSpec)
      {
        case GitCrawlSpec _:
          return (IFailedItemsPatchDescriptionCreator) new GitFailedItemsPatchDescriptionCreator(indexingExecutionContext, traceMetaData);
        case TfvcCrawlSpec _:
          return (IFailedItemsPatchDescriptionCreator) new TfvcFailedItemsPatchDescriptionCreator(indexingExecutionContext, traceMetaData);
        default:
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported crawl spec: {0}", (object) codeCrawlSpec)));
      }
    }
  }
}
