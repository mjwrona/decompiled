// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.ProjectIndexingUnitsDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class ProjectIndexingUnitsDataProvider : IndexingUnitsDataProvider
  {
    protected List<IndexingUnit> ProjectIndexingUnitsForCollection { get; set; }

    public ProjectIndexingUnitsDataProvider()
      : this(DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal ProjectIndexingUnitsDataProvider(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    public override List<HealthData> GetData(ProviderContext providerContext)
    {
      IndexingUnitContext indexingUnitContext = (IndexingUnitContext) providerContext;
      if (this.ProjectIndexingUnitsForCollection == null)
      {
        if (indexingUnitContext.EntityType.Name == "All")
        {
          this.ProjectIndexingUnitsForCollection = new List<IndexingUnit>();
          foreach (IEntityType entityType in providerContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes())
            this.ProjectIndexingUnitsForCollection.AddRange((IEnumerable<IndexingUnit>) this.IndexingUnitDataAccess.GetIndexingUnits(indexingUnitContext.RequestContext, "Project", entityType, -1));
        }
        else
          this.ProjectIndexingUnitsForCollection = this.IndexingUnitDataAccess.GetIndexingUnits(indexingUnitContext.RequestContext, "Project", indexingUnitContext.EntityType, -1);
      }
      return new List<HealthData>()
      {
        (HealthData) new IndexingUnitData((IEnumerable<IndexingUnit>) this.ProjectIndexingUnitsForCollection, DataType.ProjectIndexingUnitData)
      };
    }
  }
}
