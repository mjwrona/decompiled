// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.FailedFilesDataProvider
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
  public class FailedFilesDataProvider : IDataProvider
  {
    private readonly IItemLevelFailureDataAccess m_itemLevelFailureDataAccess;

    public FailedFilesDataProvider()
      : this(DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal FailedFilesDataProvider(IDataAccessFactory dataAccessFactory) => this.m_itemLevelFailureDataAccess = dataAccessFactory.GetItemLevelFailureDataAccess();

    public List<HealthData> GetData(ProviderContext providerContext) => new List<HealthData>()
    {
      (HealthData) new FailedFilesCountData(this.m_itemLevelFailureDataAccess.GetCountOfRecordsByIndexingUnit(providerContext.RequestContext), DataType.FailedFilesCountData)
    };
  }
}
