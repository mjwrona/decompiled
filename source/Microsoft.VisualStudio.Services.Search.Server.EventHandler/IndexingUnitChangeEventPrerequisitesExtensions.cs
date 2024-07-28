// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventPrerequisitesExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  internal static class IndexingUnitChangeEventPrerequisitesExtensions
  {
    public static bool Evaluate(
      this IndexingUnitChangeEventPrerequisites prerequisites,
      ExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      IEntityType entityType,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = dataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      IReindexingStatusDataAccess statusDataAccess = dataAccessFactory.GetReindexingStatusDataAccess();
      bool flag1 = true;
      bool flag2 = true;
      if (prerequisites.Count == 0)
        return true;
      IList<long> list = (IList<long>) prerequisites.Select<IndexingUnitChangeEventPrerequisitesFilter, long>((Func<IndexingUnitChangeEventPrerequisitesFilter, long>) (item => item.Id)).ToList<long>();
      IDictionary<long, IndexingUnitChangeEventPrerequisitesFilter> dictionary = (IDictionary<long, IndexingUnitChangeEventPrerequisitesFilter>) new Dictionary<long, IndexingUnitChangeEventPrerequisitesFilter>();
      foreach (IndexingUnitChangeEventPrerequisitesFilter prerequisite in (List<IndexingUnitChangeEventPrerequisitesFilter>) prerequisites)
        dictionary[prerequisite.Id] = prerequisite;
      IDictionary<long, IndexingUnitChangeEventState> unitChangeEvents = changeEventDataAccess.GetStateOfIndexingUnitChangeEvents(executionContext.RequestContext, list);
      if (unitChangeEvents.Count != dictionary.Count)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080522, "Indexing Pipeline", "IndexingUnitChangeEventHandler", "Prereque event(s) is deleted.");
      foreach (KeyValuePair<long, IndexingUnitChangeEventState> keyValuePair in (IEnumerable<KeyValuePair<long, IndexingUnitChangeEventState>>) unitChangeEvents)
      {
        IndexingUnitChangeEventPrerequisitesFilter prerequisitesFilter = dictionary[keyValuePair.Key];
        switch (prerequisitesFilter.Operator)
        {
          case IndexingUnitChangeEventFilterOperator.Contains:
            flag1 = prerequisitesFilter.PossibleStates.Contains(keyValuePair.Value);
            break;
          case IndexingUnitChangeEventFilterOperator.DoesNotContain:
            flag1 = !prerequisitesFilter.PossibleStates.Contains(keyValuePair.Value);
            break;
        }
        flag2 &= flag1;
      }
      if (!flag2)
        return false;
      if (indexingUnitChangeEvent != null && indexingUnitChangeEvent.ChangeType == "CompleteBulkIndex" && indexingUnitChangeEvent.ChangeData.Trigger == 33 && executionContext.RequestContext.IsZeroStalenessReindexingEnabled(entityType))
      {
        ReindexingStatusEntry reindexingStatusEntry = statusDataAccess.GetReindexingStatusEntry(executionContext.RequestContext.To(TeamFoundationHostType.Deployment), executionContext.RequestContext.GetCollectionID(), entityType);
        if ((reindexingStatusEntry != null ? (reindexingStatusEntry.IsReindexingFailedOrInProgress() ? 1 : 0) : 0) != 0)
          return executionContext.RequestContext.GetService<IReindexingStatusService>().CanFinalize(executionContext.RequestContext, entityType);
      }
      return flag2;
    }
  }
}
