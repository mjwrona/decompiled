// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.RepositoryContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Nest;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo
{
  public class RepositoryContract : ProjectRepoContractBase
  {
    private int m_size;
    [StaticSafe]
    private static readonly Dictionary<string, string> s_storedFieldToFieldMapping = new Dictionary<string, string>()
    {
      {
        "collectionName",
        "collectionNameOriginal"
      },
      {
        "organizationName",
        "organizationNameOriginal"
      }
    };
    [StaticSafe]
    private static readonly Dictionary<string, string> s_fieldToStoredFieldMapping = new Dictionary<string, string>()
    {
      {
        "collectionNameOriginal",
        "collectionName"
      },
      {
        "organizationNameOriginal",
        "organizationName"
      },
      {
        "collectionNameAnalyzed",
        "collectionName"
      },
      {
        "languages.analysed",
        "languages"
      },
      {
        "name.casechangeanalyzed",
        "name"
      },
      {
        "projectName.casechangeanalyzed",
        "projectName"
      },
      {
        "collectionNameAnalyzed.casechangeanalyzed",
        "collectionName"
      }
    };

    [Text(Name = "readme")]
    public string Readme { get; set; }

    [Keyword(Name = "readmeMetadata")]
    public string ReadmeMetadata { get; set; }

    [Text(Name = "readmeLinks")]
    public string[] ReadmeLinks { get; set; }

    [Keyword(Name = "readmeFilePath")]
    public string ReadmeFilePath { get; set; }

    [Number(Name = "forks")]
    public int? ForksCount { get; set; }

    [Object(Name = "activityStats")]
    public ActivityTuple[] ActivityStats { get; set; }

    [Number(Name = "activityCount1day")]
    public int? ActivityCount1day { get; set; }

    [Number(Name = "activityCount7day")]
    public int? ActivityCount7days { get; set; }

    [Number(Name = "activityCount30days")]
    public int? ActivityCount30days { get; set; }

    [Text(Name = "languages")]
    public string[] Languages { get; set; }

    [Keyword(Name = "versionControl")]
    public string VersionControl { get; set; }

    [Keyword(Name = "url")]
    public string Url { get; set; }

    [Text(Name = "projectName")]
    public string ProjectName { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.RepositoryContract;

    public override void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      object data,
      IMetaDataStore metaDataStore,
      ParsedData parsedData,
      ProvisionerSettings settings,
      byte[] originalContent = null)
    {
      this.SetMetadataProperties(metaDataStore);
      string readmeRaw = new TextEncoding().GetString(parsedData.Content);
      this.m_size = parsedData.Content.Length;
      ProjectRepositoryEntityMetadata repoMetadata = data as ProjectRepositoryEntityMetadata;
      this.PopulateFileContractDetails(metaDataStore, repoMetadata, readmeRaw);
    }

    public virtual void PopulateFileContractDetails(
      IMetaDataStore metaDataStore,
      ProjectRepositoryEntityMetadata repoMetadata,
      string readmeRaw)
    {
      this.SetMetadataProperties(metaDataStore);
      this.DocumentId = repoMetadata.EntityId;
      this.Name = repoMetadata.EntityName;
      this.Item = this.DocumentId;
      this.ParentDocumentId = repoMetadata.EntityParentId;
      this.Visibility = repoMetadata.ProjectVisibility;
      this.ForksCount = new int?(repoMetadata.RepositoryForks);
      this.Languages = repoMetadata.RepositoryLanguages;
      this.LastUpdatedDate = repoMetadata.EntityLastUpdated;
      this.VersionControl = repoMetadata.EntityVersionControlType.ToString();
      this.ActivityCount1day = new int?(repoMetadata.EntityActivityCount1day);
      this.ActivityCount7days = new int?(repoMetadata.EntityActivityCount7days);
      this.ActivityCount30days = new int?(repoMetadata.EntityActivityCount30days);
      this.Url = repoMetadata.Url;
      this.ProjectName = repoMetadata.EntityParentName;
      this.ReadmeFilePath = repoMetadata.ReadmeFilePath;
      this.JoinField = JoinField.Link((RelationName) nameof (RepositoryContract), (Id) repoMetadata.EntityParentId);
      this.ReadmeMetadata = string.IsNullOrWhiteSpace(this.Readme) ? "Empty" : "NonEmpty";
      List<ActivityTuple> activityTupleList = new List<ActivityTuple>();
      foreach (Tuple<DateTime, int> tuple in repoMetadata.EntityRecentActivity)
        activityTupleList.Add(new ActivityTuple(tuple.Item1, tuple.Item2));
      this.ActivityStats = activityTupleList.ToArray();
    }

    public override string GetFieldNameForStoredField(string storedField)
    {
      string str;
      return RepositoryContract.s_storedFieldToFieldMapping.TryGetValue(storedField, out str) ? str : storedField;
    }

    public override string GetStoredFieldForFieldName(string field)
    {
      string str;
      return RepositoryContract.s_fieldToStoredFieldMapping.TryGetValue(field, out str) ? str : field;
    }

    public override string GetStoredFieldValue(string field, string fieldValue) => fieldValue;

    public override string GetSearchFieldForType(string type) => type;

    public override int GetSize() => this.m_size;

    public override bool IsSourceEnabled(IVssRequestContext requestContext) => true;
  }
}
