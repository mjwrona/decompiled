// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.ICollectionMetadataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations
{
  internal interface ICollectionMetadataCrawler
  {
    List<IndexingUnitWithSize> CrawlMetadata(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit,
      bool isShadowCrawlingRequired = false);
  }
}
