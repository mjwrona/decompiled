// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IIndexPatchProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal interface IIndexPatchProvider
  {
    IEnumerable<PatchDescription> GetPatches(
      IndexingExecutionContext iexContext,
      string branchName,
      CodeCrawlSpec codeCrawlSpec,
      IndexingUnit indexingUnit);

    void PostPatchOperation(
      IndexingExecutionContext iexContext,
      string branchName,
      IEnumerable<PatchDescription> patchDescriptions);
  }
}
