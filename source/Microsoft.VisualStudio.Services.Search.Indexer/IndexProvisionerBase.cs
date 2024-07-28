// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisionerBase
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public abstract class IndexProvisionerBase : IIndexProvisioner, IDisposable
  {
    private readonly object m_serviceLock = new object();
    private Semaphore m_maxOnboardingAtOncePerJobAgent;
    private readonly IEntityType m_entityType;
    private bool m_disposedValue;

    protected IndexProvisionerBase(
      IndexingExecutionContext indexingExecutionContext,
      IndexProvisionType provisionerType,
      IEntityType entityType)
    {
      this.ProvisionType = provisionerType;
      this.m_entityType = entityType;
      this.SearchClusterManagementService = SearchPlatformFactory.GetInstance().CreateSearchClusterManagementService(indexingExecutionContext.ServiceSettings.JobAgentSearchPlatformConnectionString, indexingExecutionContext.ServiceSettings.JobAgentSearchPlatformSettings, indexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
    }

    private ProvisionerConfigAndConstantsProvider EntityProvisionProvider => EntityProvisionerFactory.GetIndexProvisioner(this.m_entityType);

    public IndexProvisionType ProvisionType { get; }

    protected ISearchClusterManagementService SearchClusterManagementService { get; }

    public IndexIdentity ProvisionIndex(
      IndexingExecutionContext indexingExecutionContext,
      ISearchPlatform searchPlatform,
      IndexIdentity indexToSkip = null)
    {
      Tracer.TraceEnter(1080200, "Indexing Pipeline", "Indexer", nameof (ProvisionIndex));
      IndexIdentity indexIdentity = (IndexIdentity) null;
      try
      {
        if (searchPlatform == null)
          throw new ArgumentNullException(nameof (searchPlatform));
        try
        {
          this.EnterSemaphoreQueue((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) indexingExecutionContext);
          if (indexingExecutionContext.IndexingUnit.IsIndexingIndexNameAvailable())
            indexIdentity = IndexIdentity.CreateIndexIdentity(indexingExecutionContext.IndexingUnit.GetIndexingIndexName());
          if (indexToSkip != null && indexToSkip.Equals((object) indexIdentity))
          {
            indexIdentity = this.MigrateIndexingUnit(indexingExecutionContext, searchPlatform, this.EntityProvisionProvider, indexToSkip);
            Tracer.TraceInfo(1080201, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("{0}IndexProvisioner: Provisioned another index [{1}] for [{2}]; index to skip [{3}].", (object) this.ProvisionType, (object) indexIdentity.Name, (object) indexingExecutionContext.IndexingUnit.TFSEntityId, (object) indexToSkip.Name)));
          }
          else
          {
            if (indexIdentity != null)
            {
              if (searchPlatform.IndexExists((Microsoft.VisualStudio.Services.Search.Common.ExecutionContext) indexingExecutionContext, indexIdentity))
                goto label_13;
            }
            indexIdentity = this.OnboardIndexingUnit(indexingExecutionContext, searchPlatform, this.EntityProvisionProvider);
            Tracer.TraceInfo(1080201, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("{0}IndexProvisioner: Provisioned index [{1}] for [{2}]", (object) this.ProvisionType, (object) indexIdentity.Name, (object) indexingExecutionContext.IndexingUnit.TFSEntityId)));
          }
        }
        catch (Exception ex)
        {
          indexingExecutionContext.FaultService.SetError(ex);
          ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
          this.LeaveSemaphoreQueue();
        }
      }
      finally
      {
        Tracer.TraceLeave(1080204, "Indexing Pipeline", "Indexer", nameof (ProvisionIndex));
      }
label_13:
      return indexIdentity;
    }

    internal virtual void Reset()
    {
    }

    protected abstract IndexIdentity OnboardIndexingUnit(
      IndexingExecutionContext executionContext,
      ISearchPlatform searchPlatform,
      ProvisionerConfigAndConstantsProvider entityProvisionProvider);

    protected abstract IndexIdentity MigrateIndexingUnit(
      IndexingExecutionContext executionContext,
      ISearchPlatform searchPlatform,
      ProvisionerConfigAndConstantsProvider entityProvisionerProvider,
      IndexIdentity indexToSkip);

    private void EnterSemaphoreQueue(Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext)
    {
      if (this.m_maxOnboardingAtOncePerJobAgent == null)
      {
        lock (this.m_serviceLock)
        {
          if (this.m_maxOnboardingAtOncePerJobAgent == null)
            this.m_maxOnboardingAtOncePerJobAgent = new Semaphore(executionContext.ServiceSettings.ProvisionerSettings.MaxAccountsToOnboardOnce, executionContext.ServiceSettings.ProvisionerSettings.MaxAccountsToOnboardOnce);
        }
      }
      this.m_maxOnboardingAtOncePerJobAgent.WaitOne();
    }

    private void LeaveSemaphoreQueue() => this.m_maxOnboardingAtOncePerJobAgent.Release();

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && this.m_maxOnboardingAtOncePerJobAgent != null)
      {
        this.m_maxOnboardingAtOncePerJobAgent.Dispose();
        this.m_maxOnboardingAtOncePerJobAgent = (Semaphore) null;
      }
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
