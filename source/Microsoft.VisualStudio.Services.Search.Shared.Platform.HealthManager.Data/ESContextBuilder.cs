// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.ESContextBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class ESContextBuilder : IProviderContextBuilder
  {
    public ProviderContext BuildContext(
      IVssRequestContext requestContext,
      Scenario scenario,
      HealthStatusJobData healthInputData,
      CollectionIndexingProperties indexProperties)
    {
      ESContext esContext = new ESContext();
      esContext.ContractType = indexProperties.QueryContractType;
      esContext.Indices = indexProperties.QueryIndices.Select<IndexInfo, string>((Func<IndexInfo, string>) (x => x.IndexName)).ToList<string>();
      esContext.QueryConnectionString = indexProperties.QueryESConnectionString;
      esContext.RequestContext = requestContext;
      esContext.SearchFilters = healthInputData.SearchFilters;
      esContext.SearchText = healthInputData.SearchText;
      esContext.EntityType = healthInputData.EntityType;
      esContext.SearchPlatformSettings = requestContext.To(TeamFoundationHostType.Deployment).GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/JobAgentSearchPlatformSettings");
      if (esContext.SearchFilters == null)
        esContext.SearchFilters = new Dictionary<string, List<string>>();
      esContext.SearchFilters.Add("collectionId", new List<string>()
      {
        requestContext.GetCollectionIdAsNormalizedString()
      });
      return (ProviderContext) esContext;
    }
  }
}
