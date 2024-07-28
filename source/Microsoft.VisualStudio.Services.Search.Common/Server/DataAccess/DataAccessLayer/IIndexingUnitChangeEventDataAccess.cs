// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IIndexingUnitChangeEventDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface IIndexingUnitChangeEventDataAccess
  {
    IndexingUnitChangeEvent GetIndexingUnitChangeEvent(IVssRequestContext requestContext, long id);

    IDictionary<long, IndexingUnitChangeEventState> GetStateOfIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IList<long> ids);

    IndexingUnitChangeEvent AddIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent);

    IList<IndexingUnitChangeEvent> AddIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IList<IndexingUnitChangeEvent> indexingUnitChangeEvents);

    IndexingUnitChangeEvent UpdateIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent);

    IList<IndexingUnitChangeEvent> UpdateIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IList<IndexingUnitChangeEvent> indexingUnitChangeEvents);

    void DeleteIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent);

    void DeleteIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      List<IndexingUnitChangeEvent> indexingUnitChangeEventList);

    void ArchiveIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      int periodicCleanupOldEvents);

    IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IndexingUnitChangeEventState state,
      int topCount);

    IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      IndexingUnitChangeEventState state,
      int topCount);

    int GetCountOfIndexingUnitChangeEventsInProgressOrQueued(
      IVssRequestContext requestContext,
      IEntityType entityType);

    IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string changeType,
      int topCount);

    IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string changeType,
      IndexingUnitChangeEventState state,
      int topCount);

    IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      List<string> changeTypeList,
      List<IndexingUnitChangeEventState> stateList,
      int topCount);

    IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      List<string> changeTypeList,
      List<IndexingUnitChangeEventState> stateList,
      int topCount);

    List<Tuple<IEntityType, string, int>> GetIndexingOperationsInProgress(
      IVssRequestContext requestContext,
      List<string> changeTypeList,
      List<int> jobTriggers);

    List<Tuple<IEntityType, int, string, int, ChangeEventData>> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      List<string> changeTypeList,
      List<int> jobTriggers,
      List<IndexingUnitChangeEventState> stateList);

    int GetCountOfIndexingUnitChangeEvents(IVssRequestContext requestContext);

    int GetCountOfIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IndexingUnitChangeEventState state);

    int GetCountOfIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IndexingUnitChangeEventState state,
      int numberOfDays);

    IDictionary<string, TimeSpan> GetMaxTimeForEachChangeType(IVssRequestContext requestContext);

    LockStatus AcquireLockAndMarkEventsAsQueued(
      IVssRequestContext requestContext,
      IList<LockDetails> lockingRequirements,
      IEnumerable<IndexingUnitChangeEvent> changeEvents);

    void ReleaseLocksOfCompletedEvents(IVssRequestContext requestContext);

    int DeleteIndexingUnitChangeEventsOfDeletedUnits(IVssRequestContext requestContext);

    IndexingUnitChangeEvent AddNewEventIfNotAlreadyPresent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      out bool newEventAdded);

    IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      IVssRequestContext requestContext,
      int count,
      int indexingUnitId);

    IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      IVssRequestContext requestContext,
      int count,
      IEntityType entityType);

    List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> GetChangeEventsCountOfAChangeTypeAndState(
      IVssRequestContext requestContext);

    IDictionary<string, int> GetCountOfEventsBreachingThreshold(
      IVssRequestContext requestContext,
      int bulkIndexThreshold,
      int updateIndexThreshold);
  }
}
