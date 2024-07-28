// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.IndexingUnitsDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public abstract class IndexingUnitsDataProvider : IDataProvider
  {
    internal IDataAccessFactory m_dataAccessFactory;

    protected IIndexingUnitDataAccess IndexingUnitDataAccess { get; set; }

    [Info("InternalForTestPurpose")]
    internal IndexingUnitsDataProvider(IDataAccessFactory dataAccessFactory)
    {
      this.m_dataAccessFactory = dataAccessFactory;
      this.IndexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
    }

    public abstract List<HealthData> GetData(ProviderContext providerContext);
  }
}
