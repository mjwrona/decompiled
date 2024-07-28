// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.ReindexingStatusDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class ReindexingStatusDataProvider : IDataProvider
  {
    private readonly IReindexingStatusDataAccess m_reindexingStatusDataAccess;

    public ReindexingStatusDataProvider()
      : this(DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal ReindexingStatusDataProvider(IDataAccessFactory dataAccessFactory) => this.m_reindexingStatusDataAccess = dataAccessFactory.GetReindexingStatusDataAccess();

    public List<HealthData> GetData(ProviderContext providerContext)
    {
      ReindexingStatusContext reindexingStatusContext = (ReindexingStatusContext) providerContext;
      Dictionary<IEntityType, ReindexingStatusEntry> reindexingStatusData = new Dictionary<IEntityType, ReindexingStatusEntry>();
      if (reindexingStatusContext != null)
      {
        List<IEntityType> entityTypeList1;
        if (!(reindexingStatusContext.EntityType.Name == "All"))
        {
          entityTypeList1 = new List<IEntityType>()
          {
            reindexingStatusContext.EntityType
          };
        }
        else
        {
          entityTypeList1 = new List<IEntityType>();
          entityTypeList1.Add((IEntityType) CodeEntityType.GetInstance());
          entityTypeList1.Add((IEntityType) WorkItemEntityType.GetInstance());
          entityTypeList1.Add((IEntityType) WikiEntityType.GetInstance());
        }
        List<IEntityType> entityTypeList2 = entityTypeList1;
        if (providerContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          foreach (IEntityType key in entityTypeList2)
            reindexingStatusData.Add(key, (ReindexingStatusEntry) null);
        }
        else
        {
          foreach (IEntityType entityType in entityTypeList2)
            reindexingStatusData.Add(entityType, this.m_reindexingStatusDataAccess.GetReindexingStatusEntry(reindexingStatusContext.DeploymentRequestContext, reindexingStatusContext.RequestContext.GetCurrentHostId(), entityType));
        }
      }
      return new List<HealthData>()
      {
        (HealthData) new ReindexingStatusData((IDictionary<IEntityType, ReindexingStatusEntry>) reindexingStatusData, DataType.ReindexingStatusData)
      };
    }
  }
}
