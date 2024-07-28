// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.IndexingUnitChangeEventDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  public class IndexingUnitChangeEventDataAccess : 
    SqlAzureDataAccess,
    IIndexingUnitChangeEventDataAccess
  {
    public IDictionary<long, IndexingUnitChangeEventState> GetStateOfIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IList<long> idList)
    {
      Dictionary<long, IndexingUnitChangeEventState> dictionary = new Dictionary<long, IndexingUnitChangeEventState>();
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return (IDictionary<long, IndexingUnitChangeEventState>) this.InvokeTableOperation<Dictionary<long, IndexingUnitChangeEventState>>((Func<Dictionary<long, IndexingUnitChangeEventState>>) (() => component.RetrieveStateOfIndexingUnitChangeEventsForListOfIds(idList)));
    }

    public IndexingUnitChangeEvent GetIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      long id)
    {
      if (id <= 0L)
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("Indexing Unit Change event Id must be a positive integer"));
      TableEntityFilterList entityFilterList = new TableEntityFilterList();
      entityFilterList.Add(new TableEntityFilter("Id", "eq", id.ToString()));
      TableEntityFilterList filters = entityFilterList;
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
      {
        List<IndexingUnitChangeEventDetails> changeEventDetailsList = this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetails(-1, filters)));
        if (changeEventDetailsList.Count == 0)
          return (IndexingUnitChangeEvent) null;
        return changeEventDetailsList.Count <= 1 ? changeEventDetailsList[0].IndexingUnitChangeEvent : throw new DataAccessException(DataAccessErrorCodeEnum.UNEXPECTED_ERROR, (Exception) new InvalidOperationException("More than one Indexing Unit Change event found with Id " + id.ToString()));
      }
    }

    public IndexingUnitChangeEvent AddIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      this.ValidateNotNull<IndexingUnitChangeEvent>(nameof (indexingUnitChangeEvent), indexingUnitChangeEvent);
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return component.Insert(indexingUnitChangeEvent);
    }

    public IList<IndexingUnitChangeEvent> AddIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IList<IndexingUnitChangeEvent> indexingUnitChangeEvents)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnitChangeEvent>(nameof (indexingUnitChangeEvents), indexingUnitChangeEvents);
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return (IList<IndexingUnitChangeEvent>) component.AddTableEntityBatch(indexingUnitChangeEvents.ToList<IndexingUnitChangeEvent>(), false);
    }

    public IndexingUnitChangeEvent UpdateIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      this.ValidateNotNull<IndexingUnitChangeEvent>(nameof (indexingUnitChangeEvent), indexingUnitChangeEvent);
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return this.InvokeTableOperation<IndexingUnitChangeEvent>((Func<IndexingUnitChangeEvent>) (() => component.Update(indexingUnitChangeEvent)));
    }

    public IList<IndexingUnitChangeEvent> UpdateIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IList<IndexingUnitChangeEvent> indexingUnitChangeEvents)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnitChangeEvent>(nameof (indexingUnitChangeEvents), indexingUnitChangeEvents);
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return this.InvokeTableOperation<IList<IndexingUnitChangeEvent>>((Func<IList<IndexingUnitChangeEvent>>) (() => component.Update(indexingUnitChangeEvents)));
    }

    public void DeleteIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      this.ValidateNotNull<IndexingUnitChangeEvent>(nameof (indexingUnitChangeEvent), indexingUnitChangeEvent);
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        this.InvokeTableOperation<IndexingUnitChangeEvent>((Func<IndexingUnitChangeEvent>) (() =>
        {
          component.Delete(indexingUnitChangeEvent);
          return (IndexingUnitChangeEvent) null;
        }));
    }

    public void DeleteIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      List<IndexingUnitChangeEvent> indexingUnitChangeEventList)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnitChangeEvent>(nameof (indexingUnitChangeEventList), (IList<IndexingUnitChangeEvent>) indexingUnitChangeEventList);
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        this.InvokeTableOperation<List<IndexingUnitChangeEvent>>((Func<List<IndexingUnitChangeEvent>>) (() =>
        {
          component.DeleteIndexingUnitChangeEventBatch(indexingUnitChangeEventList);
          return (List<IndexingUnitChangeEvent>) null;
        }));
    }

    public void ArchiveIndexingUnitChangeEvent(
      IVssRequestContext requestContext,
      int periodicCleanupOldEvents)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        this.InvokeTableOperation<IndexingUnitChangeEvent>((Func<IndexingUnitChangeEvent>) (() =>
        {
          component.ArchiveCompletedIndexingUnitChangeEvents(periodicCleanupOldEvents);
          return (IndexingUnitChangeEvent) null;
        }));
    }

    public IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IndexingUnitChangeEventState state,
      int topCount)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return (IEnumerable<IndexingUnitChangeEventDetails>) this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetailsByState(topCount, state)));
    }

    public IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      IndexingUnitChangeEventState state,
      int topCount)
    {
      if (indexingUnitId <= 0)
      {
        using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
          return (IEnumerable<IndexingUnitChangeEventDetails>) this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetailsByState(topCount, state)));
      }
      else
      {
        using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
          return (IEnumerable<IndexingUnitChangeEventDetails>) this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetailsByIndexingUnitAndState(topCount, indexingUnitId, state)));
      }
    }

    public int GetCountOfIndexingUnitChangeEventsInProgressOrQueued(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return component.GetCountOfIndexingUnitChangeEventsInProgressOrQueued(entityType);
    }

    public IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string changeType,
      int topCount)
    {
      TableEntityFilterList entityFilterList = new TableEntityFilterList();
      entityFilterList.Add(new TableEntityFilter("IndexingUnitId", "eq", indexingUnitId.ToString()));
      TableEntityFilterList filters = entityFilterList;
      filters.Add(new TableEntityFilter("ChangeType", "eq", changeType));
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return (IEnumerable<IndexingUnitChangeEventDetails>) this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetails(topCount, filters)));
    }

    public IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string changeType,
      IndexingUnitChangeEventState state,
      int topCount)
    {
      TableEntityFilterList entityFilterList = new TableEntityFilterList();
      entityFilterList.Add(new TableEntityFilter("IndexingUnitId", "eq", indexingUnitId.ToString()));
      TableEntityFilterList filters = entityFilterList;
      filters.Add(new TableEntityFilter("ChangeType", "eq", changeType));
      filters.Add(new TableEntityFilter("State", "eq", state.ToString()));
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return (IEnumerable<IndexingUnitChangeEventDetails>) this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetails(topCount, filters)));
    }

    public IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      List<string> changeTypeList,
      List<IndexingUnitChangeEventState> stateList,
      int topCount)
    {
      TableEntityFilterList filters = new TableEntityFilterList();
      if (changeTypeList != null)
      {
        filters.Add(new TableEntityFilter("ChangeType", "eq", (string) null, (object) changeTypeList));
      }
      else
      {
        PriorityManagerService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<PriorityManagerService>();
        filters.Add(new TableEntityFilter("ChangeType", "eq", (string) null, (object) service.GetAllChangeTypes()));
      }
      if (stateList != null)
        filters.Add(new TableEntityFilter("State", "eq", (string) null, (object) stateList));
      else
        filters.Add(new TableEntityFilter("State", "eq", (string) null, (object) Enum.GetValues(typeof (IndexingUnitChangeEventState)).Cast<IndexingUnitChangeEventState>().ToList<IndexingUnitChangeEventState>()));
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return (IEnumerable<IndexingUnitChangeEventDetails>) this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetails(topCount, filters, true)));
    }

    public List<Tuple<IEntityType, string, int>> GetIndexingOperationsInProgress(
      IVssRequestContext requestContext,
      List<string> changeTypeList,
      List<int> jobTriggers)
    {
      if (changeTypeList == null || changeTypeList.Count == 0)
        throw new ArgumentException("changeTypeList can not be null or Empty.");
      if (jobTriggers == null || jobTriggers.Count == 0)
        throw new ArgumentException("jobTriggers can not be null or Empty.");
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return component.GetindexingOperationsInProgress(changeTypeList, jobTriggers);
    }

    public List<Tuple<IEntityType, int, string, int, ChangeEventData>> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      List<string> changeTypeList,
      List<int> jobTriggers,
      List<IndexingUnitChangeEventState> stateList)
    {
      if (changeTypeList == null || changeTypeList.Count == 0)
        throw new ArgumentException("changeTypeList can not be null or Empty.");
      if (jobTriggers == null || jobTriggers.Count == 0)
        throw new ArgumentException("jobTriggers can not be null or Empty.");
      if (stateList == null || stateList.Count == 0)
        throw new ArgumentException("stateList can not be null or Empty.");
      using (IndexingUnitChangeEventComponentV11 component = requestContext.CreateComponent<IndexingUnitChangeEventComponentV11>())
        return component.RetrieveChangeEventColumnsForJobTriggerAndState(changeTypeList, jobTriggers, stateList);
    }

    public IEnumerable<IndexingUnitChangeEventDetails> GetIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      int indexingUnitId,
      List<string> changeTypeList,
      List<IndexingUnitChangeEventState> stateList,
      int topCount)
    {
      TableEntityFilterList entityFilterList = new TableEntityFilterList();
      entityFilterList.Add(new TableEntityFilter("IndexingUnitId", "eq", indexingUnitId.ToString()));
      TableEntityFilterList filters = entityFilterList;
      if (changeTypeList != null)
      {
        filters.Add(new TableEntityFilter("ChangeType", "eq", (string) null, (object) changeTypeList));
      }
      else
      {
        PriorityManagerService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<PriorityManagerService>();
        filters.Add(new TableEntityFilter("ChangeType", "eq", (string) null, (object) service.GetAllChangeTypes()));
      }
      if (stateList != null)
        filters.Add(new TableEntityFilter("State", "eq", (string) null, (object) stateList));
      else
        filters.Add(new TableEntityFilter("State", "eq", (string) null, (object) Enum.GetValues(typeof (IndexingUnitChangeEventState)).Cast<IndexingUnitChangeEventState>().ToList<IndexingUnitChangeEventState>()));
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return (IEnumerable<IndexingUnitChangeEventDetails>) this.InvokeTableOperation<List<IndexingUnitChangeEventDetails>>((Func<List<IndexingUnitChangeEventDetails>>) (() => component.RetrieveIndexingUnitChangeEventDetails(topCount, filters, true)));
    }

    public int GetCountOfIndexingUnitChangeEvents(IVssRequestContext requestContext)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return this.InvokeTableOperation<int>((Func<int>) (() => component.GetCountOfIndexingUnitChangeEvents()));
    }

    public int GetCountOfIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IndexingUnitChangeEventState state)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return this.InvokeTableOperation<int>((Func<int>) (() => component.GetCountOfIndexingUnitChangeEvents(state)));
    }

    public int GetCountOfIndexingUnitChangeEvents(
      IVssRequestContext requestContext,
      IndexingUnitChangeEventState state,
      int numberOfDays)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
      {
        IndexingUnitChangeEventComponentV10 componentV10 = component as IndexingUnitChangeEventComponentV10;
        if (componentV10 != null)
          return this.InvokeTableOperation<int>((Func<int>) (() => componentV10.GetCountOfIndexingUnitChangeEvents(state, numberOfDays)));
      }
      return 0;
    }

    public IDictionary<string, TimeSpan> GetMaxTimeForEachChangeType(
      IVssRequestContext requestContext)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return this.InvokeTableOperation<IDictionary<string, TimeSpan>>((Func<IDictionary<string, TimeSpan>>) (() => component.GetMaxTimeForEachChangeType()));
    }

    public int DeleteIndexingUnitChangeEventsOfDeletedUnits(IVssRequestContext requestContext)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return this.InvokeTableOperation<int>((Func<int>) (() => component.DeleteEventsOfDeletedIndexingUnits()));
    }

    public IndexingUnitChangeEvent AddNewEventIfNotAlreadyPresent(
      IVssRequestContext requestContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      out bool newEventAdded)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return component.AddNewEventIfNotAlreadyPresent(indexingUnitChangeEvent, out newEventAdded);
    }

    public virtual List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> GetChangeEventsCountOfAChangeTypeAndState(
      IVssRequestContext requestContext)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
      {
        IndexingUnitChangeEventComponentV8 eventComponentV8 = (IndexingUnitChangeEventComponentV8) component;
        if (eventComponentV8 != null)
          return eventComponentV8.GetChangeEventsCountOfAChangeTypeAndState();
      }
      return new List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>>();
    }

    public LockStatus AcquireLockAndMarkEventsAsQueued(
      IVssRequestContext requestContext,
      IList<LockDetails> lockingRequirements,
      IEnumerable<IndexingUnitChangeEvent> changeEvents)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return this.InvokeTableOperation<LockStatus>((Func<LockStatus>) (() => component.AcquireLockAndMarkEventsAsQueued(lockingRequirements, changeEvents)));
    }

    public void ReleaseLocksOfCompletedEvents(IVssRequestContext requestContext)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        this.InvokeTableOperation<IndexingUnitChangeEvent>((Func<IndexingUnitChangeEvent>) (() =>
        {
          component.ReleaseLocksOfCompletedEvents();
          return (IndexingUnitChangeEvent) null;
        }));
    }

    public IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      IVssRequestContext requestContext,
      int count,
      int indexingUnitId)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return indexingUnitId <= 0 ? (component as IndexingUnitChangeEventComponentV5).GetNextSetOfEventsToProcess(count) : (component as IndexingUnitChangeEventComponentV5).GetNextSetOfEventsToProcess(count, indexingUnitId);
    }

    public IEnumerable<IndexingUnitChangeEventDetailsWithEntityType> GetNextSetOfEventsToProcess(
      IVssRequestContext requestContext,
      int count,
      IEntityType entityType)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return component.GetNextSetOfEventsToProcess(count, entityType);
    }

    public IDictionary<string, int> GetCountOfEventsBreachingThreshold(
      IVssRequestContext requestContext,
      int bulkIndexThreshold,
      int updateIndexThreshold)
    {
      using (IndexingUnitChangeEventComponent component = requestContext.CreateComponent<IndexingUnitChangeEventComponent>())
        return component.GetCountOfEventsBreachingThreshold(bulkIndexThreshold, updateIndexThreshold);
    }
  }
}
