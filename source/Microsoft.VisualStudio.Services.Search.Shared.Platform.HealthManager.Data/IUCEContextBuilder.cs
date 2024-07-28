// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.IUCEContextBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class IUCEContextBuilder : IProviderContextBuilder
  {
    public ProviderContext BuildContext(
      IVssRequestContext requestContext,
      Scenario scenario,
      HealthStatusJobData healthInputData,
      CollectionIndexingProperties indexProperties = null)
    {
      IndexingUnitChangeEventContext changeEventContext = new IndexingUnitChangeEventContext();
      changeEventContext.IUCEOperations = this.GetIndexingUnitChangeOperationTypes(scenario);
      changeEventContext.IUCEStates = this.GetIndexingUnitChangeEventStates(scenario);
      changeEventContext.RequestContext = requestContext;
      return (ProviderContext) changeEventContext;
    }

    private List<string> GetIndexingUnitChangeOperationTypes(Scenario scenario)
    {
      HashSet<string> source = new HashSet<string>();
      source.UnionWith((IEnumerable<string>) new List<string>()
      {
        "BeginBulkIndex"
      });
      return source.ToList<string>();
    }

    private List<IndexingUnitChangeEventState> GetIndexingUnitChangeEventStates(Scenario scenario)
    {
      HashSet<IndexingUnitChangeEventState> source = new HashSet<IndexingUnitChangeEventState>();
      if (scenario == Scenario.CodeExtensionInstalledNoResults)
        source.UnionWith((IEnumerable<IndexingUnitChangeEventState>) new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Failed,
          IndexingUnitChangeEventState.Pending,
          IndexingUnitChangeEventState.Queued,
          IndexingUnitChangeEventState.InProgress,
          IndexingUnitChangeEventState.FailedAndRetry,
          IndexingUnitChangeEventState.Succeeded
        });
      return source.ToList<IndexingUnitChangeEventState>();
    }
  }
}
