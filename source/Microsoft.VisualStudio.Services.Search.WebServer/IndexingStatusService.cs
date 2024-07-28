// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.IndexingStatusService
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class IndexingStatusService : IIndexingStatusService, IVssFrameworkService, IDisposable
  {
    private readonly IDictionary<IEntityType, IndexingStatusProvider> m_indexingStatusProviders;
    private readonly ConcurrentDictionary<IEntityType, CollectionIndexingStatus> m_indexingStatus;
    private CountdownEvent m_indexingStateRefreshEvent;
    private TeamFoundationTask m_indexingStateRefreshTask;
    private Guid m_collectionId;
    private readonly IDictionary<IEntityType, IndexingStatusDetails> m_indexingStatusDetails;
    private HashSet<string> m_indexingOperations = new HashSet<string>();
    private HashSet<int> m_indexingJobTriggerList = new HashSet<int>();
    private bool disposedValue;

    public IndexingStatusService()
      : this(DataAccessFactory.GetInstance())
    {
    }

    private IndexingStatusService(IDataAccessFactory dataAccessFactory)
    {
      this.m_indexingStatusProviders = (IDictionary<IEntityType, IndexingStatusProvider>) new FriendlyDictionary<IEntityType, IndexingStatusProvider>()
      {
        [(IEntityType) CodeEntityType.GetInstance()] = (IndexingStatusProvider) new CodeIndexingStatusProvider(dataAccessFactory),
        [(IEntityType) WorkItemEntityType.GetInstance()] = (IndexingStatusProvider) new WorkItemIndexingStatusProvider(dataAccessFactory),
        [(IEntityType) WikiEntityType.GetInstance()] = (IndexingStatusProvider) new WikiIndexingStatusProvider(dataAccessFactory),
        [(IEntityType) PackageEntityType.GetInstance()] = (IndexingStatusProvider) new PackageIndexingStatusProvider(dataAccessFactory)
      };
      foreach (KeyValuePair<IEntityType, IndexingStatusProvider> indexingStatusProvider1 in (IEnumerable<KeyValuePair<IEntityType, IndexingStatusProvider>>) this.m_indexingStatusProviders)
      {
        IEntityType key = indexingStatusProvider1.Key;
        IndexingStatusProvider indexingStatusProvider2 = indexingStatusProvider1.Value;
        this.m_indexingOperations.UnionWith((IEnumerable<string>) indexingStatusProvider2.GetSupportedOperations());
        this.m_indexingJobTriggerList.UnionWith((IEnumerable<int>) indexingStatusProvider2.GetSupportedJobTriggers());
      }
      this.m_indexingStatus = new ConcurrentDictionary<IEntityType, CollectionIndexingStatus>();
      this.m_indexingStatusDetails = (IDictionary<IEntityType, IndexingStatusDetails>) new Dictionary<IEntityType, IndexingStatusDetails>();
      foreach (IEntityType key in (IEnumerable<IEntityType>) this.m_indexingStatusProviders.Keys)
      {
        this.m_indexingStatus[key] = CollectionIndexingStatus.NotIndexing;
        this.m_indexingStatusDetails[key] = (IndexingStatusDetails) null;
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_collectionId = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetCollectionID() : throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/IndexingStatusRefreshIntervalInSeconds"));
      this.m_indexingStateRefreshEvent = new CountdownEvent(1);
      this.m_indexingStateRefreshTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshIndexingStateCallback), (object) null, (int) timeSpan.TotalMilliseconds);
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      this.RefreshIndexingState(vssRequestContext);
      if (this.m_indexingStateRefreshTask == null)
        return;
      vssRequestContext.GetService<ITeamFoundationTaskService>().AddTask(vssRequestContext, this.m_indexingStateRefreshTask);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_indexingStateRefreshTask != null)
        vssRequestContext.GetService<ITeamFoundationTaskService>().RemoveTask(vssRequestContext, this.m_indexingStateRefreshTask);
      this.m_indexingStateRefreshEvent.Signal();
      this.m_indexingStateRefreshEvent.Wait();
      this.m_indexingStateRefreshEvent.Dispose();
    }

    public CollectionIndexingStatus GetCollectionIndexingStatus(IEntityType entityType) => this.Supports(entityType) ? this.m_indexingStatus[entityType] : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Entity type [{0}] is not supported yet.", (object) entityType)));

    public bool GetIndexingStatusDetails(
      IVssRequestContext requestContext,
      IEntityType entityType,
      string projectName,
      string repoName,
      string branchName)
    {
      if (this.Supports(entityType))
        return entityType == CodeEntityType.GetInstance() && ((CodeIndexingStatusDetails) this.m_indexingStatusDetails[entityType] ?? new CodeIndexingStatusDetails()).IsBranchIndexing(requestContext, projectName, repoName, branchName, this.m_collectionId.ToString());
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Entity type [{0}] is not supported yet.", (object) entityType)));
    }

    public bool Supports(IEntityType entityType) => this.m_indexingStatusProviders.ContainsKey(entityType);

    private void RefreshIndexingStateCallback(
      IVssRequestContext deploymentRequestContext,
      object taskargs)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(deploymentRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081425, "Query Pipeline", "Query", nameof (RefreshIndexingStateCallback));
      try
      {
        this.m_indexingStateRefreshEvent.AddCount();
        this.RefreshIndexingState(deploymentRequestContext);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081382, "Query Pipeline", "Query", ex);
      }
      finally
      {
        this.m_indexingStateRefreshEvent.Signal();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081303, "Query Pipeline", "Query", nameof (RefreshIndexingStateCallback));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private void RefreshIndexingState(IVssRequestContext deploymentRequestContext)
    {
      using (IVssRequestContext vssRequestContext = deploymentRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(deploymentRequestContext, this.m_collectionId, RequestContextType.SystemContext))
      {
        List<Tuple<IEntityType, string, int>> operationsInProgress = DataAccessFactory.GetInstance().GetIndexingUnitChangeEventDataAccess().GetIndexingOperationsInProgress(vssRequestContext, this.m_indexingOperations.ToList<string>(), this.m_indexingJobTriggerList.ToList<int>());
        foreach (KeyValuePair<IEntityType, IndexingStatusProvider> indexingStatusProvider1 in (IEnumerable<KeyValuePair<IEntityType, IndexingStatusProvider>>) this.m_indexingStatusProviders)
        {
          IEntityType entityType = indexingStatusProvider1.Key;
          IndexingStatusProvider indexingStatusProvider2 = indexingStatusProvider1.Value;
          List<Tuple<IEntityType, string, int>> list = operationsInProgress.Where<Tuple<IEntityType, string, int>>((Func<Tuple<IEntityType, string, int>, bool>) (Tuple => Tuple.Item1.Name == entityType.Name)).ToList<Tuple<IEntityType, string, int>>();
          IndexingStatusDetails indexingStatusDetails;
          this.m_indexingStatus[entityType] = indexingStatusProvider2.GetCollectionIndexingStatus(vssRequestContext, list, out indexingStatusDetails);
          this.m_indexingStatusDetails[entityType] = indexingStatusDetails;
        }
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing && this.m_indexingStateRefreshEvent != null)
      {
        this.m_indexingStateRefreshEvent.Dispose();
        this.m_indexingStateRefreshEvent = (CountdownEvent) null;
      }
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
