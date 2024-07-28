// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.ReindexingStatusService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  internal class ReindexingStatusService : IReindexingStatusService, IVssFrameworkService
  {
    private readonly List<EntityType> m_supportedEntityTypes;
    private HashSet<EntityType> m_entityTypesUnderReindexing;
    private IDataAccessFactory m_dataAccessFactoryInstance;
    internal TeamFoundationTask m_calculateReindexingStatusTask;

    public ReindexingStatusService()
      : this(DataAccessFactory.GetInstance())
    {
    }

    internal ReindexingStatusService(IDataAccessFactory dataAccessFactory)
    {
      this.m_dataAccessFactoryInstance = dataAccessFactory;
      this.m_supportedEntityTypes = new List<EntityType>()
      {
        (EntityType) CodeEntityType.GetInstance(),
        (EntityType) WorkItemEntityType.GetInstance()
      };
      this.m_entityTypesUnderReindexing = new HashSet<EntityType>();
    }

    public bool CanFinalize(IVssRequestContext requestContext, IEntityType entityType)
    {
      if (this.m_calculateReindexingStatusTask == null)
        this.CalculateReindexingStatusAndAddRefreshTask(requestContext);
      return requestContext.GetCurrentHostConfigValue<bool>(this.GetFinalizationRegistrySettingNameForEntity(entityType));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.CalculateReindexingStatusAndAddRefreshTask(systemRequestContext);
    }

    private void CalculateReindexingStatusAndAddRefreshTask(IVssRequestContext systemRequestContext)
    {
      this.m_calculateReindexingStatusTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.CalculateReindexingStatusCallback), (object) null, (int) TimeSpan.FromSeconds((double) systemRequestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/ReindexingStatusServiceRefreshIntervalInSec", 600)).TotalMilliseconds);
      IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        this.CalculateReindexingStatus(systemRequestContext);
        context.GetService<ITeamFoundationTaskService>().AddTask(systemRequestContext, this.m_calculateReindexingStatusTask);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083144, "Indexing Pipeline", "Indexer", ex);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_calculateReindexingStatusTask != null)
        context.GetService<ITeamFoundationTaskService>().RemoveTask(systemRequestContext, this.m_calculateReindexingStatusTask);
      this.m_calculateReindexingStatusTask = (TeamFoundationTask) null;
      this.CleanUpShadowReindexingRegistrySettings(systemRequestContext);
    }

    private void CleanUpShadowReindexingRegistrySettings(IVssRequestContext collectionRequestContext)
    {
      IVssRegistryService service = collectionRequestContext.GetService<IVssRegistryService>();
      service.SetValue<bool>(collectionRequestContext, "/Service/ALMSearch/Settings/SuspendCodeIndexingOnPrimary", false);
      service.SetValue<bool>(collectionRequestContext, "/Service/ALMSearch/Settings/CanFinalizeCodeIndex", false);
      service.SetValue<bool>(collectionRequestContext, "/Service/ALMSearch/Settings/SuspendWorkItemIndexingOnPrimary", false);
      service.SetValue<bool>(collectionRequestContext, "/Service/ALMSearch/Settings/CanFinalizeWorkItemIndex", false);
    }

    internal void CalculateReindexingStatusCallback(
      IVssRequestContext collectionRequestContext,
      object taskArgs)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(collectionRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083145, "Indexing Pipeline", "Indexer", nameof (CalculateReindexingStatusCallback));
      try
      {
        this.CalculateReindexingStatus(collectionRequestContext);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083144, "Indexing Pipeline", "Indexer", ex);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083146, "Indexing Pipeline", "Indexer", nameof (CalculateReindexingStatusCallback));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    internal void CalculateReindexingStatus(IVssRequestContext collectionRequestContext)
    {
      this.InitializeEntityTypesUnderReindexing(collectionRequestContext);
      if (this.m_entityTypesUnderReindexing.Count == 0)
        throw new SearchServiceException("ReindexingStatusService is running when no entity type is under reindexing.");
      IIndexingUnitDataAccess indexingUnitDataAccess1 = this.m_dataAccessFactoryInstance.GetIndexingUnitDataAccess();
      bool flag1 = false;
      foreach (EntityType entityType in this.m_entityTypesUnderReindexing)
      {
        IReindexingStatusEvaluator indexingStatusEvaluator = this.GetIndexingStatusEvaluator((IEntityType) entityType);
        IVssRegistryService service = collectionRequestContext.GetService<IVssRegistryService>();
        IVssRequestContext requestContext = collectionRequestContext;
        IIndexingUnitDataAccess indexingUnitDataAccess2 = indexingUnitDataAccess1;
        bool flag2 = indexingStatusEvaluator.Evaluate(requestContext, indexingUnitDataAccess2);
        service.SetValue<bool>(collectionRequestContext, this.GetFinalizationRegistrySettingNameForEntity((IEntityType) entityType), flag2);
        flag1 |= flag2;
      }
      if (!flag1)
        return;
      collectionRequestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(collectionRequestContext, (IEnumerable<Guid>) new Guid[1]
      {
        JobConstants.PeriodicMaintenanceJobId
      });
    }

    private void InitializeEntityTypesUnderReindexing(IVssRequestContext collectionRequestContext)
    {
      IReindexingStatusDataAccess statusDataAccess = this.m_dataAccessFactoryInstance.GetReindexingStatusDataAccess();
      this.m_entityTypesUnderReindexing.Clear();
      foreach (EntityType supportedEntityType in this.m_supportedEntityTypes)
      {
        ReindexingStatusEntry reindexingStatusEntry = statusDataAccess.GetReindexingStatusEntry(collectionRequestContext.To(TeamFoundationHostType.Deployment), collectionRequestContext.GetCollectionID(), (IEntityType) supportedEntityType);
        if ((reindexingStatusEntry != null ? (reindexingStatusEntry.IsReindexingFailedOrInProgress() ? 1 : 0) : 0) != 0)
          this.m_entityTypesUnderReindexing.Add(supportedEntityType);
      }
    }

    private IReindexingStatusEvaluator GetIndexingStatusEvaluator(IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return (IReindexingStatusEvaluator) new CodeReindexingStatusEvaluator();
        case "WorkItem":
          return (IReindexingStatusEvaluator) new WorkItemReindexingStatusEvaluator();
        default:
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType {0} is not supported for Zero staleness reindexing.", (object) entityType.Name)));
      }
    }

    private string GetFinalizationRegistrySettingNameForEntity(IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return "/Service/ALMSearch/Settings/CanFinalizeCodeIndex";
        case "WorkItem":
          return "/Service/ALMSearch/Settings/CanFinalizeWorkItemIndex";
        default:
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Entity type {0} is not supported for Zero staleness reindexing.", (object) entityType.Name)));
      }
    }
  }
}
