// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.JobQueueDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class JobQueueDataProvider : IDataProvider
  {
    internal int batchSize = 100;
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;

    public JobQueueDataProvider()
      : this(DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal JobQueueDataProvider(IDataAccessFactory dataAccessFactory) => this.m_indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();

    public List<HealthData> GetData(ProviderContext providerContext)
    {
      JobQueueContext jobQueueContext = (JobQueueContext) providerContext;
      Dictionary<Guid, IEntityType> jobIds = jobQueueContext.JobIds;
      ITeamFoundationJobService service = jobQueueContext.RequestContext.GetService<ITeamFoundationJobService>();
      Dictionary<Guid, IndexingUnit> associatedJobIds = this.m_indexingUnitDataAccess.GetAssociatedJobIds(jobQueueContext.RequestContext, new List<IEntityType>()
      {
        (IEntityType) AllEntityType.GetInstance()
      });
      List<HealthData> data = new List<HealthData>();
      int count1 = associatedJobIds.Count;
      int count2 = 0;
      List<KeyValuePair<IndexingUnit, TeamFoundationJobQueueEntry>> jobQueueData1 = new List<KeyValuePair<IndexingUnit, TeamFoundationJobQueueEntry>>();
      if (associatedJobIds.Any<KeyValuePair<Guid, IndexingUnit>>())
      {
        while (count2 < count1)
        {
          Dictionary<Guid, IndexingUnit> dictionary = associatedJobIds.Skip<KeyValuePair<Guid, IndexingUnit>>(count2).Take<KeyValuePair<Guid, IndexingUnit>>(this.batchSize).ToDictionary<KeyValuePair<Guid, IndexingUnit>, Guid, IndexingUnit>((Func<KeyValuePair<Guid, IndexingUnit>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, IndexingUnit>, IndexingUnit>) (x => x.Value));
          count2 += dictionary.Count;
          foreach (TeamFoundationJobQueueEntry queryJob in service.QueryJobQueue(jobQueueContext.RequestContext, (IEnumerable<Guid>) dictionary.Keys))
          {
            if (queryJob != null)
              jobQueueData1.Add(new KeyValuePair<IndexingUnit, TeamFoundationJobQueueEntry>(associatedJobIds[queryJob.JobId], queryJob));
          }
        }
      }
      if (jobIds.Any<KeyValuePair<Guid, IEntityType>>())
      {
        foreach (TeamFoundationJobQueueEntry queryJob in service.QueryJobQueue(jobQueueContext.RequestContext, (IEnumerable<Guid>) jobIds.Keys))
        {
          if (queryJob != null)
            jobQueueData1.Add(new KeyValuePair<IndexingUnit, TeamFoundationJobQueueEntry>(new IndexingUnit(jobQueueContext.RequestContext.GetCollectionID(), "Collection", jobIds[queryJob.JobId], 0), queryJob));
        }
      }
      JobQueueData jobQueueData2 = new JobQueueData(jobQueueData1, DataType.JobQueueData);
      data.Add((HealthData) jobQueueData2);
      return data;
    }
  }
}
