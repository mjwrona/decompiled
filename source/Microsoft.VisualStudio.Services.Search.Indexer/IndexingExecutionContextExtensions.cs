// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class IndexingExecutionContextExtensions
  {
    public static bool IsSupportedFileExtensionForContentCrawl(
      this IndexingExecutionContext indexingExecutionContext,
      string path)
    {
      return IndexingExecutionContextExtensions.IsSupportedFileExtension(indexingExecutionContext.CrawlerSettings.SupportedFileExtensionsForContentCrawl, indexingExecutionContext.CrawlerSettings.UnsupportedFileExtensionsForContentCrawl, path);
    }

    public static bool IsSupportedFileExtensionForIndexing(
      this IndexingExecutionContext indexingExecutionContext,
      string path)
    {
      return IndexingExecutionContextExtensions.IsSupportedFileExtension(indexingExecutionContext.CrawlerSettings.SupportedFileExtensionsForIndexing, indexingExecutionContext.CrawlerSettings.UnsupportedFileExtensionsForIndexing, path);
    }

    public static int GetIndexVersion(
      this IndexingExecutionContext indexingExecutionContext,
      string indexName)
    {
      return indexingExecutionContext.ProvisioningContext.SearchPlatform.GetIndexVersion(indexingExecutionContext.IndexingUnit.EntityType, indexName);
    }

    private static bool IsSupportedFileExtension(
      IEnumerable<string> supportedFileExtensions,
      IEnumerable<string> unsupportedFileExtensions,
      string path)
    {
      string lowerInvariant = (FilePathUtils.GetFileExtension(path) ?? string.Empty).ToLowerInvariant();
      if (supportedFileExtensions != null && supportedFileExtensions.Count<string>() > 0)
        return supportedFileExtensions.Contains<string>(lowerInvariant);
      return unsupportedFileExtensions == null || unsupportedFileExtensions.Count<string>() <= 0 || !unsupportedFileExtensions.Contains<string>(lowerInvariant);
    }

    public static bool IsShadowIndexingRequired(
      this IndexingExecutionContext indexingExecutionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      switch (indexingExecutionContext.IndexingUnit.EntityType.Name)
      {
        case "WorkItem":
          return indexingUnitChangeEvent.ChangeData.Trigger == 33 && indexingExecutionContext.RequestContext.IsWorkItemReindexingWithZeroStalenessFeatureEnabled();
        case "Code":
          return indexingUnitChangeEvent.ChangeData.Trigger == 33 && indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled();
        default:
          return false;
      }
    }

    public static bool CheckIfZLRI_IsEnabledForEntity(
      this IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit)
    {
      IEntityType entityType = indexingUnit.EntityType;
      if (entityType.Name == "Code")
        return indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled();
      return entityType.Name == "WorkItem" && indexingExecutionContext.RequestContext.IsWorkItemReindexingWithZeroStalenessFeatureEnabled();
    }
  }
}
