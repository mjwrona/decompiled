// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.MissingWorkItemProjectHandlerTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class MissingWorkItemProjectHandlerTask : IIndexingPatchTask
  {
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private readonly IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    private readonly IDictionary<Guid, string> m_nameMapForWellFormedTfsEntities;
    private readonly IndexingUnit m_collectionIndexingUnit;

    public string Name { get; } = nameof (MissingWorkItemProjectHandlerTask);

    internal MissingWorkItemProjectHandlerTask(
      IndexingUnit collectionIndexingUnit,
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IDictionary<Guid, string> nameMapForWellFormedTfsEntities)
    {
      this.m_collectionIndexingUnit = collectionIndexingUnit;
      this.m_indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
      this.m_nameMapForWellFormedTfsEntities = nameMapForWellFormedTfsEntities;
    }

    public void Patch(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      ISet<Guid> existingProjectIndexingUnitIds = (ISet<Guid>) new HashSet<Guid>(this.m_indexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", (IEntityType) WorkItemEntityType.GetInstance(), -1).Select<IndexingUnit, Guid>((Func<IndexingUnit, Guid>) (iu => iu.TFSEntityId)));
      IEnumerable<KeyValuePair<Guid, string>> keyValuePairs = this.m_nameMapForWellFormedTfsEntities.Where<KeyValuePair<Guid, string>>((Func<KeyValuePair<Guid, string>, bool>) (kvp => !existingProjectIndexingUnitIds.Contains(kvp.Key)));
      List<IndexingUnit> indexingUnits = new List<IndexingUnit>();
      foreach (KeyValuePair<Guid, string> keyValuePair in keyValuePairs)
      {
        TeamProject teamProject1 = new TeamProject();
        teamProject1.Id = keyValuePair.Key;
        teamProject1.Name = keyValuePair.Value;
        TeamProject teamProject2 = teamProject1;
        IndexingUnit itemIndexingUnit;
        try
        {
          itemIndexingUnit = teamProject2.ToProjectWorkItemIndexingUnit(this.m_collectionIndexingUnit);
        }
        catch (SearchServiceException ex) when (ex.ErrorCode == SearchServiceErrorCode.IndexingIndexNameDoesNotExist)
        {
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Collection Indexing Unit [{0}] does not have Indexing Index Name in its Properties. Hence, not creating any project indexing unit. ", (object) this.m_collectionIndexingUnit)));
          return;
        }
        indexingUnits.Add(itemIndexingUnit);
      }
      if (indexingUnits.Count > 0)
      {
        List<IndexingUnit> indexingUnitList = this.m_indexingUnitDataAccess.AddOrUpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnits, true);
        IList<IndexingUnitChangeEvent> indexingUnitChangeEventList1 = (IList<IndexingUnitChangeEvent>) new List<IndexingUnitChangeEvent>();
        foreach (IndexingUnit indexingUnit in indexingUnitList)
          indexingUnitChangeEventList1.Add(new IndexingUnitChangeEvent()
          {
            IndexingUnitId = indexingUnit.IndexingUnitId,
            ChangeType = "BeginBulkIndex",
            ChangeData = (ChangeEventData) new WorkItemBulkIndexEventData((ExecutionContext) indexingExecutionContext)
            {
              Finalize = false
            },
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = (byte) 0
          });
        IList<IndexingUnitChangeEvent> indexingUnitChangeEventList2 = this.m_indexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, indexingUnitChangeEventList1);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created [{0}] {1} events for projects. ", (object) indexingUnitChangeEventList2.Count, (object) "BeginBulkIndex")));
      }
      else
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created [0] {0} events for projects. ", (object) "BeginBulkIndex")));
    }
  }
}
