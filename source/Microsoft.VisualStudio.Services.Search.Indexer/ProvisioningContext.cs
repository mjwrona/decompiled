// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.ProvisioningContext
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class ProvisioningContext
  {
    private ISearchPlatform m_searchPlatform;
    private ISearchClusterManagementService m_searchClusterManagementService;

    internal ProvisioningContext(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit,
      string searchPlatformSettings,
      string searchPlatformConnectionString = null)
    {
      this.Settings = new ProvisionerSettings(indexingExecutionContext.RequestContext);
      this.IndexProvisioner = indexingExecutionContext.RequestContext.GetIndexProvisionerFactory(indexingUnit.EntityType).GetIndexProvisioner(indexingExecutionContext, indexingUnit);
      this.EntityProvisionProvider = EntityProvisionerFactory.GetIndexProvisioner(indexingUnit.EntityType);
      this.SearchPlatformSettings = searchPlatformSettings;
      this.IsOnPrem = indexingExecutionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment;
      if (indexingExecutionContext.RequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && indexingUnit.EntityType.Name == "Setting")
      {
        this.ContractType = DocumentContractType.SettingContract;
        this.SearchPlatformConnectionString = string.IsNullOrWhiteSpace(searchPlatformConnectionString) ? indexingExecutionContext.ServiceSettings.JobAgentSearchPlatformConnectionString : searchPlatformConnectionString;
      }
      else
      {
        IDocumentContractTypeService service = indexingExecutionContext.RequestContext.GetService<IDocumentContractTypeService>();
        bool flag1 = indexingExecutionContext.CheckIfZLRI_IsEnabledForEntity(indexingUnit) && indexingExecutionContext.IsReindexingFailedOrInProgress(indexingExecutionContext.DataAccessFactory, indexingUnit.EntityType);
        bool flag2 = this.IsIndexingUnitTypeSupportingShadowIndexing(indexingUnit);
        this.ContractType = flag1 & flag2 ? service.GetSupportedIndexDocumentContractTypeDuringZLRI(indexingExecutionContext.RequestContext, indexingUnit, indexingUnit.IsShadow) : service.GetSupportedIndexDocumentContractType(indexingExecutionContext.RequestContext, indexingUnit.EntityType);
        if (string.IsNullOrWhiteSpace(searchPlatformConnectionString))
          this.SearchPlatformConnectionString = flag1 & flag2 ? service.GetIndexESConnectionStringDuringZLRI(indexingExecutionContext.RequestContext, indexingUnit) : service.GetIndexESConnectionString(indexingExecutionContext.RequestContext, indexingUnit.EntityType);
        else
          this.SearchPlatformConnectionString = searchPlatformConnectionString;
      }
    }

    public ProvisionerSettings Settings { get; private set; }

    public virtual DocumentContractType ContractType { get; private set; }

    public IIndexProvisioner IndexProvisioner { get; private set; }

    public ProvisionerConfigAndConstantsProvider EntityProvisionProvider { get; private set; }

    public virtual string SearchPlatformConnectionString { get; private set; }

    public string SearchPlatformSettings { get; private set; }

    public bool IsOnPrem { get; private set; }

    public ISearchPlatform SearchPlatform
    {
      get
      {
        if (this.m_searchPlatform == null)
          this.m_searchPlatform = SearchPlatformFactory.GetInstance().Create(this.SearchPlatformConnectionString, this.SearchPlatformSettings, this.IsOnPrem);
        return this.m_searchPlatform;
      }
      internal set => this.m_searchPlatform = value;
    }

    public ISearchClusterManagementService SearchClusterManagementService
    {
      get
      {
        if (this.m_searchClusterManagementService == null)
          this.m_searchClusterManagementService = SearchPlatformFactory.GetInstance().CreateSearchClusterManagementService(this.SearchPlatformConnectionString, this.SearchPlatformSettings, this.IsOnPrem);
        return this.m_searchClusterManagementService;
      }
      internal set => this.m_searchClusterManagementService = value;
    }

    private bool IsIndexingUnitTypeSupportingShadowIndexing(IndexingUnit indexingUnit)
    {
      if (indexingUnit != null)
      {
        IEntityType entityType = indexingUnit.EntityType;
        if (entityType.Name == "Code")
        {
          string indexingUnitType = indexingUnit.IndexingUnitType;
          if (indexingUnitType == "Git_Repository" || indexingUnitType == "ScopedIndexingUnit" || indexingUnitType == "TemporaryIndexingUnit" || indexingUnitType == "TFVC_Repository")
            return true;
        }
        else if (entityType.Name == "WorkItem" && indexingUnit.IndexingUnitType == "Project")
          return true;
      }
      return false;
    }
  }
}
