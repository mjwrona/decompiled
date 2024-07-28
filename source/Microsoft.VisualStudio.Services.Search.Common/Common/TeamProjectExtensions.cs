// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.TeamProjectExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class TeamProjectExtensions
  {
    public static VersionControlType GetDefaultVersionControlType(
      this TeamProject projectWithCapabilities)
    {
      if (projectWithCapabilities == null)
        throw new ArgumentNullException(nameof (projectWithCapabilities));
      Dictionary<string, string> dictionary;
      projectWithCapabilities.Capabilities.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlCapabilityName, out dictionary);
      if (dictionary == null)
        throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Version Control info not found for the project {0}", (object) projectWithCapabilities.Id)));
      string str;
      dictionary.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlCapabilityAttributeName, out str);
      if (str == null)
        throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Source Control Type info not found for the project {0}", (object) projectWithCapabilities.Id)));
      switch (str.ToLower(CultureInfo.InvariantCulture))
      {
        case "git":
          return VersionControlType.Git;
        case "tfvc":
          return VersionControlType.TFVC;
        case "custom":
          return VersionControlType.Custom;
        default:
          throw new InvalidDataException(FormattableString.Invariant(FormattableStringFactory.Create("Version control type not found {0}", (object) str)));
      }
    }

    public static VersionControlType GetSupportedVersionControlType(
      this TeamProject projectWithCapabilities)
    {
      if (projectWithCapabilities == null)
        throw new ArgumentNullException(nameof (projectWithCapabilities));
      VersionControlType versionControlType = VersionControlType.Unknown;
      if (projectWithCapabilities.Capabilities == null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No capabilities for project {0}", (object) projectWithCapabilities.Id));
      Dictionary<string, string> dictionary;
      projectWithCapabilities.Capabilities.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlCapabilityName, out dictionary);
      if (dictionary == null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Version Control info not found for the project {0}", (object) projectWithCapabilities.Id));
      string a;
      dictionary.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlCapabilityAttributeName, out a);
      if (a == null)
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Source Control Type info not found for the project {0}", (object) projectWithCapabilities.Id));
      string str1;
      dictionary.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlGitEnabledAttributeName, out str1);
      string str2;
      dictionary.TryGetValue(TeamProjectCapabilitiesConstants.VersionControlTfvcEnabledAttributeName, out str2);
      bool boolean1 = Convert.ToBoolean(str1, (IFormatProvider) CultureInfo.InvariantCulture);
      bool boolean2 = Convert.ToBoolean(str2, (IFormatProvider) CultureInfo.InvariantCulture);
      if (string.Equals(a, "tfvc", StringComparison.OrdinalIgnoreCase))
        versionControlType = boolean1 ? VersionControlType.GitAndTFVC : VersionControlType.TFVC;
      else if (string.Equals(a, "git", StringComparison.OrdinalIgnoreCase))
        versionControlType = boolean2 ? VersionControlType.GitAndTFVC : VersionControlType.Git;
      return versionControlType;
    }

    public static string GetProjectUrl(this TeamProject project)
    {
      if (project == null)
        throw new ArgumentNullException(nameof (project));
      return !(project.Links?.Links["web"] is ReferenceLink link) ? (string) null : link.Href;
    }

    public static string GetProjectImageUrl(
      this TeamProject project,
      IVssRequestContext requestContext)
    {
      if (project == null)
        throw new ArgumentNullException(nameof (project));
      IdentityService service = requestContext.GetService<IdentityService>();
      string name = "Microsoft.TeamFoundation.Identity.Image.Data";
      IVssRequestContext requestContext1 = requestContext;
      Guid[] identityIds = new Guid[1]
      {
        project.DefaultTeam.Id
      };
      string[] propertyNameFilters = new string[1]{ name };
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(requestContext1, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) propertyNameFilters).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
      object obj = (object) null;
      identity?.TryGetProperty(name, out obj);
      return obj == null ? string.Empty : project.GetProjectUrl() + "/_api/_common/IdentityImage?id=" + (object) project.DefaultTeam.Id;
    }

    public static IndexingUnit ToTfvcRepoCodeIndexingUnit(
      this TeamProjectReference teamProject,
      int parentIndexingUnitId,
      bool isShadowIndexingUnit = false)
    {
      Guid tfsEntityId = teamProject != null ? teamProject.Id : throw new ArgumentNullException(nameof (teamProject));
      CodeEntityType instance = CodeEntityType.GetInstance();
      bool flag = isShadowIndexingUnit;
      int parentUnitId = parentIndexingUnitId;
      int num = flag ? 1 : 0;
      IndexingUnit codeIndexingUnit = new IndexingUnit(tfsEntityId, "TFVC_Repository", (IEntityType) instance, parentUnitId, num != 0);
      TfvcCodeRepoTFSAttributes repoTfsAttributes = new TfvcCodeRepoTFSAttributes();
      repoTfsAttributes.RepositoryName = "$/" + teamProject.Name;
      codeIndexingUnit.TFSEntityAttributes = (TFSEntityAttributes) repoTfsAttributes;
      TfvcCodeRepoIndexingProperties indexingProperties = new TfvcCodeRepoIndexingProperties();
      indexingProperties.Name = "$/" + teamProject.Name;
      codeIndexingUnit.Properties = (IndexingProperties) indexingProperties;
      return codeIndexingUnit;
    }

    public static IndexingUnit ToProjectCodeIndexingUnit(
      this TeamProjectReference teamProject,
      int parentIndexingUnitId)
    {
      if (teamProject == null)
        throw new ArgumentNullException(nameof (teamProject));
      IndexingUnit codeIndexingUnit = new IndexingUnit(teamProject.Id, "Project", (IEntityType) CodeEntityType.GetInstance(), parentIndexingUnitId);
      codeIndexingUnit.TFSEntityAttributes = (TFSEntityAttributes) new ProjectCodeTFSAttributes()
      {
        ProjectName = teamProject.Name
      };
      ProjectCodeIndexingProperties indexingProperties = new ProjectCodeIndexingProperties();
      indexingProperties.Name = teamProject.Name;
      codeIndexingUnit.Properties = (IndexingProperties) indexingProperties;
      return codeIndexingUnit;
    }

    public static IndexingUnit ToProjectWorkItemIndexingUnit(
      this TeamProjectReference teamProject,
      IndexingUnit collectionIndexingUnit,
      bool isShadowIndexingUnit = false)
    {
      if (teamProject == null)
        throw new ArgumentNullException(nameof (teamProject));
      if (collectionIndexingUnit == null)
        throw new ArgumentNullException(nameof (collectionIndexingUnit));
      IndexInfo indexInfo = collectionIndexingUnit.IsIndexingIndexNameAvailable() ? collectionIndexingUnit.GetIndexInfo() : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Work Item Collection Indexing Unit [{0}] does not have indexing index name populated.", (object) collectionIndexingUnit)), SearchServiceErrorCode.IndexingIndexNameDoesNotExist);
      List<IndexInfo> indexInfoList = new List<IndexInfo>()
      {
        new IndexInfo()
        {
          IndexName = indexInfo.IndexName,
          Version = indexInfo.Version,
          IsShadow = false,
          DocumentContractType = collectionIndexingUnit.GetIndexInfo().DocumentContractType
        }
      };
      IndexingUnit itemIndexingUnit = new IndexingUnit(teamProject.Id, "Project", (IEntityType) WorkItemEntityType.GetInstance(), collectionIndexingUnit.IndexingUnitId, isShadowIndexingUnit);
      itemIndexingUnit.TFSEntityAttributes = (TFSEntityAttributes) new ProjectWorkItemTFSAttributes()
      {
        ProjectName = teamProject.Name,
        ProjectId = teamProject.Id
      };
      ProjectWorkItemIndexingProperties indexingProperties = new ProjectWorkItemIndexingProperties();
      indexingProperties.Name = teamProject.Name;
      indexingProperties.IndexIndices = indexInfoList;
      indexingProperties.WorkItemIndexJobYieldData = new WorkItemIndexJobYieldData();
      itemIndexingUnit.Properties = (IndexingProperties) indexingProperties;
      return itemIndexingUnit;
    }

    public static IndexingUnit ToProjectWorkItemPrimaryIndexingUnit(
      this TeamProjectReference teamProject,
      IndexingUnit collectionIndexingUnit,
      bool isShadowIndexingUnit = false)
    {
      if (teamProject == null)
        throw new ArgumentNullException(nameof (teamProject));
      if (collectionIndexingUnit == null)
        throw new ArgumentNullException(nameof (collectionIndexingUnit));
      IndexInfo indexInfo = collectionIndexingUnit.IsIndexingIndexNameAvailable() ? collectionIndexingUnit.GetQueryIndexInfo() : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Work Item Collection Indexing Unit [{0}] does not have indexing index name populated.", (object) collectionIndexingUnit)), SearchServiceErrorCode.IndexingIndexNameDoesNotExist);
      List<IndexInfo> indexInfoList = new List<IndexInfo>()
      {
        new IndexInfo()
        {
          IndexName = indexInfo.IndexName,
          Version = indexInfo.Version,
          IsShadow = false,
          DocumentContractType = collectionIndexingUnit.GetIndexInfo().DocumentContractType
        }
      };
      IndexingUnit primaryIndexingUnit = new IndexingUnit(teamProject.Id, "Project", (IEntityType) WorkItemEntityType.GetInstance(), collectionIndexingUnit.IndexingUnitId, isShadowIndexingUnit);
      primaryIndexingUnit.TFSEntityAttributes = (TFSEntityAttributes) new ProjectWorkItemTFSAttributes()
      {
        ProjectName = teamProject.Name,
        ProjectId = teamProject.Id
      };
      ProjectWorkItemIndexingProperties indexingProperties = new ProjectWorkItemIndexingProperties();
      indexingProperties.Name = teamProject.Name;
      indexingProperties.IndexIndices = indexInfoList;
      indexingProperties.WorkItemIndexJobYieldData = new WorkItemIndexJobYieldData();
      primaryIndexingUnit.Properties = (IndexingProperties) indexingProperties;
      return primaryIndexingUnit;
    }
  }
}
