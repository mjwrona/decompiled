// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.RepositoryIndexingUnitsDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class RepositoryIndexingUnitsDataProvider : IndexingUnitsDataProvider
  {
    protected List<IndexingUnit> RepoIndexingUnitsForCollection { get; set; }

    public RepositoryIndexingUnitsDataProvider()
      : this(DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal RepositoryIndexingUnitsDataProvider(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    public override List<HealthData> GetData(ProviderContext providerContext)
    {
      IndexingUnitContext indexingUnitContext = (IndexingUnitContext) providerContext;
      if (this.RepoIndexingUnitsForCollection == null)
      {
        this.RepoIndexingUnitsForCollection = new List<IndexingUnit>();
        List<IEntityType> entityTypeList;
        if (!(indexingUnitContext.EntityType.Name == "All"))
        {
          entityTypeList = new List<IEntityType>()
          {
            indexingUnitContext.EntityType
          };
        }
        else
        {
          entityTypeList = new List<IEntityType>();
          entityTypeList.Add((IEntityType) CodeEntityType.GetInstance());
          entityTypeList.Add((IEntityType) WikiEntityType.GetInstance());
        }
        List<IEntityType> entityTypes = entityTypeList;
        this.RepoIndexingUnitsForCollection = this.GetRepoIndexingUnits((ProviderContext) indexingUnitContext, entityTypes);
      }
      return new List<HealthData>()
      {
        (HealthData) new IndexingUnitData((IEnumerable<IndexingUnit>) this.RepoIndexingUnitsForCollection, DataType.RepoIndexingUnitData)
      };
    }

    private List<IndexingUnit> GetRepoIndexingUnits(
      ProviderContext providerContext,
      List<IEntityType> entityTypes)
    {
      foreach (IEntityType entityType in entityTypes)
      {
        switch (entityType.Name)
        {
          case "Code":
            this.RepoIndexingUnitsForCollection.AddRange((IEnumerable<IndexingUnit>) this.IndexingUnitDataAccess.GetIndexingUnits(providerContext.RequestContext, "Git_Repository", entityType, -1));
            this.RepoIndexingUnitsForCollection.AddRange((IEnumerable<IndexingUnit>) this.IndexingUnitDataAccess.GetIndexingUnits(providerContext.RequestContext, "TFVC_Repository", entityType, -1));
            this.RepoIndexingUnitsForCollection.AddRange((IEnumerable<IndexingUnit>) this.IndexingUnitDataAccess.GetIndexingUnits(providerContext.RequestContext, "CustomRepository", entityType, -1));
            continue;
          case "Wiki":
            this.RepoIndexingUnitsForCollection.AddRange((IEnumerable<IndexingUnit>) this.IndexingUnitDataAccess.GetIndexingUnits(providerContext.RequestContext, "Git_Repository", entityType, -1));
            continue;
          default:
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported entity type {0} encountered while trying to fetch repository indexing units", (object) entityType)));
        }
      }
      return this.RepoIndexingUnitsForCollection;
    }
  }
}
