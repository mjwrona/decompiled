// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.SharedIndexSelectorService
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class SharedIndexSelectorService : ISharedIndexSelectorService, IVssFrameworkService
  {
    private SharedIndexSelectorService.SelectorSettings m_settings;

    internal virtual SharedIndexSelectorService.SelectorSettings BuildSettings(
      IVssRequestContext requestContext)
    {
      return new SharedIndexSelectorService.SelectorSettings(requestContext);
    }

    public IndexIdentity SelectSharedIndexToOnboardAccount(
      IndexingExecutionContext executionContext,
      IEntityType entityType,
      IndexIdentity indexToSkip = null)
    {
      EntitySharedIndexSelector sharedIndexSelector;
      if (!this.m_settings.IndexSelectors.TryGetValue(entityType.Name, out sharedIndexSelector))
        throw new IndexProvisionException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1} :: Unsupported entity type '{2}'", (object) nameof (SharedIndexSelectorService), (object) nameof (SelectSharedIndexToOnboardAccount), (object) entityType)));
      return sharedIndexSelector.SelectSharedIndexToOnboardAccount(executionContext, indexToSkip);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnPlatformConnectionStringChange), (RegistryQuery) "/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
      IVssRequestContext deploymentRequestContext = this.GetDeploymentRequestContext(systemRequestContext);
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        deploymentRequestContext.GetService<IVssRegistryService>().RegisterNotification(deploymentRequestContext, new RegistrySettingsChangedCallback(this.OnUserChanged), "/Service/ALMSearch/Settings/ElasticsearchUser");
        deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(deploymentRequestContext, new StrongBoxItemChangedCallback(this.OnPasswordChanged), "ElasticsearchPasswordDrawer", (IEnumerable<string>) new string[1]
        {
          "ElasticsearchPassword"
        });
      }
      else
        deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(deploymentRequestContext, new StrongBoxItemChangedCallback(this.OnPasswordChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
        {
          "ElasticSearchHostedAuthPassword"
        });
      Interlocked.CompareExchange<SharedIndexSelectorService.SelectorSettings>(ref this.m_settings, this.BuildSettings(systemRequestContext), (SharedIndexSelectorService.SelectorSettings) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnPlatformConnectionStringChange));
      IVssRequestContext deploymentRequestContext = this.GetDeploymentRequestContext(systemRequestContext);
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        deploymentRequestContext.GetService<IVssRegistryService>().UnregisterNotification(deploymentRequestContext, new RegistrySettingsChangedCallback(this.OnUserChanged));
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnPasswordChanged));
    }

    private void OnUserChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.OnPlatformCollectionStringOrSettingsChange(requestContext);
    }

    private void OnPasswordChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.OnPlatformCollectionStringOrSettingsChange(requestContext);
    }

    private void OnPlatformConnectionStringChange(
      IVssRequestContext requestContext,
      RegistryEntryCollection updatedRegistry)
    {
      this.OnPlatformCollectionStringOrSettingsChange(requestContext);
    }

    private void OnPlatformCollectionStringOrSettingsChange(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        Volatile.Write<SharedIndexSelectorService.SelectorSettings>(ref this.m_settings, this.BuildSettings(requestContext));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082500, "Indexing Pipeline", "Indexer", ex);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private IVssRequestContext GetDeploymentRequestContext(IVssRequestContext requestContext) => !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.To(TeamFoundationHostType.Deployment) : requestContext;

    internal class SelectorSettings
    {
      private IDataAccessFactory m_dataAccessFactory;

      public SelectorSettings()
      {
      }

      [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Temporarily suppressing as this has many virtual properties and requires more thoughtful refactoring to fix the violation.")]
      public SelectorSettings(IVssRequestContext requestContext) => this.Init(requestContext);

      public void Init(IVssRequestContext requestContext)
      {
        Microsoft.VisualStudio.Services.Search.Common.ExecutionContext executionContext = requestContext.GetExecutionContext(MethodBase.GetCurrentMethod().Name, 0);
        executionContext.FaultService = (IIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext));
        this.SearchPlatform = SearchPlatformFactory.GetInstance().Create(executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString, executionContext.ServiceSettings.JobAgentSearchPlatformSettings, executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
        this.SearchClusterManagementService = SearchPlatformFactory.GetInstance().CreateSearchClusterManagementService(executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString, executionContext.ServiceSettings.JobAgentSearchPlatformSettings, executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment);
        this.IndexSelectors = new Dictionary<string, EntitySharedIndexSelector>();
        this.m_dataAccessFactory = DataAccessFactory.GetInstance();
        this.CreateSearchIndexHelper = new CreateSearchIndexHelper();
        foreach (ProvisionerConfigAndConstantsProvider indexProvisioner1 in EntityProvisionerFactory.GetIndexProvisioners(executionContext))
        {
          EntitySharedIndexSelector indexProvisioner2 = SharedIndexSelectorFactory.GetSharedIndexProvisioner(executionContext, indexProvisioner1, this.SearchPlatform, this.SearchClusterManagementService, this.m_dataAccessFactory, this.CreateSearchIndexHelper);
          this.IndexSelectors.Add(indexProvisioner1.EntityType.Name, indexProvisioner2);
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(1082277, TraceLevel.Info, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("{0} initialized with platform connection string [{1}] and settings [{2}].", (object) nameof (SharedIndexSelectorService), (object) executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString, (object) this.SanitizeConnectionString(executionContext.ServiceSettings.JobAgentSearchPlatformSettings))));
      }

      internal string SanitizeConnectionString(string connectionSettings)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string str in connectionSettings.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
          if (str.StartsWith("User", StringComparison.Ordinal) || str.StartsWith("Password", StringComparison.Ordinal))
          {
            stringBuilder.Append(str.Substring(0, str.IndexOf("=", StringComparison.Ordinal) + 1) + "******,");
          }
          else
          {
            stringBuilder.Append(str);
            stringBuilder.Append(",");
          }
        }
        return stringBuilder.ToString().TrimEnd(",".ToCharArray());
      }

      internal virtual ISearchPlatform SearchPlatform { get; set; }

      internal virtual CreateSearchIndexHelper CreateSearchIndexHelper { get; set; }

      internal virtual ISearchClusterManagementService SearchClusterManagementService { get; set; }

      internal Dictionary<string, EntitySharedIndexSelector> IndexSelectors { get; set; }
    }
  }
}
