// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo.ProjectContract
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
  public class ProjectContract : ProjectRepoContractBase
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
      },
      {
        "collectionId",
        "collectionId"
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
        "collectionId",
        "collectionId"
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
        "tags.casechangeanalyzed",
        "tags"
      },
      {
        "collectionNameAnalyzed.casechangeanalyzed",
        "collectionName"
      }
    };

    [Text(Name = "description")]
    public string Description { get; set; }

    [Keyword(Name = "descriptionMetadata")]
    public string DescriptionMetadata { get; set; }

    [Number(Name = "likesCount")]
    public int? LikesCount { get; set; }

    [Text(Name = "tags")]
    public string[] Tags { get; set; }

    [Number(Name = "activityCount1day")]
    public int? ActivityCount1day { get; set; }

    [Number(Name = "activityCount7day")]
    public int? ActivityCount7days { get; set; }

    [Number(Name = "activityCount30days")]
    public int? ActivityCount30days { get; set; }

    [Number(Name = "TrendFactor1Day")]
    public int? TrendFactor1Day { get; set; }

    [Number(Name = "TrendFactor7Days")]
    public int? TrendFactor7Days { get; set; }

    [Number(Name = "TrendFactor30Days")]
    public int? TrendFactor30Days { get; set; }

    [Object(Name = "activityStats")]
    public ActivityTuple[] AggregatedActivity { get; set; }

    [Number(Name = "languages")]
    public string[] ProjectLanguages { get; set; }

    [Keyword(Name = "url")]
    public string Url { get; set; }

    [Keyword(Name = "imageUrl")]
    public string ImageUrl { get; set; }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.ProjectContract;

    public override void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      object data,
      IMetaDataStore metaDataStore,
      ParsedData parsedData,
      ProvisionerSettings settings,
      byte[] originalContent = null)
    {
      this.SetMetadataProperties(metaDataStore);
      string description = new TextEncoding().GetString(parsedData.Content);
      this.m_size = parsedData.Content.Length;
      ProjectRepositoryEntityMetadata projectMetadata = data as ProjectRepositoryEntityMetadata;
      this.PopulateFileContractDetails(metaDataStore, projectMetadata, description);
    }

    public virtual void PopulateFileContractDetails(
      IMetaDataStore metaDataStore,
      ProjectRepositoryEntityMetadata projectMetadata,
      string description)
    {
      this.SetMetadataProperties(metaDataStore);
      this.DocumentId = projectMetadata.EntityId;
      this.Name = projectMetadata.EntityName;
      this.Description = description;
      this.DescriptionMetadata = string.IsNullOrWhiteSpace(this.Description) ? "Empty" : "NonEmpty";
      this.Item = this.DocumentId;
      this.LikesCount = new int?(projectMetadata.ProjectLikesCount);
      this.Tags = projectMetadata.ProjectTags;
      this.Visibility = projectMetadata.ProjectVisibility;
      this.LastUpdatedDate = projectMetadata.EntityLastUpdated;
      this.ActivityCount1day = new int?(projectMetadata.EntityActivityCount1day);
      this.ActivityCount7days = new int?(projectMetadata.EntityActivityCount7days);
      this.ActivityCount30days = new int?(projectMetadata.EntityActivityCount30days);
      this.TrendFactor1Day = new int?(projectMetadata.EntityTrendFactor1Day);
      this.TrendFactor7Days = new int?(projectMetadata.EntityTrendFactor7Days);
      this.TrendFactor30Days = new int?(projectMetadata.EntityTrendFactor30Days);
      this.Url = projectMetadata.Url;
      this.ProjectLanguages = projectMetadata.ProjectLanguages;
      this.ImageUrl = projectMetadata.ImageUrl;
      this.JoinField = (JoinField) nameof (ProjectContract);
      List<ActivityTuple> activityTupleList = new List<ActivityTuple>();
      foreach (Tuple<DateTime, int> tuple in projectMetadata.EntityRecentActivity)
        activityTupleList.Add(new ActivityTuple(tuple.Item1, tuple.Item2));
      this.AggregatedActivity = activityTupleList.ToArray();
    }

    public override string GetFieldNameForStoredField(string storedField)
    {
      string str;
      return ProjectContract.s_storedFieldToFieldMapping.TryGetValue(storedField, out str) ? str : storedField;
    }

    public override string GetStoredFieldForFieldName(string field)
    {
      string str;
      return ProjectContract.s_fieldToStoredFieldMapping.TryGetValue(field, out str) ? str : field;
    }

    public override string GetStoredFieldValue(string field, string fieldValue) => fieldValue;

    public override string GetSearchFieldForType(string type) => type;

    public override int GetSize() => this.m_size;

    public override bool IsSourceEnabled(IVssRequestContext requestContext) => true;
  }
}
