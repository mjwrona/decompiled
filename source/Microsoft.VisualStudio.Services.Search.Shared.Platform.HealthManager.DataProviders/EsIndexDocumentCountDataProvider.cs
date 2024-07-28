// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.EsIndexDocumentCountDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class EsIndexDocumentCountDataProvider : AbstractSearchDataProvider, IDataProvider
  {
    public EsIndexDocumentCountDataProvider()
      : base(Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchPlatformFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal EsIndexDocumentCountDataProvider(ISearchPlatformFactory searchPlatformFactory)
      : base(searchPlatformFactory)
    {
    }

    public List<HealthData> GetData(ProviderContext providerContext)
    {
      this.Initialize(providerContext);
      List<string> filteredIndices = this.GetFilteredIndices(providerContext);
      if (filteredIndices.Count == 0)
        return new List<HealthData>();
      return new List<HealthData>()
      {
        (HealthData) new SearchIndexDocumentCountData(this.SearchPlatform.GetIndices(this.ExecutionContext, filteredIndices).Records.Select<CatIndicesRecord, ElasticsearchIndexDetail>(new Func<CatIndicesRecord, ElasticsearchIndexDetail>(this.ToElasticsearchIndexDetail)), DataType.SearchIndexDocumentCountData)
      };
    }

    [Info("InternalForTestPurpose")]
    internal ElasticsearchIndexDetail ToElasticsearchIndexDetail(CatIndicesRecord catIndicesRecord)
    {
      long result1;
      long result2;
      return new ElasticsearchIndexDetail()
      {
        IndexName = catIndicesRecord.Index,
        DocumentCount = long.TryParse(catIndicesRecord.DocsCount, out result1) ? result1 : 0L,
        DeletedDocumentCount = long.TryParse(catIndicesRecord.DocsDeleted, out result2) ? result2 : 0L
      };
    }
  }
}
