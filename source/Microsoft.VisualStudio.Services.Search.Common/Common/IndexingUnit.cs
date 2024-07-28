// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexingUnit
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.Code;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class IndexingUnit
  {
    private string m_indexingUnitTypeExtended;
    public const int NullParentIndexingUnitId = -1;

    public IndexingUnit(
      Guid tfsEntityId,
      string indexingUnitType,
      IEntityType entityType,
      int parentUnitId,
      bool isShadow = false)
    {
      this.TFSEntityId = tfsEntityId;
      this.IndexingUnitType_Extended = IndexingUnit.GetIndexingUnitTypeExtended(indexingUnitType, isShadow);
      this.EntityType = entityType;
      this.ParentUnitId = parentUnitId;
    }

    internal string IndexingUnitType_Extended
    {
      get => this.m_indexingUnitTypeExtended;
      set
      {
        this.m_indexingUnitTypeExtended = value;
        bool isShadow;
        this.IndexingUnitType = IndexingUnit.ParseIndexingUnitTypeExtended(this.m_indexingUnitTypeExtended, out isShadow);
        this.IsShadow = isShadow;
      }
    }

    public Guid TFSEntityId { get; }

    public string IndexingUnitType { get; private set; }

    public bool IsShadow { get; private set; }

    public IEntityType EntityType { get; }

    public int IndexingUnitId { get; set; }

    public int ParentUnitId { get; private set; }

    public Guid? AssociatedJobId { get; set; }

    public DateTime CreatedTimeUTC { get; set; }

    public TFSEntityAttributes TFSEntityAttributes { get; set; }

    public IndexingProperties Properties { get; set; }

    internal static string GetIndexingUnitTypeExtended(string indexingUnitType, bool isShadow) => !isShadow ? indexingUnitType : indexingUnitType + "*";

    internal static string ParseIndexingUnitTypeExtended(
      string indexingUnitType_Extended,
      out bool isShadow)
    {
      isShadow = indexingUnitType_Extended != null && indexingUnitType_Extended.Contains("*");
      return indexingUnitType_Extended?.Replace("*", "");
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(TFSEntityId: ");
      stringBuilder.Append((object) this.TFSEntityId);
      stringBuilder.Append(", IndexingUnitType: ");
      stringBuilder.Append(this.IndexingUnitType);
      stringBuilder.Append(", EntityType: ");
      stringBuilder.Append((object) this.EntityType);
      stringBuilder.Append(", IndexingUnitId: ");
      stringBuilder.Append(this.IndexingUnitId);
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
        return true;
      return obj is IndexingUnit indexingUnit && indexingUnit.TFSEntityId.CompareTo(this.TFSEntityId) == 0 && indexingUnit.IndexingUnitType == this.IndexingUnitType && indexingUnit.EntityType.Name == this.EntityType.Name;
    }

    public override int GetHashCode() => this.TFSEntityId.GetHashCode();

    public bool IsLargeCollection(IVssRequestContext requestContext)
    {
      TFSEntityAttributes entityAttributes = this.TFSEntityAttributes;
      if (this.EntityType.Name == "WorkItem")
        return requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsLargeWorkItemCollection");
      return this.EntityType.Name == "Code" && requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsLargeCodeCollection");
    }

    public bool IsLargeRepository(IVssRequestContext requestContext)
    {
      string fromTfsAttributes = this.GetRepositoryNameFromTFSAttributes();
      if (fromTfsAttributes == null || string.IsNullOrWhiteSpace(fromTfsAttributes))
        return false;
      string currentHostConfigValue = requestContext.GetCurrentHostConfigValue("/Service/ALMSearch/Settings/LargeRepositoriesName");
      if (!string.IsNullOrWhiteSpace(currentHostConfigValue))
      {
        if (((IEnumerable<string>) currentHostConfigValue.Split(',')).Contains<string>(fromTfsAttributes))
          return true;
      }
      return false;
    }

    public string GetCollectionName() => !(this.TFSEntityAttributes is CollectionAttributes entityAttributes) ? (string) null : entityAttributes.CollectionName;

    public string GetTFSEntityName()
    {
      switch (this.IndexingUnitType)
      {
        case "Git_Repository":
        case "TFVC_Repository":
        case "CustomRepository":
          return this.GetRepositoryNameFromTFSAttributes();
        case "Project":
          return this.GetProjectNameFromTFSAttributesIfProjectIUElseNull();
        case "ScopedIndexingUnit":
          return this.GetScopePathFromTFSAttributesIfScopedIUElseNull();
        case "Collection":
          return this.GetCollectionName();
        default:
          throw new NotImplementedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetTFSEntityName is not supported for [{0}] yet", (object) this.IndexingUnitType));
      }
    }

    public string GetRepositoryNameFromTFSAttributes()
    {
      string repositoryName;
      if (this.IndexingUnitType == "CustomRepository")
      {
        if (!(this.TFSEntityAttributes is CustomRepoCodeTFSAttributes entityAttributes))
          return (string) null;
        repositoryName = entityAttributes.RepositoryName;
      }
      else
      {
        if (!(this.TFSEntityAttributes is CodeRepoTFSAttributes entityAttributes))
          return (string) null;
        repositoryName = entityAttributes.RepositoryName;
      }
      return repositoryName;
    }

    public string GetProjectNameFromTFSAttributesIfProjectIUElseNull()
    {
      if (!(this.IndexingUnitType == "Project"))
        return (string) null;
      return !(this.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes) ? (string) null : entityAttributes.ProjectName;
    }

    public int GetBranchCountFromTFSAttributesIfGitRepo() => !(this.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes) ? 0 : entityAttributes.BranchesToIndex.Count;

    public string GetScopePathFromTFSAttributesIfScopedIUElseNull()
    {
      if (!(this.IndexingUnitType == "ScopedIndexingUnit"))
        return (string) null;
      return !(this.TFSEntityAttributes is ScopedGitRepositoryAttributes entityAttributes) ? (string) null : entityAttributes.ScopePath;
    }

    public bool IsRepository() => this.IndexingUnitType == "CustomRepository" || this.IndexingUnitType == "Git_Repository" || this.IndexingUnitType == "TFVC_Repository";

    public IndexInfo GetIndexInfo()
    {
      IndexingProperties properties = this.Properties;
      int num1;
      if (properties == null)
      {
        num1 = 0;
      }
      else
      {
        int? count = properties.IndexIndices?.Count;
        int num2 = 1;
        num1 = count.GetValueOrDefault() == num2 & count.HasValue ? 1 : 0;
      }
      return num1 == 0 ? (IndexInfo) null : this.Properties.IndexIndices[0];
    }

    public IndexInfo GetQueryIndexInfo()
    {
      List<IndexInfo> queryIndices = this.Properties.QueryIndices;
      // ISSUE: explicit non-virtual call
      return (queryIndices != null ? (__nonvirtual (queryIndices.Count) == 1 ? 1 : 0) : 0) == 0 ? (IndexInfo) null : this.Properties.QueryIndices[0];
    }

    public bool IsIndexingIndexNameAvailable() => !string.IsNullOrWhiteSpace(this.GetIndexingIndexName());

    public string GetIndexingIndexName() => this.Properties != null && this.Properties.IndexIndices != null && this.Properties.IndexIndices.Count == 1 && !string.IsNullOrWhiteSpace(this.Properties.IndexIndices.First<IndexInfo>().IndexName) ? this.Properties.IndexIndices.First<IndexInfo>().IndexName : (string) null;

    public string GetQueryIndexName() => this.Properties != null && this.Properties.QueryIndices != null && this.Properties.QueryIndices.Count == 1 && !string.IsNullOrWhiteSpace(this.Properties.QueryIndices.First<IndexInfo>().IndexName) ? this.Properties.QueryIndices.First<IndexInfo>().IndexName : (string) null;

    public static IndexingUnit CreateProjectIndexingUnitFrom(
      GitRepository repository,
      int parentIndexingUnitId,
      IEntityType entityType = null)
    {
      if (repository == null)
        throw new ArgumentNullException(nameof (repository));
      if (entityType == null)
        entityType = (IEntityType) CodeEntityType.GetInstance();
      IndexingUnit indexingUnitFrom = new IndexingUnit(repository.ProjectReference.Id, "Project", entityType, parentIndexingUnitId);
      indexingUnitFrom.TFSEntityAttributes = (TFSEntityAttributes) new ProjectCodeTFSAttributes()
      {
        ProjectName = repository.ProjectReference.Name,
        ProjectVisibility = repository.ProjectReference.Visibility
      };
      ProjectCodeIndexingProperties indexingProperties = new ProjectCodeIndexingProperties();
      indexingProperties.Name = repository.ProjectReference.Name;
      indexingProperties.ProjectVisibility = repository.ProjectReference.Visibility;
      indexingUnitFrom.Properties = (IndexingProperties) indexingProperties;
      return indexingUnitFrom;
    }

    public string GetTfsEntityIdAsNormalizedString() => this.TFSEntityId.ToString().NormalizeString();

    internal void SetIndexingUnitType(string indexingUnitType) => this.IndexingUnitType_Extended = indexingUnitType;

    public bool EraseIndexingWaterMarks(bool isShadowIndexing = false) => this.Properties != null && this.Properties.EraseIndexingWaterMarks(isShadowIndexing);
  }
}
