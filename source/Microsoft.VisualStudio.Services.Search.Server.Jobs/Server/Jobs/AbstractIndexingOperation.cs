// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractIndexingOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal abstract class AbstractIndexingOperation : IRunnable<OperationResult>
  {
    protected const string TraceArea = "Indexing Pipeline";
    protected const string TraceLayer = "IndexingOperation";
    protected internal readonly int m_maxIndexRetryCount;
    private IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private IIndexingUnitChangeEventDataAccess m_indexingUnitChangeEventDataAccess;

    protected AbstractIndexingOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    protected AbstractIndexingOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory)
    {
      this.IndexingUnit = indexingUnit;
      this.IndexingUnitChangeEvent = indexingUnitChangeEvent;
      this.DataAccessFactory = dataAccessFactory;
      this.IndexingUnitChangeEventHandler = (executionContext is CoreIndexingExecutionContext executionContext1 ? executionContext1.IndexingUnitChangeEventHandler : (IIndexingUnitChangeEventHandler) null) ?? (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler();
      this.m_maxIndexRetryCount = this.GetMaxIndexingRetryCount(executionContext);
    }

    public abstract OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext);

    protected internal virtual IndexingUnit IndexingUnit { get; set; }

    protected internal IndexingUnitChangeEvent IndexingUnitChangeEvent { get; set; }

    protected internal virtual IIndexingUnitDataAccess IndexingUnitDataAccess
    {
      get
      {
        if (this.m_indexingUnitDataAccess == null)
          this.m_indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
        return this.m_indexingUnitDataAccess;
      }
      set => this.m_indexingUnitDataAccess = value;
    }

    protected IIndexingUnitChangeEventDataAccess IndexingUnitChangeEventDataAccess
    {
      get
      {
        if (this.m_indexingUnitChangeEventDataAccess == null)
          this.m_indexingUnitChangeEventDataAccess = this.DataAccessFactory.GetIndexingUnitChangeEventDataAccess();
        return this.m_indexingUnitChangeEventDataAccess;
      }
      set => this.m_indexingUnitChangeEventDataAccess = value;
    }

    protected internal virtual IIndexingUnitChangeEventHandler IndexingUnitChangeEventHandler { get; set; }

    protected internal IDataAccessFactory DataAccessFactory { get; set; }

    public OperationResult Run(ExecutionContext executionContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) null;
      OperationResult result = new OperationResult();
      try
      {
        indexingExecutionContext = executionContext as IndexingExecutionContext;
        if ((int) this.IndexingUnitChangeEvent.AttemptCount > this.m_maxIndexRetryCount)
        {
          result.Status = OperationStatus.Failed;
          result.Message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}: The operation stopped at pre-check for retry limits. Retry Limit : {2}, Current attempt count : {3}", (object) this.IndexingUnitChangeEvent.ChangeType, (object) this.IndexingUnitChangeEvent.ChangeData.Trigger, (object) this.m_maxIndexRetryCount, (object) this.IndexingUnitChangeEvent.AttemptCount);
        }
        else
        {
          this.PreRun((CoreIndexingExecutionContext) indexingExecutionContext);
          string reasonToSkipOperation;
          if (this.ShouldSkipOperation(executionContext.RequestContext, out reasonToSkipOperation))
          {
            result.Status = OperationStatus.Succeeded;
            result.Message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Skipping Operation. Reason: [{0}]", (object) reasonToSkipOperation);
          }
          else
            result = this.RunOperation((CoreIndexingExecutionContext) indexingExecutionContext);
          result.Message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}: {2}", (object) this.IndexingUnitChangeEvent.ChangeType, (object) this.IndexingUnitChangeEvent.ChangeData.Trigger, (object) result.Message);
          indexingExecutionContext.ExecutionTracerContext.PublishClientTrace("Indexing Pipeline", "IndexingOperation", "OperationStatus", (object) result.Status.ToString(), true);
        }
        this.PostRun((CoreIndexingExecutionContext) indexingExecutionContext, result);
      }
      catch (Exception ex)
      {
        this.HandleOperationFailure(indexingExecutionContext, result, ex);
        this.PostRun((CoreIndexingExecutionContext) indexingExecutionContext, result, ex);
      }
      finally
      {
        stopwatch.Stop();
        this.PublishToHostedAndOnPremise(executionContext.ExecutionTracerContext, this.IndexingUnit.EntityType, this.IndexingUnit.IndexingUnitType, this.IndexingUnitChangeEvent.ChangeType, stopwatch.ElapsedMilliseconds);
      }
      return result;
    }

    internal virtual void PopulateRoutingCacheUpdateData(
      QueryScopingCacheUpdateData queryScopingCacheUpdateData,
      string oldEntityName)
    {
      queryScopingCacheUpdateData.OldEntityName = oldEntityName;
      queryScopingCacheUpdateData.NewEntityName = this.IndexingUnit.Properties.Name;
      queryScopingCacheUpdateData.TfsEntityId = this.IndexingUnit.TFSEntityId;
      queryScopingCacheUpdateData.parentUnitId = this.IndexingUnit.ParentUnitId;
      queryScopingCacheUpdateData.EntityType = this.IndexingUnit.EntityType;
    }

    internal virtual void PreRun(CoreIndexingExecutionContext executionContext) => this.UpdateChangeEventStateOnStart(executionContext);

    internal virtual bool ShouldSkipOperation(
      IVssRequestContext requestContext,
      out string reasonToSkipOperation)
    {
      return this.ShouldSkipOperationDueToReindexing(requestContext, out reasonToSkipOperation);
    }

    internal virtual void PostRun(
      CoreIndexingExecutionContext executionContext,
      OperationResult result,
      Exception e = null)
    {
      if (executionContext == null)
        return;
      try
      {
        if (result.Status != OperationStatus.Succeeded)
        {
          if (result.Status != OperationStatus.PartiallySucceeded)
            goto label_7;
        }
        executionContext.FaultService.ResetError((Exception) null);
      }
      catch (Exception ex)
      {
        result.Message += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FaultService.ResetError for IndexingUnitChangeEvent.Id: {0} and IndexingUnitId: {1} failed with error {2}", (object) this.IndexingUnitChangeEvent.Id, (object) this.IndexingUnit.IndexingUnitId, (object) ex);
      }
label_7:
      try
      {
        this.UpdateChangeEventStateOnEnd(executionContext, result, e);
      }
      catch (Exception ex)
      {
        result.Message += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Update State for IndexingUnitChangeEvent with Id: {0} for IndexingUnitId: {1} failed with error {2}", (object) this.IndexingUnitChangeEvent.Id, (object) this.IndexingUnit.IndexingUnitId, (object) ex);
      }
      if (string.IsNullOrEmpty(executionContext.Log.Content))
        return;
      result.Message += executionContext.Log.Content;
    }

    protected bool ShouldSkipOperationDueToReindexing(
      IVssRequestContext requestContext,
      out string reasonToSkip)
    {
      reasonToSkip = string.Empty;
      if (this.IndexingUnit.IsShadow || !this.IsZeroStalenessReindexingFFEnabledAndIndexingOnPrimarySuspended(requestContext))
        return false;
      reasonToSkip = "Indexing Operations on Primary Indexing Unit is suspended due to ReIndexing.";
      return true;
    }

    protected virtual int GetMaxIndexingRetryCount(ExecutionContext executionContext) => executionContext.ServiceSettings.JobSettings.MaxIndexingRetryCount;

    protected internal virtual bool ValidateCIFeatureFlags(IndexingExecutionContext iexContext)
    {
      IVssRequestContext requestContext = iexContext.RequestContext;
      IEntityType entityType = iexContext.IndexingUnit.EntityType;
      if (this.IsOnPremisesDeployment(requestContext) || requestContext.IsContinuousIndexingEnabled(entityType))
        return true;
      iexContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Either Indexing or CI is disabled, bailing out. Indexing Feature Flag: {0}, Crud Feature Flag: {1}", (object) iexContext.IsIndexingEnabled(), (object) iexContext.IsCrudOperationsFeatureEnabled())));
      return false;
    }

    protected internal virtual bool IsOnPremisesDeployment(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    protected internal virtual void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception e)
    {
      if (indexingExecutionContext.OperationFailureHandler != null && indexingExecutionContext.OperationFailureHandler.HandleError((CoreIndexingExecutionContext) indexingExecutionContext, e))
      {
        result.Status = OperationStatus.Succeeded;
      }
      else
      {
        result.Status = (int) this.IndexingUnitChangeEvent.AttemptCount < this.m_maxIndexRetryCount ? OperationStatus.FailedAndRetry : OperationStatus.Failed;
        string empty = string.Empty;
        if (indexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          empty = e.GetBaseException().ToString();
        string str = empty + e.ToString();
        result.Message += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}: The operation did not complete successfully because of exception {2}.", (object) this.IndexingUnitChangeEvent.ChangeType, (object) this.IndexingUnitChangeEvent.ChangeData.Trigger, (object) str);
      }
    }

    protected internal virtual TimeSpan GetChangeEventDelay(
      CoreIndexingExecutionContext executionContext,
      Exception e)
    {
      return e == null ? new TimeSpan(0L) : TimeSpan.FromSeconds(IndexFaultMapManager.GetFaultMapper(typeof (VssThrottlingFaultMapper)).IsMatch(e) || IndexFaultMapManager.GetFaultMapper(typeof (VssConcurrencyLimitFaultMapper)).IsMatch(e) ? executionContext.RequestContext.GetCurrentHostConfigValue<double>("/Service/ALMSearch/Settings/ChangeEventDelayForThrottlingFailureInSeconds", true, 900.0) : executionContext.RequestContext.GetCurrentHostConfigValue<double>("/Service/ALMSearch/Settings/ChangeEventDelayForJobFailureInSeconds", true, 600.0));
    }

    private void PublishToHostedAndOnPremise(
      ExecutionTracerContext executionTraceContext,
      IEntityType entityType,
      string indexingUnitType,
      string changeType,
      long elapsedTime)
    {
      string kpiName = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}.{2}", (object) entityType.Name, (object) indexingUnitType, (object) changeType));
      executionTraceContext.PublishKpi(kpiName, "Indexing Pipeline", (double) elapsedTime);
      executionTraceContext.PublishCi("Indexing Pipeline", "Indexing Pipeline", "EntityType", entityType.Name, true);
      executionTraceContext.PublishCi("Indexing Pipeline", "Indexing Pipeline", "IndexingUnitType", indexingUnitType, true);
      executionTraceContext.PublishCi("Indexing Pipeline", "Indexing Pipeline", "ChangeType", changeType, true);
      executionTraceContext.PublishCi("Indexing Pipeline", "Indexing Pipeline", "JobExecutionTime", elapsedTime.ToString((IFormatProvider) CultureInfo.InvariantCulture), true);
    }

    private void UpdateChangeEventStateOnStart(CoreIndexingExecutionContext executionContext)
    {
      this.IndexingUnitChangeEvent.State = IndexingUnitChangeEventState.InProgress;
      ++this.IndexingUnitChangeEvent.AttemptCount;
      this.IndexingUnitChangeEventDataAccess.UpdateIndexingUnitChangeEvent(executionContext.RequestContext, this.IndexingUnitChangeEvent);
    }

    private void UpdateChangeEventStateOnEnd(
      CoreIndexingExecutionContext executionContext,
      OperationResult result,
      Exception e)
    {
      this.IndexingUnitChangeEvent.State = this.GetIndexingUnitChangeEventState(result.Status);
      if (this.IndexingUnitChangeEvent.State == IndexingUnitChangeEventState.FailedAndRetry || this.IndexingUnitChangeEvent.State == IndexingUnitChangeEventState.PartiallySucceeded)
        this.IndexingUnitChangeEvent.ChangeData.Delay = this.GetChangeEventDelay(executionContext, e);
      this.IndexingUnitChangeEventDataAccess.UpdateIndexingUnitChangeEvent(executionContext.RequestContext, this.IndexingUnitChangeEvent);
    }

    private IndexingUnitChangeEventState GetIndexingUnitChangeEventState(
      OperationStatus operationStatus)
    {
      switch (operationStatus)
      {
        case OperationStatus.Succeeded:
          return IndexingUnitChangeEventState.Succeeded;
        case OperationStatus.PartiallySucceeded:
          return IndexingUnitChangeEventState.PartiallySucceeded;
        case OperationStatus.FailedAndRetry:
          return IndexingUnitChangeEventState.FailedAndRetry;
        case OperationStatus.Failed:
          return IndexingUnitChangeEventState.Failed;
        default:
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid operation status {0}", (object) operationStatus)));
      }
    }

    private bool IsZeroStalenessReindexingFFEnabledAndIndexingOnPrimarySuspended(
      IVssRequestContext requestContext)
    {
      switch (this.IndexingUnit.EntityType.Name)
      {
        case "WorkItem":
          return requestContext.IsWorkItemReindexingWithZeroStalenessFeatureEnabled() && requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/SuspendWorkItemIndexingOnPrimary");
        case "Code":
          return !(this.IndexingUnit.IndexingUnitType == "CustomRepository") && requestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/SuspendCodeIndexingOnPrimary");
        default:
          return false;
      }
    }
  }
}
