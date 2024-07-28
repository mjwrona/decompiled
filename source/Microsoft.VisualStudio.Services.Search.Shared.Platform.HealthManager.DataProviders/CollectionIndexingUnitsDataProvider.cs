// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.CollectionIndexingUnitsDataProvider
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
  public class CollectionIndexingUnitsDataProvider : IndexingUnitsDataProvider
  {
    private List<IndexingUnit> m_collectionIndexingUnits;

    public CollectionIndexingUnitsDataProvider()
      : this(DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal CollectionIndexingUnitsDataProvider(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    public override List<HealthData> GetData(ProviderContext providerContext)
    {
      IndexingUnitContext indexingUnitContext = (IndexingUnitContext) providerContext;
      if (this.m_collectionIndexingUnits == null)
      {
        if (indexingUnitContext.EntityType.Name == "All")
        {
          this.m_collectionIndexingUnits = new List<IndexingUnit>();
          foreach (IEntityType entityType in providerContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes())
            this.m_collectionIndexingUnits.AddRange((IEnumerable<IndexingUnit>) this.IndexingUnitDataAccess.GetIndexingUnits(indexingUnitContext.RequestContext, "Collection", entityType, -1));
        }
        else
          this.m_collectionIndexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(indexingUnitContext.RequestContext, "Collection", indexingUnitContext.EntityType, -1);
      }
      return new List<HealthData>()
      {
        (HealthData) new IndexingUnitData((IEnumerable<IndexingUnit>) this.m_collectionIndexingUnits, DataType.CollectionIndexingUnitData)
      };
    }
  }
}
