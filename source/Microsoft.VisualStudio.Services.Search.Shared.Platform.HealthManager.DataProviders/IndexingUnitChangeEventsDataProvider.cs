// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.IndexingUnitChangeEventsDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class IndexingUnitChangeEventsDataProvider : IDataProvider
  {
    private readonly IIndexingUnitChangeEventDataAccess m_indexingUnitChangeEventDataAccess;

    public IndexingUnitChangeEventsDataProvider()
      : this(DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal IndexingUnitChangeEventsDataProvider(IDataAccessFactory dataAccessFactory) => this.m_indexingUnitChangeEventDataAccess = dataAccessFactory.GetIndexingUnitChangeEventDataAccess();

    public List<HealthData> GetData(ProviderContext providerContext)
    {
      IndexingUnitChangeEventContext changeEventContext = (IndexingUnitChangeEventContext) providerContext;
      return new List<HealthData>()
      {
        (HealthData) new IndexingUnitChangeEventData(this.m_indexingUnitChangeEventDataAccess.GetIndexingUnitChangeEvents(providerContext.RequestContext, changeEventContext.IUCEOperations, changeEventContext.IUCEStates, -1), DataType.IndexingUnitChangeEventsData)
      };
    }
  }
}
