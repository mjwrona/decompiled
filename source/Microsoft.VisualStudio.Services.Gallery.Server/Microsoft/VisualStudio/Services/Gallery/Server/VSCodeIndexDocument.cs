// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.VSCodeIndexDocument
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Search;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class VSCodeIndexDocument
  {
    [Key]
    [IsSearchable]
    [IsFilterable]
    public string ExtensionId { get; set; }

    [IsSearchable]
    [IsSortable]
    [IndexAnalyzer("DefaultIndexAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public string ExtensionName { get; set; }

    [IsSearchable]
    [IsFilterable]
    [IsSortable]
    [IndexAnalyzer("DefaultIndexAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public string ExtensionDisplayName { get; set; }

    [IsSearchable]
    [IsFilterable]
    [IsSortable]
    [IndexAnalyzer("PrefixAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public string ExtensionDisplayNameForPrefixMatch { get; set; }

    [IsSearchable]
    [IndexAnalyzer("DefaultIndexAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public string ShortDescription { get; set; }

    [IsSearchable]
    [IndexAnalyzer("PrefixAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public string ShortDescriptionForPrefixMatch { get; set; }

    public string PublisherName { get; set; }

    [IsSearchable]
    [IsFilterable]
    [IsSortable]
    [IndexAnalyzer("DefaultIndexAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public string PublisherDisplayName { get; set; }

    [IsSearchable]
    [IsFilterable]
    [IsSortable]
    [IndexAnalyzer("PrefixAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public string PublisherDisplayNameForPrefixMatch { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsSearchable]
    [IsFilterable]
    [IsRetrievable(false)]
    [Analyzer("KeywordIndexAnalyzer")]
    public string PublisherDisplayNameForExactMatch { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsFilterable]
    [IsRetrievable(false)]
    public bool? IsDomainVerified { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsSearchable]
    [IsRetrievable(false)]
    [Analyzer("KeywordIndexAnalyzer")]
    public string ExtensionFullyQualifiedNameForExactMatch { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsSearchable]
    [IsRetrievable(false)]
    [Analyzer("KeywordIndexAnalyzer")]
    public string ExtensionNameForExactMatch { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsSearchable]
    [IsRetrievable(false)]
    [Analyzer("KeywordIndexAnalyzer")]
    public string PublisherNameForExactMatch { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsSearchable]
    [IsRetrievable(false)]
    [Analyzer("KeywordIndexAnalyzer")]
    public string ExtensionDisplayNameForExactMatch { get; set; }

    [IsFilterable]
    public List<string> ExtensionFlags { get; set; }

    [IsFilterable]
    [IsRetrievable(false)]
    public List<string> PublisherFlags { get; set; }

    public string Publisher { get; set; }

    [IsSortable]
    public DateTime? LastUpdated { get; set; }

    public DateTime? PublishedDate { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [IsSortable]
    public DateTime? ReleasedDate { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Tags { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsRetrievable(false)]
    [IsSearchable]
    [IsFilterable]
    [IndexAnalyzer("DefaultIndexAnalyzer")]
    [SearchAnalyzer("SearchTermAnalyzer")]
    public List<string> SearchableTags { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsFilterable]
    [IsFacetable]
    public List<string> Categories { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsFilterable]
    [IsFacetable]
    [IsRetrievable(false)]
    public List<string> TargetPlatforms { get; set; }

    [IsSortable]
    [IsFilterable]
    public double DownloadCount { get; set; }

    [IsSortable]
    [IsFilterable]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public double? InstallCount { get; set; }

    [IsSortable]
    [IsFilterable]
    [IsRetrievable(false)]
    public double WeightedRating { get; set; }

    [IsSortable]
    public double TrendingScore { get; set; }

    [IsFilterable]
    [IsRetrievable(false)]
    public List<string> InstallationTargetList { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [IsFilterable]
    [IsRetrievable(false)]
    public List<string> SearchableMetadata { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Metadata { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string DeploymentType { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string ValidatedVersions { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string AllVersions { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Statistics { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string SharedWith { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string InstallationTargets { get; set; }

    [IsFilterable]
    [IsRetrievable(false)]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> EnterpriseSharedWithIds { get; set; }

    [IsFilterable]
    [IsRetrievable(false)]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> OrgSharedWithIds { get; set; }

    [IsFilterable]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Lcids { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [IsFilterable]
    [IsFacetable]
    [IsRetrievable(false)]
    public string ProjectType { get; set; }

    public VSCodeIndexDocument ShallowCopy() => (VSCodeIndexDocument) this.MemberwiseClone();
  }
}
