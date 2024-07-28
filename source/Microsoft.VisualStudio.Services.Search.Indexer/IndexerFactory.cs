// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class IndexerFactory
  {
    internal static AbstractIndexUpdater CreateIndexUpdater(
      IndexingExecutionContext indexingExecutionContext,
      IndexUpdaterParams indexUpdaterParams,
      ISearchPlatform searchPlatform,
      RouteLevel routingLevel)
    {
      IndexerFactory.ValidateIndexUpdaterParams(indexUpdaterParams, searchPlatform);
      ISearchIndex index = searchPlatform.GetIndex(indexUpdaterParams.IndexIdentity);
      string name = indexUpdaterParams.ContractType.GetEntityTypeForContractType().Name;
      if (name != null)
      {
        switch (name.Length)
        {
          case 4:
            switch (name[0])
            {
              case 'C':
                if (name == "Code")
                  return (AbstractIndexUpdater) new CodeIndexUpdater(indexingExecutionContext, indexUpdaterParams.IndexSubScope, index, routingLevel, indexUpdaterParams.ContractType);
                break;
              case 'W':
                if (name == "Wiki")
                  return (AbstractIndexUpdater) new WikiIndexUpdater(indexingExecutionContext, indexUpdaterParams.IndexSubScope, index, routingLevel, indexUpdaterParams.ContractType);
                break;
            }
            break;
          case 5:
            if (name == "Board")
              return (AbstractIndexUpdater) new BoardIndexUpdater(indexingExecutionContext, indexUpdaterParams.IndexSubScope, index, routingLevel, indexUpdaterParams.ContractType);
            break;
          case 7:
            switch (name[0])
            {
              case 'P':
                if (name == "Package")
                  return (AbstractIndexUpdater) new PackageVersionIndexUpdater(indexingExecutionContext, indexUpdaterParams.IndexSubScope, index, routingLevel, indexUpdaterParams.ContractType);
                break;
              case 'S':
                if (name == "Setting")
                  return (AbstractIndexUpdater) new SettingIndexUpdater(indexingExecutionContext, indexUpdaterParams.IndexSubScope, index, routingLevel, indexUpdaterParams.ContractType);
                break;
            }
            break;
          case 8:
            if (name == "WorkItem")
              return (AbstractIndexUpdater) new WorkItemIndexUpdater(indexingExecutionContext, indexUpdaterParams.IndexSubScope, index, routingLevel, indexUpdaterParams.ContractType);
            break;
          case 11:
            if (name == "ProjectRepo")
              return (AbstractIndexUpdater) new ProjectIndexUpdater(indexingExecutionContext, indexUpdaterParams.IndexSubScope, index, routingLevel, indexUpdaterParams.ContractType);
            break;
        }
      }
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported contract type '{0}'", (object) indexUpdaterParams.ContractType)));
    }

    private static void ValidateIndexUpdaterParams(
      IndexUpdaterParams indexParams,
      ISearchPlatform searchPlatform)
    {
      if (indexParams?.IndexIdentity == null || string.IsNullOrWhiteSpace(indexParams.IndexIdentity.Name))
        throw new ArgumentException("Index information is not available in index params.");
      if (searchPlatform == null)
        throw new ArgumentNullException(nameof (searchPlatform));
      if (!Enum.IsDefined(typeof (DocumentContractType), (object) indexParams.ContractType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Document contract type {0} is not supported", (object) indexParams.ContractType.ToString()));
      if (indexParams.IndexSubScope == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Scope of index update needs to be specified"));
    }
  }
}
