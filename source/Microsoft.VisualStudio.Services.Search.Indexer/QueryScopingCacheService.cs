// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.QueryScopingCacheService
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public abstract class QueryScopingCacheService
  {
    internal IQueryScopingCache m_queryScopingCache;
    private object m_lock = new object();
    private TeamFoundationTask m_cacheRoutingTask;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        this.InitializeQueryScopingCache(requestContext);
        this.RegisterForSQLNotification(requestContext);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083020, "Common", "DocumentContractTypeService", ex);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<IndexInfo> GetScopedQueryIndexInfo(
      IEntityType entityType,
      EntitySearchQuery searchQuery,
      IVssRequestContext requestContext)
    {
      if (entityType.Name != "Code")
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is not supported by this method GetScopedQueryIndexInfo", (object) entityType.Name)));
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("SummarizedHitCountNeeded", (object) searchQuery?.SummarizedHitCountsNeeded);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi("Query Pipeline", "Query Pipeline", properties);
      if (this.IsScopedQuery(searchQuery))
      {
        IEnumerable<IndexInfo> scopedIndexInfos = this.GetScopedIndexInfos(requestContext, searchQuery);
        if (scopedIndexInfos != null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("IsScopingUsed", "Query Pipeline", 1.0);
          return scopedIndexInfos;
        }
      }
      return requestContext.GetService<IDocumentContractTypeService>().GetQueryIndexInfo(entityType);
    }

    private void CreateQueryScopingCache(IVssRequestContext requestContext)
    {
      this.m_queryScopingCache.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheCreationRunning;
      lock (this.m_lock)
      {
        this.RemoveQueryScopingCacheCreationTask(requestContext);
        this.QueueQueryScopingCacheCreationTask(requestContext);
      }
    }

    private void QueueQueryScopingCacheCreationTask(IVssRequestContext requestContext)
    {
      ITeamFoundationTaskService service = IVssRequestContextExtensions.ElevateAsNeeded(requestContext).GetService<ITeamFoundationTaskService>();
      this.m_cacheRoutingTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.QueryScopingCacheTeamFoundationTaskCallback), (object) null, DateTime.UtcNow, this.GetCacheRefreshInterValMilliSeconds(requestContext));
      IVssRequestContext requestContext1 = requestContext;
      TeamFoundationTask cacheRoutingTask = this.m_cacheRoutingTask;
      service.AddTask(requestContext1, cacheRoutingTask);
    }

    protected virtual int GetCacheRefreshInterValMilliSeconds(IVssRequestContext requestContext)
    {
      int currentHostConfigValue = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/RoutingCacheRefreshInterval");
      return (currentHostConfigValue < 0 ? 0 : currentHostConfigValue) * 60000;
    }

    private void RemoveQueryScopingCacheCreationTask(IVssRequestContext requestContext)
    {
      if (this.m_cacheRoutingTask == null)
        return;
      IVssRequestContextExtensions.ElevateAsNeeded(requestContext).GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, this.m_cacheRoutingTask);
      this.m_cacheRoutingTask = (TeamFoundationTask) null;
    }

    private void InitializeQueryScopingCache(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081464, "Query Pipeline", "DocumentContractTypeService", nameof (InitializeQueryScopingCache));
      this.CreateQueryScopingCache(requestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081465, "Query Pipeline", "DocumentContractTypeService", nameof (InitializeQueryScopingCache));
    }

    protected void QueryScopingCacheTeamFoundationTaskCallback(
      IVssRequestContext requestContext,
      object args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081464, "Query Pipeline", "DocumentContractTypeService", nameof (QueryScopingCacheTeamFoundationTaskCallback));
      try
      {
        this.m_queryScopingCache.CreateQueryScopingCache(requestContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081465, "Query Pipeline", "DocumentContractTypeService", nameof (QueryScopingCacheTeamFoundationTaskCallback));
      }
    }

    [Info("InternalForTestPurpose")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal virtual bool IsScopedQuery(EntitySearchQuery searchQuery) => searchQuery != null && !searchQuery.SummarizedHitCountsNeeded && this.m_queryScopingCache.QueryScopingCacheStatus == QueryScopingCacheStatus.Cached;

    public virtual QueryingUnit GetCacheRoot() => this.m_queryScopingCache.QueryScopingCacheStatus != QueryScopingCacheStatus.Cached ? (QueryingUnit) null : this.m_queryScopingCache.GetCacheRoot();

    public virtual IQueryScopingCacheUpdaterMapper GetUpdaterMapper(IEntityType entityType)
    {
      if (entityType.Name == "Code")
        return (IQueryScopingCacheUpdaterMapper) new CodeQueryScopingCacheUpdaterMapper();
      throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find the Mapper class for Indexing Unit Kind: {0}", (object) entityType.Name)));
    }

    protected abstract void RegisterForSQLNotification(IVssRequestContext requestContext);

    protected abstract void DeregisterSQLNotification(IVssRequestContext requestContext);

    protected abstract IEnumerable<IndexInfo> GetScopedIndexInfos(
      IVssRequestContext requestContext,
      EntitySearchQuery searchQuery);

    public void ServiceEnd(IVssRequestContext requestContext) => this.DeregisterSQLNotification(requestContext);
  }
}
