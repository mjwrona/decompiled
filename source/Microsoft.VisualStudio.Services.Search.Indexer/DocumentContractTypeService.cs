// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.DocumentContractTypeService
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class DocumentContractTypeService : IDocumentContractTypeService, IVssFrameworkService
  {
    private DocumentContractType m_supportedQueryContractTypeForCodeEntity;
    private IEnumerable<IndexInfo> m_queryIndicesForCodeEntity;
    private string m_queryEsConnectionStringForCodeEntity;
    private DocumentContractType m_supportedQueryContractTypeForWorkItemEntity;
    private string m_queryEsConnectionStringForWorkItemEntity;
    private IEnumerable<IndexInfo> m_queryIndicesForWorkItemEntity;
    private DocumentContractType m_supportedQueryContractTypeForPackageEntity;
    private string m_queryEsConnectionStringForPackageEntity;
    private IEnumerable<IndexInfo> m_queryIndicesForPackageEntity;
    private DocumentContractType m_supportedQueryContractTypeForWikiEntity;
    private string m_queryEsConnectionStringForWikiEntity;
    private IEnumerable<IndexInfo> m_queryIndicesForWikiEntity;
    private DocumentContractType m_supportedQueryContractTypeForBoardEntity;
    private string m_queryEsConnectionStringForBoardEntity;
    private IEnumerable<IndexInfo> m_queryIndicesForBoardEntity;
    private DocumentContractType m_supportedQueryContractTypeForSettingEntity;
    private string m_queryESConnectionStringForSettingEntity;
    private IEnumerable<IndexInfo> m_queryIndicesForSettingEntity;
    private readonly IDataAccessFactory m_dataAccessFactory;
    private IEnumerable<IndexInfo> m_queryIndicesForABTesting;

    public DocumentContractTypeService()
      : this(DataAccessFactory.GetInstance())
    {
    }

    internal DocumentContractTypeService(IDataAccessFactory dataAccessFactory) => this.m_dataAccessFactory = dataAccessFactory;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.InitializeContractInfoForABTesting(systemRequestContext);
      this.InitializeCodeContractInformation(systemRequestContext);
      this.InitializeWorkItemContractInformation(systemRequestContext);
      this.InitializeWikiContractInformation(systemRequestContext);
      this.InitializePackageContractInformation(systemRequestContext);
      this.InitializeBoardContractInformation(systemRequestContext);
      this.InitializeSettingContractInformation(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.DocumentContractTypeChanged, new SqlNotificationHandler(this.UpdateDocumentContractType), false);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.DocumentContractTypeChanged, new SqlNotificationHandler(this.RemoveDocumentContractVersion), false);

    public DocumentContractType GetSupportedQueryDocumentContractType(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      string name = entityType.Name;
      if (name != null)
      {
        switch (name.Length)
        {
          case 4:
            switch (name[0])
            {
              case 'C':
                if (name == "Code")
                  break;
                goto label_19;
              case 'W':
                if (name == "Wiki")
                  goto label_15;
                else
                  goto label_19;
              default:
                goto label_19;
            }
            break;
          case 5:
            if (name == "Board")
              return this.m_supportedQueryContractTypeForBoardEntity;
            goto label_19;
          case 7:
            switch (name[0])
            {
              case 'P':
                if (name == "Package")
                  return this.m_supportedQueryContractTypeForPackageEntity;
                goto label_19;
              case 'S':
                if (name == "Setting")
                  return this.m_supportedQueryContractTypeForSettingEntity;
                goto label_19;
              default:
                goto label_19;
            }
          case 8:
            if (name == "WorkItem")
              return this.m_supportedQueryContractTypeForWorkItemEntity;
            goto label_19;
          case 10:
            switch (name[6])
            {
              case 'C':
                if (name == "TenantCode")
                  break;
                goto label_19;
              case 'W':
                if (name == "TenantWiki")
                  goto label_15;
                else
                  goto label_19;
              default:
                goto label_19;
            }
            break;
          default:
            goto label_19;
        }
        return this.m_supportedQueryContractTypeForCodeEntity;
label_15:
        return this.m_supportedQueryContractTypeForWikiEntity;
      }
label_19:
      return DocumentContractType.Unsupported;
    }

    public DocumentContractType GetSupportedIndexDocumentContractType(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      if (entityType.Name == "Code" || entityType.Name == "WorkItem" || entityType.Name == "Wiki" || entityType.Name == "Package" || entityType.Name == "Board")
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", entityType);
        return !(indexingUnit?.Properties is CollectionIndexingProperties properties) ? DocumentContractTypeServiceHelper.GetDefaultDocumentContractType(requestContext, entityType, indexingUnit) : properties.IndexContractType;
      }
      return entityType.Name == "Setting" ? this.m_supportedQueryContractTypeForSettingEntity : DocumentContractType.Unsupported;
    }

    public DocumentContractType GetSupportedIndexDocumentContractTypeDuringZLRI(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      bool isShadow)
    {
      IEntityType entityType = indexingUnit.EntityType;
      if (entityType.Name == "Code" || entityType.Name == "WorkItem")
      {
        if (!(this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", entityType)?.Properties is CollectionIndexingProperties properties))
          return DocumentContractTypeServiceHelper.GetDefaultDocumentContractType(requestContext, entityType, indexingUnit);
        return !isShadow ? properties.QueryContractType : properties.IndexContractType;
      }
      return entityType.Name == "Wiki" || entityType.Name == "Package" || entityType.Name == "Board" || entityType.Name == "ProjectRepo" || entityType.Name == "Setting" ? this.GetSupportedIndexDocumentContractType(requestContext, entityType) : DocumentContractType.Unsupported;
    }

    public IEnumerable<IndexInfo> GetQueryIndexInfo(IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return this.m_queryIndicesForCodeEntity;
        case "WorkItem":
          return this.m_queryIndicesForWorkItemEntity;
        case "Wiki":
          return this.m_queryIndicesForWikiEntity;
        case "Package":
          return this.m_queryIndicesForPackageEntity;
        case "Board":
          return this.m_queryIndicesForBoardEntity;
        case "Setting":
          return this.m_queryIndicesForSettingEntity;
        default:
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Unknown entity type {0}", (object) entityType.Name)));
      }
    }

    public IEnumerable<IndexInfo> GetQueryIndexInfoForABTesting() => this.m_queryIndicesForABTesting;

    public string GetIndexESConnectionString(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      if (entityType.Name == "Code" || entityType.Name == "WorkItem" || entityType.Name == "Wiki" || entityType.Name == "Package" || entityType.Name == "Board")
      {
        IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", entityType);
        if (!(indexingUnit?.Properties is CollectionIndexingProperties properties))
          return requestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
        this.UpdateIndexESConnectionStringForOnPremiseIfNeeded(requestContext, indexingUnitDataAccess, indexingUnit);
        return properties.IndexESConnectionString;
      }
      throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Unknown entity type {0}", (object) entityType.Name)));
    }

    public string GetIndexESConnectionStringDuringZLRI(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      IEntityType entityType = indexingUnit.EntityType;
      if (!(entityType.Name == "Code") && !(entityType.Name == "WorkItem"))
        return this.GetIndexESConnectionString(requestContext, entityType);
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      if (!(indexingUnitDataAccess.GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", entityType)?.Properties is CollectionIndexingProperties properties))
        return requestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
      this.UpdateIndexESConnectionStringForOnPremiseIfNeeded(requestContext, indexingUnitDataAccess, indexingUnit);
      return !indexingUnit.IsShadow ? properties.QueryESConnectionString : properties.IndexESConnectionString;
    }

    public string GetQueryESConnectionString(IEntityType entityType)
    {
      if (entityType.Name == "Code" || entityType.Name == "TenantCode")
        return this.m_queryEsConnectionStringForCodeEntity;
      if (entityType.Name == "WorkItem")
        return this.m_queryEsConnectionStringForWorkItemEntity;
      if (entityType.Name == "Wiki" || entityType.Name == "TenantWiki")
        return this.m_queryEsConnectionStringForWikiEntity;
      if (entityType.Name == "Package")
        return this.m_queryEsConnectionStringForPackageEntity;
      if (entityType.Name == "Board")
        return this.m_queryEsConnectionStringForBoardEntity;
      if (entityType.Name == "Setting")
        return this.m_queryESConnectionStringForSettingEntity;
      throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Unknown entity type {0}", (object) entityType.Name)));
    }

    internal virtual void UpdateDocumentContractType(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        this.InitializeContractInfoForABTesting(requestContext);
        this.InitializeCodeContractInformation(requestContext);
        this.InitializeWorkItemContractInformation(requestContext);
        this.InitializeWikiContractInformation(requestContext);
        this.InitializePackageContractInformation(requestContext);
        this.InitializeBoardContractInformation(requestContext);
        this.InitializeSettingContractInformation(requestContext);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083020, "Common", nameof (DocumentContractTypeService), ex);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    internal virtual void RemoveDocumentContractVersion(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      this.m_supportedQueryContractTypeForCodeEntity = DocumentContractType.Unsupported;
      this.m_queryEsConnectionStringForCodeEntity = string.Empty;
      this.m_supportedQueryContractTypeForWorkItemEntity = DocumentContractType.Unsupported;
      this.m_queryEsConnectionStringForWorkItemEntity = string.Empty;
      this.m_supportedQueryContractTypeForWikiEntity = DocumentContractType.Unsupported;
      this.m_queryEsConnectionStringForWikiEntity = string.Empty;
      this.m_supportedQueryContractTypeForPackageEntity = DocumentContractType.Unsupported;
      this.m_queryEsConnectionStringForPackageEntity = string.Empty;
    }

    private void InitializeContractInfoForABTesting(IVssRequestContext requestContext)
    {
      this.m_queryIndicesForABTesting = (IEnumerable<IndexInfo>) null;
      if (!requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableABTesting"))
        return;
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      if (indexingUnitDataAccess.GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) CodeEntityType.GetInstance())?.Properties is CollectionIndexingProperties properties1)
        this.m_queryIndicesForABTesting = (IEnumerable<IndexInfo>) properties1.IndexIndices;
      if (!requestContext.IsAccountWithLargeRepository())
        return;
      List<IndexInfo> source = new List<IndexInfo>();
      ISet<string> largeRepoIdSet = requestContext.GetLargeRepoIdSet();
      List<string> stringList = new List<string>()
      {
        "Git_Repository",
        "TFVC_Repository",
        "CustomRepository"
      };
      foreach (string input in (IEnumerable<string>) largeRepoIdSet)
      {
        foreach (string indexingUnitType in stringList)
        {
          IndexingProperties properties2 = indexingUnitDataAccess.GetIndexingUnit(requestContext, Guid.Parse(input), indexingUnitType, (IEntityType) CodeEntityType.GetInstance(), true)?.Properties;
          if (properties2 != null)
            source.AddRange((IEnumerable<IndexInfo>) properties2.IndexIndices);
        }
      }
      if (this.m_queryIndicesForABTesting != null)
        source.AddRange(this.m_queryIndicesForABTesting);
      this.m_queryIndicesForABTesting = (IEnumerable<IndexInfo>) source.Distinct<IndexInfo>().ToList<IndexInfo>();
    }

    private void InitializeCodeContractInformation(IVssRequestContext requestContext)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) CodeEntityType.GetInstance());
      if (!(indexingUnit?.Properties is CollectionIndexingProperties properties))
      {
        this.m_supportedQueryContractTypeForCodeEntity = DocumentContractTypeServiceHelper.GetDefaultDocumentContractType(requestContext, (IEntityType) CodeEntityType.GetInstance(), indexingUnit);
        this.m_queryEsConnectionStringForCodeEntity = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      }
      else
      {
        this.UpdateQueryESConnectionStringForOnPremiseIfNeeded(requestContext, indexingUnitDataAccess, indexingUnit);
        this.m_supportedQueryContractTypeForCodeEntity = properties.QueryContractType;
        this.m_queryEsConnectionStringForCodeEntity = properties.QueryESConnectionString;
        this.m_queryIndicesForCodeEntity = (IEnumerable<IndexInfo>) properties.QueryIndices;
      }
    }

    private void InitializeWorkItemContractInformation(IVssRequestContext requestContext)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) WorkItemEntityType.GetInstance());
      if (!(indexingUnit?.Properties is CollectionIndexingProperties properties))
      {
        this.m_supportedQueryContractTypeForWorkItemEntity = DocumentContractType.WorkItemContract;
        this.m_queryEsConnectionStringForWorkItemEntity = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      }
      else
      {
        this.UpdateQueryESConnectionStringForOnPremiseIfNeeded(requestContext, indexingUnitDataAccess, indexingUnit);
        this.m_supportedQueryContractTypeForWorkItemEntity = properties.QueryContractType;
        this.m_queryEsConnectionStringForWorkItemEntity = properties.QueryESConnectionString;
        this.m_queryIndicesForWorkItemEntity = (IEnumerable<IndexInfo>) properties.QueryIndices;
      }
    }

    private void InitializePackageContractInformation(IVssRequestContext requestContext)
    {
      if (!(this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) PackageEntityType.GetInstance())?.Properties is CollectionIndexingProperties properties))
      {
        this.m_supportedQueryContractTypeForPackageEntity = DocumentContractType.PackageVersionContract;
        this.m_queryEsConnectionStringForPackageEntity = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      }
      else
      {
        this.m_supportedQueryContractTypeForPackageEntity = properties.QueryContractType;
        this.m_queryEsConnectionStringForPackageEntity = properties.QueryESConnectionString;
        this.m_queryIndicesForPackageEntity = (IEnumerable<IndexInfo>) properties.QueryIndices;
      }
    }

    private void InitializeWikiContractInformation(IVssRequestContext requestContext)
    {
      if (!(this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) WikiEntityType.GetInstance())?.Properties is CollectionIndexingProperties properties))
      {
        this.m_supportedQueryContractTypeForWikiEntity = DocumentContractType.WikiContract;
        this.m_queryEsConnectionStringForWikiEntity = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      }
      else
      {
        this.m_supportedQueryContractTypeForWikiEntity = properties.QueryContractType;
        this.m_queryEsConnectionStringForWikiEntity = properties.QueryESConnectionString;
        this.m_queryIndicesForWikiEntity = (IEnumerable<IndexInfo>) properties.QueryIndices;
      }
    }

    private void InitializeBoardContractInformation(IVssRequestContext requestContext)
    {
      if (!(this.m_dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext, requestContext.GetCollectionID(), "Collection", (IEntityType) BoardEntityType.GetInstance())?.Properties is CollectionIndexingProperties properties))
      {
        this.m_supportedQueryContractTypeForBoardEntity = DocumentContractType.BoardContract;
        this.m_queryEsConnectionStringForBoardEntity = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      }
      else
      {
        this.m_supportedQueryContractTypeForBoardEntity = properties.QueryContractType;
        this.m_queryEsConnectionStringForBoardEntity = properties.QueryESConnectionString;
        this.m_queryIndicesForBoardEntity = (IEnumerable<IndexInfo>) properties.QueryIndices;
      }
    }

    private void InitializeSettingContractInformation(IVssRequestContext requestContext)
    {
      DeploymentIndexingProperties indexingProperties = this.m_dataAccessFactory.GetSharedIndexingPropertyDataAccess().GetIndexingProperties(requestContext);
      if (indexingProperties == null)
      {
        this.m_supportedQueryContractTypeForSettingEntity = DocumentContractType.SettingContract;
        this.m_queryESConnectionStringForSettingEntity = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      }
      else
      {
        this.m_supportedQueryContractTypeForSettingEntity = indexingProperties.QueryContractType;
        this.m_queryESConnectionStringForSettingEntity = indexingProperties.QueryESConnectionString;
        this.m_queryIndicesForSettingEntity = (IEnumerable<IndexInfo>) indexingProperties.QueryIndices;
      }
    }

    private void UpdateQueryESConnectionStringForOnPremiseIfNeeded(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      string configValue = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      if (!(indexingUnit?.Properties is CollectionIndexingProperties properties) || configValue.Equals(properties.QueryESConnectionString))
        return;
      properties.QueryESConnectionString = configValue;
      indexingUnit = indexingUnitDataAccess.UpdateIndexingUnit(requestContext, indexingUnit);
    }

    private void UpdateIndexESConnectionStringForOnPremiseIfNeeded(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      string configValue = requestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
      if (!(indexingUnit?.Properties is CollectionIndexingProperties properties) || configValue.Equals(properties.IndexESConnectionString))
        return;
      properties.IndexESConnectionString = configValue;
      indexingUnit.Properties.ErasePreReindexingState();
      indexingUnit = indexingUnitDataAccess.UpdateIndexingUnit(requestContext, indexingUnit);
    }
  }
}
