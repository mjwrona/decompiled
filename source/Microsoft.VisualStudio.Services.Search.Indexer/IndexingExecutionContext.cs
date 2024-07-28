// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.Settings;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  [Export(typeof (ICoreIndexingExecutionContext))]
  public class IndexingExecutionContext : CoreIndexingExecutionContext
  {
    private IFileMetadataStoreDataAccess m_fileMetadataStoreDataAccess;
    private FailureRecordStore m_failureRecordStore;
    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit m_indexingUnitForStore;
    private IItemLevelFailureDataAccess m_itemLevelFailureDataAccess;
    private ITempFileMetadataStoreDataAccess m_tempFileMetadataStoreDataAccess;

    public IndexingExecutionContext()
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "The class is not derived by any other class. So the virtual method invocation in this case should not cause any violation")]
    internal IndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      IDataAccessFactory dataAccessFactory)
      : base(requestContext, indexingUnit, tracerCICorrelationDetails, dataAccessFactory)
    {
      this.Initialize(requestContext, indexingUnit, (string) null, (IIndexingUnitChangeEventHandler) null);
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "The class is not derived by any other class. So the virtual method invocation in this case should not cause any violation")]
    internal IndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      string searchPlatformConnectionString)
      : base(requestContext, indexingUnit, tracerCICorrelationDetails)
    {
      this.Initialize(requestContext, indexingUnit, searchPlatformConnectionString, (IIndexingUnitChangeEventHandler) null);
    }

    public IndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails)
      : this(requestContext, indexingUnit, tracerCICorrelationDetails, (IIndexingUnitChangeEventHandler) null)
    {
    }

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "The class is not derived by any other class. So the virtual method invocation in this case should not cause any violation")]
    public IndexingExecutionContext(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      ITracerCICorrelationDetails tracerCICorrelationDetails,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(requestContext, indexingUnit, tracerCICorrelationDetails, indexingUnitChangeEventHandler)
    {
      this.Initialize(requestContext, indexingUnit, (string) null, indexingUnitChangeEventHandler);
    }

    public override IEntityType[] SupportedEntityTypes => new IEntityType[9]
    {
      (IEntityType) CodeEntityType.GetInstance(),
      (IEntityType) ProjectRepoEntityType.GetInstance(),
      (IEntityType) TenantCodeEntityType.GetInstance(),
      (IEntityType) TenantWikiEntityType.GetInstance(),
      (IEntityType) WorkItemEntityType.GetInstance(),
      (IEntityType) FileEntityType.GetInstance(),
      (IEntityType) WikiEntityType.GetInstance(),
      (IEntityType) PackageEntityType.GetInstance(),
      (IEntityType) BoardEntityType.GetInstance()
    };

    public virtual IShardDetailsDataAccess ShardDetailsDataAccess => this.DataAccessFactory.GetShardDetailsDataAccess();

    public virtual ITempFileMetadataStoreDataAccess TempFileMetadataStoreDataAccess
    {
      get
      {
        if (this.m_tempFileMetadataStoreDataAccess == null)
          this.m_tempFileMetadataStoreDataAccess = this.DataAccessFactory.GetTempFileStoreDataAccess(this.m_indexingUnitForStore);
        return this.m_tempFileMetadataStoreDataAccess;
      }
    }

    public virtual IFileMetadataStoreDataAccess FileMetadataStoreDataAccess
    {
      get
      {
        if (this.m_fileMetadataStoreDataAccess == null)
          this.m_fileMetadataStoreDataAccess = this.DataAccessFactory.GetFileMetadataStoreDataAccess(this.m_indexingUnitForStore);
        return this.m_fileMetadataStoreDataAccess;
      }
    }

    public virtual IItemLevelFailureDataAccess ItemLevelFailureDataAccess => this.m_itemLevelFailureDataAccess ?? (this.m_itemLevelFailureDataAccess = this.DataAccessFactory.GetItemLevelFailureDataAccess());

    public virtual FailureRecordStore FailureRecordStore => this.m_failureRecordStore ?? (this.m_failureRecordStore = new FailureRecordStore(this, this.DataAccessFactory));

    public virtual ProvisioningContext ProvisioningContext { get; private set; }

    public virtual string ProjectName { get; set; }

    public ProjectVisibility ProjectVisibility { get; set; }

    public virtual string RepositoryName { get; set; }

    public Guid? ProjectId { get; set; }

    public virtual Guid? RepositoryId => this.RepositoryIndexingUnit?.TFSEntityId;

    public virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit RepositoryIndexingUnit { get; set; }

    public Microsoft.VisualStudio.Services.Search.Common.IndexingUnit ProjectIndexingUnit { get; set; }

    public FeederSettings FeederSettings { get; set; }

    public CrawlerSettings CrawlerSettings { get; set; }

    public bool IsIndexingEnabled()
    {
      IndexingProperties properties = this.IndexingUnit.Properties;
      switch (this.IndexingUnit.EntityType.Name)
      {
        case "Code":
          return this.RequestContext.IsCodeIndexingEnabled() && !properties.IsDisabled;
        case "WorkItem":
          return this.RequestContext.IsWorkItemIndexingEnabled();
        case "Wiki":
          return this.RequestContext.IsWikiIndexingEnabled();
        default:
          return true;
      }
    }

    public bool IsCrudOperationsFeatureEnabled() => this.RequestContext.IsCrudOperationsFeatureEnabled(this.IndexingUnit.EntityType);

    public RouteLevel GetRouteLevel(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = null)
    {
      if (indexingUnit == null)
        indexingUnit = this.IndexingUnit;
      return "WorkItem" == indexingUnit.EntityType.Name && this.CollectionIndexingUnit != null && this.CollectionIndexingUnit.IsLargeCollection(this.RequestContext) ? RouteLevel.None : this.ServiceSettings.ProvisionerSettings.GetRouteLevel(this.IndexingUnit.EntityType);
    }

    public virtual IndexIdentity GetIndex()
    {
      IndexIdentity index = (IndexIdentity) null;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit extractingIndexInfo = this.GetIndexingUnitForExtractingIndexInfo();
      if (extractingIndexInfo.IsIndexingIndexNameAvailable())
        index = IndexIdentity.CreateIndexIdentity(extractingIndexInfo.GetIndexingIndexName());
      return index;
    }

    public virtual IndexingExecutionContext ToIndexingExecutionContextWithDifferentIndexingUnit(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit otherIndexingUnit)
    {
      IndexingExecutionContext differentIndexingUnit = new IndexingExecutionContext(this.RequestContext, otherIndexingUnit, this.ExecutionTracerContext.TracerCICorrelationDetails, this.DataAccessFactory);
      differentIndexingUnit.FaultService = this.FaultService;
      differentIndexingUnit.InitializeNameAndIds(this.DataAccessFactory);
      return differentIndexingUnit;
    }

    internal virtual string GetProjectNameFromTFS() => this.RequestContext.GetClient<ProjectHttpClient>().GetProject(this.ProjectId.ToString(), new bool?(true)).Result.Name;

    public override void InitializeNameAndIds(IDataAccessFactory dataAccessFactory)
    {
      base.InitializeNameAndIds(dataAccessFactory);
      IIndexingUnitDataAccess indexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
      string indexingUnitType = this.IndexingUnit.IndexingUnitType;
      if (indexingUnitType != null)
      {
        switch (indexingUnitType.Length)
        {
          case 7:
            if (indexingUnitType == "Project")
            {
              this.ProjectIndexingUnit = this.IndexingUnit;
              this.ProjectId = new Guid?(this.ProjectIndexingUnit.TFSEntityId);
              if (!string.IsNullOrWhiteSpace(this.ProjectIndexingUnit.Properties?.Name))
              {
                this.ProjectName = this.ProjectIndexingUnit.Properties.Name;
              }
              else
              {
                switch (this.ProjectIndexingUnit.EntityType.Name)
                {
                  case "Code":
                  case "Wiki":
                    this.ProjectName = this.ProjectIndexingUnit.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes1 ? entityAttributes1.ProjectName : (string) null;
                    this.ProjectVisibility = entityAttributes1 != null ? entityAttributes1.ProjectVisibility : ProjectVisibility.Private;
                    break;
                  case "WorkItem":
                    this.ProjectName = this.ProjectIndexingUnit.TFSEntityAttributes is ProjectWorkItemTFSAttributes entityAttributes2 ? entityAttributes2.ProjectName : (string) null;
                    break;
                  default:
                    throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType:{0} is not supported.", (object) this.IndexingUnit.EntityType.Name)));
                }
              }
              this.CollectionIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.ProjectIndexingUnit.ParentUnitId);
              break;
            }
            goto label_43;
          case 10:
            if (indexingUnitType == "Collection")
            {
              this.CollectionIndexingUnit = this.IndexingUnit;
              break;
            }
            goto label_43;
          case 12:
            if (indexingUnitType == "Organization")
            {
              this.OrganizationIndexingUnit = this.IndexingUnit;
              break;
            }
            goto label_43;
          case 14:
            if (indexingUnitType == "Git_Repository")
            {
              this.RepositoryIndexingUnit = this.IndexingUnit;
              if (!string.IsNullOrWhiteSpace(this.RepositoryIndexingUnit.Properties?.Name))
                this.RepositoryName = this.RepositoryIndexingUnit.Properties.Name;
              else if (this.RepositoryIndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes)
                this.RepositoryName = this.RepositoryIndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes ? entityAttributes.RepositoryName : (string) null;
              this.ProjectIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.IndexingUnit.ParentUnitId);
              if (this.ProjectIndexingUnit != null)
              {
                this.ProjectId = new Guid?(this.ProjectIndexingUnit.TFSEntityId);
                this.PopulateProjectPropertiesFromProjectIndexingUnit(this.ProjectIndexingUnit);
                this.CollectionIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.ProjectIndexingUnit.ParentUnitId);
                break;
              }
              break;
            }
            goto label_43;
          case 15:
            if (indexingUnitType == "TFVC_Repository")
            {
              this.RepositoryIndexingUnit = this.IndexingUnit;
              this.RepositoryName = string.IsNullOrWhiteSpace(this.IndexingUnit.Properties?.Name) ? (this.IndexingUnit.TFSEntityAttributes is TfvcCodeRepoTFSAttributes entityAttributes ? entityAttributes.RepositoryName : (string) null) : this.IndexingUnit.Properties.Name;
              this.ProjectIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.IndexingUnit.ParentUnitId);
              this.ProjectId = new Guid?(this.ProjectIndexingUnit.TFSEntityId);
              this.PopulateProjectPropertiesFromProjectIndexingUnit(this.ProjectIndexingUnit);
              this.CollectionIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.ProjectIndexingUnit.ParentUnitId);
              break;
            }
            goto label_43;
          case 16:
            if (indexingUnitType == "CustomRepository")
            {
              this.RepositoryIndexingUnit = this.IndexingUnit;
              this.RepositoryName = string.IsNullOrWhiteSpace(this.IndexingUnit.Properties?.Name) ? (this.IndexingUnit.TFSEntityAttributes is CustomRepoCodeTFSAttributes entityAttributes ? entityAttributes.RepositoryName : (string) null) : this.IndexingUnit.Properties.Name;
              this.ProjectName = (this.IndexingUnit.TFSEntityAttributes as CustomRepoCodeTFSAttributes).ProjectName;
              this.ProjectIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.IndexingUnit.ParentUnitId);
              this.ProjectId = new Guid?(this.ProjectIndexingUnit.TFSEntityId);
              this.CollectionIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.ProjectIndexingUnit.ParentUnitId);
              break;
            }
            goto label_43;
          case 18:
            if (indexingUnitType == "ScopedIndexingUnit")
            {
              this.RepositoryIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.IndexingUnit.ParentUnitId);
              if (!string.IsNullOrWhiteSpace(this.RepositoryIndexingUnit.Properties?.Name))
              {
                this.RepositoryName = this.RepositoryIndexingUnit.Properties.Name;
              }
              else
              {
                switch (this.RepositoryIndexingUnit.IndexingUnitType)
                {
                  case "Git_Repository":
                    this.RepositoryName = this.RepositoryIndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes3 ? entityAttributes3.RepositoryName : (string) null;
                    break;
                  case "TFVC_Repository":
                    this.RepositoryName = this.RepositoryIndexingUnit.TFSEntityAttributes is TfvcCodeRepoTFSAttributes entityAttributes4 ? entityAttributes4.RepositoryName : (string) null;
                    break;
                  case "CustomRepository":
                    this.RepositoryName = this.RepositoryIndexingUnit.TFSEntityAttributes is CustomRepoCodeTFSAttributes entityAttributes5 ? entityAttributes5.RepositoryName : (string) null;
                    break;
                  default:
                    throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("IndexingUnitType:{0} is not supported as parent for ScopedIndexingUnit.", (object) this.RepositoryIndexingUnit.IndexingUnitType)));
                }
              }
              this.ProjectIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.RepositoryIndexingUnit.ParentUnitId);
              this.PopulateProjectPropertiesFromProjectIndexingUnit(this.ProjectIndexingUnit);
              this.ProjectId = new Guid?(this.ProjectIndexingUnit.TFSEntityId);
              this.CollectionIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.ProjectIndexingUnit.ParentUnitId);
              break;
            }
            goto label_43;
          case 21:
            if (indexingUnitType == "TemporaryIndexingUnit")
            {
              Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.IndexingUnit.ParentUnitId);
              this.RepositoryIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, indexingUnit.ParentUnitId);
              if (!string.IsNullOrWhiteSpace(this.RepositoryIndexingUnit.Properties?.Name))
              {
                this.RepositoryName = this.RepositoryIndexingUnit.Properties.Name;
              }
              else
              {
                switch (this.RepositoryIndexingUnit.IndexingUnitType)
                {
                  case "Git_Repository":
                    this.RepositoryName = this.RepositoryIndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes6 ? entityAttributes6.RepositoryName : (string) null;
                    break;
                  case "TFVC_Repository":
                    this.RepositoryName = this.RepositoryIndexingUnit.TFSEntityAttributes is TfvcCodeRepoTFSAttributes entityAttributes7 ? entityAttributes7.RepositoryName : (string) null;
                    break;
                  default:
                    throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("IndexingUnitType:{0} is not supported as parent for TemporaryIndexingUnit.", (object) this.RepositoryIndexingUnit.IndexingUnitType)));
                }
              }
              this.ProjectIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.RepositoryIndexingUnit.ParentUnitId);
              this.PopulateProjectPropertiesFromProjectIndexingUnit(this.ProjectIndexingUnit);
              this.ProjectId = new Guid?(this.ProjectIndexingUnit.TFSEntityId);
              this.CollectionIndexingUnit = indexingUnitDataAccess.GetIndexingUnit(this.RequestContext, this.ProjectIndexingUnit.ParentUnitId);
              break;
            }
            goto label_43;
          default:
            goto label_43;
        }
        if ((this.IndexingUnit.IndexingUnitType == "Project" || this.IndexingUnit.IsRepository() || this.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit" || this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit") && string.IsNullOrWhiteSpace(this.ProjectName))
          this.ProjectName = this.GetProjectNameFromTFS();
        if (!this.IndexingUnit.IsRepository() && !(this.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit") && !(this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit") || !string.IsNullOrWhiteSpace(this.RepositoryName))
          return;
        throw new IndexerException("RepositoryName cannot be null or empty.");
      }
label_43:
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("IndexingUnitType:{0} is not supported.", (object) this.IndexingUnit.IndexingUnitType)));
    }

    internal void PopulateProjectPropertiesFromProjectIndexingUnit(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit)
    {
      if (projectIndexingUnit.IndexingUnitType != "Project")
        throw new ArgumentException("Unsupported indexingunit type is passed.");
      if (projectIndexingUnit.EntityType.Name != "Code" && projectIndexingUnit.EntityType.Name != "Wiki")
        throw new ArgumentException("Unsupported entity type is passed.");
      this.ProjectName = string.IsNullOrWhiteSpace(projectIndexingUnit.Properties?.Name) ? (projectIndexingUnit.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes1 ? entityAttributes1.ProjectName : (string) null) : projectIndexingUnit.Properties.Name;
      this.ProjectVisibility = projectIndexingUnit.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes2 ? entityAttributes2.ProjectVisibility : ProjectVisibility.Private;
    }

    internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetIndexingUnitForExtractingIndexInfo()
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit extractingIndexInfo = this.IndexingUnit;
      string str = "The indexingUnit is in a corrupted state, its ParentUnitId points to an indexingUnit which does not exist. To recover from this, delete the corrupted units and reindex.";
      if (extractingIndexInfo.IndexingUnitType == "TemporaryIndexingUnit")
      {
        extractingIndexInfo = this.IndexingUnitDataAccess.GetIndexingUnit(this.RequestContext, extractingIndexInfo.ParentUnitId);
        if (extractingIndexInfo == null)
          throw new InvalidOperationException("Could not fetch parent of temporary indexing unit for extracting index info." + str);
      }
      if (extractingIndexInfo.IndexingUnitType == "ScopedIndexingUnit")
      {
        extractingIndexInfo = this.IndexingUnitDataAccess.GetIndexingUnit(this.RequestContext, extractingIndexInfo.ParentUnitId);
        if (extractingIndexInfo == null)
          throw new InvalidOperationException("Could not fetch parent of scoped indexing unit for extracting index info." + str);
      }
      if (extractingIndexInfo.IndexingUnitType == "Project" && !extractingIndexInfo.IsIndexingIndexNameAvailable())
      {
        extractingIndexInfo = this.CollectionIndexingUnit;
        if (extractingIndexInfo == null)
          throw new InvalidOperationException("Could not fetch collection indexing unit for project indexing unit to extract index info." + str);
      }
      if (extractingIndexInfo.IsRepository() && !extractingIndexInfo.IsIndexingIndexNameAvailable())
      {
        extractingIndexInfo = this.CollectionIndexingUnit;
        if (extractingIndexInfo == null)
          throw new InvalidOperationException("Could not fetch collection indexing unit for repository indexing unit to extract index info." + str);
      }
      return extractingIndexInfo;
    }

    private void Initialize(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      string searchPlatformConnectionString,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      this.m_indexingUnitForStore = this.GetIndexingUnitForTempStore(this.IndexingUnit, this.DataAccessFactory);
      this.InitializeSettings(requestContext);
      this.ProvisioningContext = new ProvisioningContext(this, indexingUnit, this.ServiceSettings.JobAgentSearchPlatformSettings, searchPlatformConnectionString);
      this.IndexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
    }

    private void InitializeSettings(IVssRequestContext requestContext)
    {
      this.FeederSettings = new FeederSettings(requestContext, this.IndexingUnit.EntityType);
      this.CrawlerSettings = new CrawlerSettings(requestContext, this.IndexingUnit.EntityType);
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetIndexingUnitForTempStore(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IDataAccessFactory DataAccessFactory)
    {
      return this.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit" ? DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(this.RequestContext, this.IndexingUnit.ParentUnitId) : indexingUnit;
    }
  }
}
