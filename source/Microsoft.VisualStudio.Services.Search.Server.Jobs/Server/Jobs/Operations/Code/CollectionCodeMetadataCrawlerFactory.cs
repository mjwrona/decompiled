// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCodeMetadataCrawlerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal static class CollectionCodeMetadataCrawlerFactory
  {
    public static ICollectionMetadataCrawler GetCrawler(
      IndexingExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      string indexingUnitType,
      CodeFileContract codeFileContract)
    {
      switch (indexingUnitType)
      {
        case "TFVC_Repository":
          return (ICollectionMetadataCrawler) new CollectionTfvcCodeMetadataCrawler(executionContext, dataAccessFactory, codeFileContract);
        case "Git_Repository":
          return (ICollectionMetadataCrawler) new CollectionGitCodeMetadataCrawler(executionContext, dataAccessFactory, codeFileContract);
        case "CustomRepository":
          return (ICollectionMetadataCrawler) new CollectionCustomCodeMetadataCrawler(executionContext, dataAccessFactory, codeFileContract);
        default:
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IndexingUnitType '{0}' does not have a supported metadata crawler at collection level.", (object) indexingUnitType));
      }
    }
  }
}
