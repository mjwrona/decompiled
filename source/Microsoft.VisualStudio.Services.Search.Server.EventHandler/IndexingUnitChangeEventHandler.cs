// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  public class IndexingUnitChangeEventHandler : IIndexingUnitChangeEventHandler
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "IndexingUnitChangeEventHandler";

    internal IDataAccessFactory DataAccessFactory { get; set; }

    public IndexingUnitChangeEventHandler()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal IndexingUnitChangeEventHandler(IDataAccessFactory dataAccessFactory) => this.DataAccessFactory = dataAccessFactory;

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent HandleEvent(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      return this.HandleEvent(executionContext, indexingUnitChangeEvent, false);
    }

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent HandleEvent(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      bool scopeChangeEventProcessingToIndexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEvent));
      try
      {
        indexingUnitChangeEvent = this.AddEventNotificationsToDB(executionContext, indexingUnitChangeEvent);
        if (scopeChangeEventProcessingToIndexingUnit)
        {
          this.ProcessEvents(executionContext, indexingUnitChangeEvent.IndexingUnitId);
        }
        else
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, indexingUnitChangeEvent.IndexingUnitId);
          if (indexingUnit == null)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("No Indexing Unit was retrieved from DB. For ID:{0}", (object) indexingUnitChangeEvent.IndexingUnitId)));
          else
            this.ProcessEvents(executionContext, indexingUnit.EntityType);
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEvent));
      }
      return indexingUnitChangeEvent;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> HandleEvents(
      ExecutionContext executionContext,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEvents));
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList1;
      try
      {
        if (indexingUnitChangeEventList == null || !indexingUnitChangeEventList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
          return indexingUnitChangeEventList;
        indexingUnitChangeEventList1 = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess().AddIndexingUnitChangeEvents(executionContext.RequestContext, indexingUnitChangeEventList);
        HashSet<int> indexingUnitIds = new HashSet<int>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList1)
          indexingUnitIds.Add(indexingUnitChangeEvent.IndexingUnitId);
        IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnits(executionContext.RequestContext, (IEnumerable<int>) indexingUnitIds);
        if (indexingUnits.Any<KeyValuePair<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>())
        {
          IEntityType entityType = indexingUnits.Values.First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().EntityType;
          bool flag = true;
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits.Values)
          {
            this.AssociateJobIDIfNeeded(executionContext, indexingUnit);
            if (indexingUnit.EntityType.Name != entityType.Name)
              flag = false;
          }
          if (flag)
          {
            this.ProcessEvents(executionContext, entityType);
          }
          else
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Processing Multi Entity Set of Events, which is not expected")));
            this.ProcessEvents(executionContext, -1);
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEvents));
      }
      return indexingUnitChangeEventList1;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> HandleEventsChildIUCEsForFinalize(
      ExecutionContext executionContext,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventsChildIUCEsForFinalize));
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList1;
      try
      {
        if (indexingUnitChangeEventList == null || !indexingUnitChangeEventList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
          return indexingUnitChangeEventList;
        indexingUnitChangeEventList1 = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess().AddIndexingUnitChangeEvents(executionContext.RequestContext, indexingUnitChangeEventList);
        HashSet<int> indexingUnitIds = new HashSet<int>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList1)
          indexingUnitIds.Add(indexingUnitChangeEvent.IndexingUnitId);
        IDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnits(executionContext.RequestContext, (IEnumerable<int>) indexingUnitIds);
        if (indexingUnits.Any<KeyValuePair<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>())
        {
          IEntityType entityType = indexingUnits.Values.First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().EntityType;
          foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits.Values)
            this.AssociateJobIDIfNeeded(executionContext, indexingUnit);
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventsChildIUCEsForFinalize));
      }
      return indexingUnitChangeEventList1;
    }

    public virtual void HandleEventForEntity(
      ExecutionContext executionContext,
      IEntityType entityType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventForEntity));
      try
      {
        this.ProcessEvents(executionContext, entityType);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventForEntity));
      }
    }

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent HandleEventWithAddingEventWhenNeeded(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventWithAddingEventWhenNeeded));
      try
      {
        IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
        bool newEventAdded = false;
        if (this.AssociateJobIDIfNeeded(executionContext, indexingUnitChangeEvent.IndexingUnitId))
        {
          indexingUnitChangeEvent = changeEventDataAccess.AddNewEventIfNotAlreadyPresent(executionContext.RequestContext, indexingUnitChangeEvent, out newEventAdded);
          if (newEventAdded)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Added Indexing Unit Change Event : {0} to DB.", (object) indexingUnitChangeEvent.ToString())));
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, indexingUnitChangeEvent.IndexingUnitId);
            if (indexingUnit != null)
              this.ProcessEvents(executionContext, indexingUnit.EntityType);
            else
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Not processing the event as the indexing unit is unavailable (probably deleted).")));
          }
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Not Queuing new event as there is already an event {0} to process in the DB.", (object) indexingUnitChangeEvent.ToString())));
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Unable to associate any JobId for the given event hence no event is added to the DB")));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventWithAddingEventWhenNeeded));
      }
      return indexingUnitChangeEvent;
    }

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent HandleEventWithAddingEventWhenNeededForFinalize(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventWithAddingEventWhenNeededForFinalize));
      try
      {
        IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
        bool newEventAdded = false;
        if (this.AssociateJobIDIfNeeded(executionContext, indexingUnitChangeEvent.IndexingUnitId))
        {
          indexingUnitChangeEvent = changeEventDataAccess.AddNewEventIfNotAlreadyPresent(executionContext.RequestContext, indexingUnitChangeEvent, out newEventAdded);
          if (newEventAdded)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Added Indexing Unit Change Event : {0} to DB.", (object) indexingUnitChangeEvent.ToString())));
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Not Queuing new event as there is already an event {0} to process in the DB.", (object) indexingUnitChangeEvent.ToString())));
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Unable to associate any JobId for the given event hence no event is added to the DB")));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (HandleEventWithAddingEventWhenNeededForFinalize));
      }
      return indexingUnitChangeEvent;
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent AddEventNotificationsToDB(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (AddEventNotificationsToDB));
      try
      {
        IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
        if (this.AssociateJobIDIfNeeded(executionContext, indexingUnitChangeEvent.IndexingUnitId))
        {
          indexingUnitChangeEvent = changeEventDataAccess.AddIndexingUnitChangeEvent(executionContext.RequestContext, indexingUnitChangeEvent);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Added Indexing Unit Change Event : {0} to DB.", (object) indexingUnitChangeEvent)));
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("No Indexing Unit Change Event was added to DB because we were unable to associate any JobId to the changeEvent.")));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (AddEventNotificationsToDB));
      }
      return indexingUnitChangeEvent;
    }

    internal virtual bool AssociateJobIDIfNeeded(
      ExecutionContext executionContext,
      int indexingUnitId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (AssociateJobIDIfNeeded));
      try
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(executionContext.RequestContext, indexingUnitId);
        return this.AssociateJobIDIfNeeded(executionContext, indexingUnit);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (AssociateJobIDIfNeeded));
      }
    }

    internal virtual void ProcessEvents(ExecutionContext executionContext, int indexingUnitId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (ProcessEvents));
      try
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), "Notifying IndexingUnitChangeEventProcessor to process next set of events.");
        executionContext.RequestContext.GetService<IIndexingUnitChangeEventProcessor>().Process(executionContext, indexingUnitId);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (ProcessEvents));
      }
    }

    internal virtual void ProcessEvents(ExecutionContext executionContext, IEntityType entityType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (ProcessEvents));
      try
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), "Notifying IndexingUnitChangeEventProcessor to process next set of events.");
        executionContext.RequestContext.GetService<IIndexingUnitChangeEventProcessor>().Process(executionContext, entityType);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (ProcessEvents));
      }
    }

    private bool AssociateJobIDIfNeeded(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (AssociateJobIDIfNeeded));
      bool flag = true;
      try
      {
        IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
        if (indexingUnit != null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Retrieved Indexing Unit : {0} from DB.", (object) indexingUnit)));
          Guid? associatedJobId = indexingUnit.AssociatedJobId;
          if (!associatedJobId.HasValue)
          {
            Guid jobDefinition = this.CreateJobDefinition(executionContext.RequestContext, indexingUnit);
            indexingUnit.AssociatedJobId = new Guid?(jobDefinition);
            indexingUnit = indexingUnitDataAccess.AssociateJobId(executionContext.RequestContext, indexingUnit);
            associatedJobId = indexingUnit.AssociatedJobId;
            if (associatedJobId.Value != jobDefinition)
            {
              ITeamFoundationJobService service = executionContext.RequestContext.GetService<ITeamFoundationJobService>();
              try
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Created duplicate JobDefinition for {0} : {1}", (object) indexingUnit, (object) jobDefinition)));
                service.DeleteJobDefinitions(executionContext.RequestContext, (IEnumerable<Guid>) new List<Guid>()
                {
                  jobDefinition
                });
              }
              catch (Exception ex)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Stale Job Definition for {0} created with JobId : {1}", (object) indexingUnit, (object) jobDefinition)));
              }
            }
          }
        }
        else
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), "No Indexing Unit was retrieved from DB. IndexingUnit was null");
          flag = false;
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (AssociateJobIDIfNeeded));
      }
      return flag;
    }

    private Guid CreateJobDefinition(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (CreateJobDefinition));
      Guid jobId = Guid.NewGuid();
      try
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        SearchServiceJobData searchServiceJobData = new SearchServiceJobData(indexingUnit.IndexingUnitId);
        TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId, this.GetJobName(indexingUnit), this.GetJobExtension(indexingUnit), searchServiceJobData.ToXml(), TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.AboveNormal);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), FormattableString.Invariant(FormattableStringFactory.Create("Creating a new Job Definiton with Job ID {0} for Indexing Unit {1}", (object) jobId, (object) indexingUnit.ToString())));
        service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new List<TeamFoundationJobDefinition>()
        {
          foundationJobDefinition
        });
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080521, "Indexing Pipeline", nameof (IndexingUnitChangeEventHandler), nameof (CreateJobDefinition));
      }
      return jobId;
    }

    private string GetJobName(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit) => indexingUnit.IndexingUnitType + indexingUnit.EntityType.Name + "Indexing";

    protected virtual string GetJobExtension(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      switch (indexingUnit.EntityType.Name)
      {
        case "Code":
          string indexingUnitType = indexingUnit.IndexingUnitType;
          if (indexingUnitType != null)
          {
            switch (indexingUnitType.Length)
            {
              case 7:
                if (indexingUnitType == "Project")
                  return "Microsoft.VisualStudio.Services.Search.Server.Jobs.ProjectCodeIndexingJob";
                goto label_26;
              case 10:
                if (indexingUnitType == "Collection")
                  return "Microsoft.VisualStudio.Services.Search.Server.Jobs.CollectionCodeIndexingJob";
                goto label_26;
              case 14:
                if (indexingUnitType == "Git_Repository")
                  break;
                goto label_26;
              case 15:
                if (indexingUnitType == "TFVC_Repository")
                  break;
                goto label_26;
              case 16:
                if (indexingUnitType == "CustomRepository")
                  break;
                goto label_26;
              case 18:
                if (indexingUnitType == "ScopedIndexingUnit")
                  return "Microsoft.VisualStudio.Services.Search.Server.Jobs.ScopedIndexingUnitCodeIndexingJob";
                goto label_26;
              case 21:
                if (indexingUnitType == "TemporaryIndexingUnit")
                  return "Microsoft.VisualStudio.Services.Search.Server.Jobs.TemporaryIndexingUnitCodeIndexingJob";
                goto label_26;
              default:
                goto label_26;
            }
            return "Microsoft.VisualStudio.Services.Search.Server.Jobs.RepositoryCodeIndexingJob";
          }
          break;
        case "WorkItem":
          switch (indexingUnit.IndexingUnitType)
          {
            case "Collection":
              return "Microsoft.VisualStudio.Services.Search.Server.Jobs.CollectionWorkItemIndexingJob";
            case "Project":
              return "Microsoft.VisualStudio.Services.Search.Server.Jobs.ProjectWorkItemIndexingJob";
          }
          break;
        case "Wiki":
          switch (indexingUnit.IndexingUnitType)
          {
            case "Collection":
              return "Microsoft.VisualStudio.Services.Search.Server.Jobs.CollectionWikiIndexingJob";
            case "Project":
              return "Microsoft.VisualStudio.Services.Search.Server.Jobs.ProjectWikiIndexingJob";
            case "Git_Repository":
              return "Microsoft.VisualStudio.Services.Search.Server.Jobs.RepositoryWikiIndexingJob";
          }
          break;
        case "Package":
          if (indexingUnit.IndexingUnitType == "Collection")
            return "Microsoft.VisualStudio.Services.Search.Server.Jobs.CollectionPackageIndexingJob";
          break;
        case "Board":
          if (indexingUnit.IndexingUnitType == "Collection")
            return "Microsoft.VisualStudio.Services.Search.Server.Jobs.CollectionBoardIndexingJob";
          break;
      }
label_26:
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the extension of the Indexing Unit ID : {0}, EntityType: {1}, IndexingUnitType: {2}", (object) indexingUnit.IndexingUnitId, (object) indexingUnit.EntityType, (object) indexingUnit.IndexingUnitType)));
    }
  }
}
