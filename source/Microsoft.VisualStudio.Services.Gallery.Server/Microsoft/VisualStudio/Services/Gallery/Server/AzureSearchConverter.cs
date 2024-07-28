// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureSearchConverter
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Azure.Search.Models;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class AzureSearchConverter : ISearchConverter
  {
    private const string s_area = "gallery";
    private const string s_layer = "SearchConverter";
    private readonly int maxSupportedVsIdeMajorVersion;
    private readonly int maxSupportedVsIdeMinorVersionIncrement;
    private readonly IReadOnlyCollection<int> supportedVsIdeMajorVersions;
    private readonly IReadOnlyCollection<string> supportedProductArchitectures;

    public AzureSearchConverter(
      int maxSupportedVsIdeMajorVersion,
      int maxSupportedVsIdeMinorVersionIncrement,
      IReadOnlyCollection<int> supportedVsIdeMajorVersions,
      IReadOnlyCollection<string> supportedProductArchitectures)
    {
      this.maxSupportedVsIdeMajorVersion = maxSupportedVsIdeMajorVersion;
      this.maxSupportedVsIdeMinorVersionIncrement = maxSupportedVsIdeMinorVersionIncrement;
      this.supportedVsIdeMajorVersions = supportedVsIdeMajorVersions ?? throw new ArgumentNullException(nameof (supportedVsIdeMajorVersions));
      this.supportedProductArchitectures = supportedProductArchitectures;
    }

    public List<AzureIndexDocument> ConvertExtensionObjectToIndexObject(
      IList<PublishedExtension> extensions,
      bool useNewIndexDefinition = false,
      bool useProductArchitectureInfo = false,
      bool isPlatformSpecificExtensionsForVSCodeEnabled = false,
      bool usePublisherDomainInfo = false)
    {
      List<AzureIndexDocument> indexObject = new List<AzureIndexDocument>();
      foreach (PublishedExtension extension in (IEnumerable<PublishedExtension>) extensions)
      {
        List<ExtensionVersion> extensionVersionList1 = new List<ExtensionVersion>();
        if (extension.IsVsCodeExtension())
          extensionVersionList1 = GalleryServerUtil.GetLatestValidatedExtensionVersionForEachTargetPlatform(extension.Versions);
        else if (extension.IsVsExtension() && GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(extension.Metadata))
        {
          IEnumerable<ExtensionVersion> extensionVersions = extension.Versions.Where<ExtensionVersion>((Func<ExtensionVersion, bool>) (extensionVersion => extensionVersion.Flags.HasFlag((Enum) ExtensionVersionFlags.Validated)));
          if (!extensionVersions.IsNullOrEmpty<ExtensionVersion>())
            extensionVersionList1 = extensionVersions.ToList<ExtensionVersion>();
        }
        else
        {
          ExtensionVersion extensionVersion = GalleryServerUtil.GetLatestValidatedExtensionVersion(extension.Versions);
          if (extensionVersion != null)
            extensionVersionList1.Add(extensionVersion);
        }
        if (extensionVersionList1.IsNullOrEmpty<ExtensionVersion>())
        {
          ExtensionVersion version = extension.Versions[0];
          if (version == null)
          {
            TeamFoundationTracingService.TraceRaw(12060108, TraceLevel.Error, "gallery", "SearchConverter", "Extension " + extension.Publisher.PublisherName + "." + extension.ExtensionName + " does not contain any validated version. Skipping indexing of this extension.");
            continue;
          }
          extensionVersionList1.Add(version);
        }
        AzureIndexDocument azureIndexDocument1 = new AzureIndexDocument();
        List<ServerExtensionVersion> extensionVersionList2 = JsonUtilities.Deserialize<List<ServerExtensionVersion>>(extensionVersionList1.Serialize<List<ExtensionVersion>>());
        for (int index1 = 0; index1 < extensionVersionList1.Count; ++index1)
        {
          ExtensionVersion extensionVersion1 = extensionVersionList1[index1];
          ServerExtensionVersion extensionVersion2 = extensionVersionList2[index1];
          int index2 = 0;
          while (true)
          {
            int num = index2;
            int? count = extensionVersion1.Files?.Count;
            int valueOrDefault = count.GetValueOrDefault();
            if (num < valueOrDefault & count.HasValue)
            {
              extensionVersion2.Files[index2].SerializableFileId = extensionVersion1.Files[index2].FileId;
              ++index2;
            }
            else
              break;
          }
        }
        azureIndexDocument1.ValidatedVersions = extensionVersionList2.Serialize<List<ServerExtensionVersion>>();
        azureIndexDocument1.ExtensionId = extension.ExtensionId.ToString();
        azureIndexDocument1.ExtensionName = extension.ExtensionName;
        azureIndexDocument1.ExtensionDisplayName = extension.DisplayName;
        azureIndexDocument1.ExtensionDisplayNameForPrefixMatch = extension.DisplayName.ToLower();
        azureIndexDocument1.ShortDescription = extension.ShortDescription;
        azureIndexDocument1.ShortDescriptionForPrefixMatch = extension.ShortDescription;
        azureIndexDocument1.PublisherName = extension.Publisher.PublisherName;
        azureIndexDocument1.PublisherDisplayName = extension.Publisher.DisplayName;
        azureIndexDocument1.PublisherDisplayNameForPrefixMatch = extension.Publisher.DisplayName.ToLower();
        azureIndexDocument1.PublisherDisplayNameForExactMatch = extension.Publisher.DisplayName;
        if (extension.IsVsCodeExtension())
          azureIndexDocument1.PublisherDisplayNameForExactMatch = extension.Publisher.DisplayName.ToLower();
        azureIndexDocument1.ExtensionNameForExactMatch = extension.ExtensionName;
        azureIndexDocument1.PublisherNameForExactMatch = extension.Publisher.PublisherName;
        azureIndexDocument1.ExtensionFullyQualifiedNameForExactMatch = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
        azureIndexDocument1.ExtensionDisplayNameForExactMatch = extension.DisplayName;
        azureIndexDocument1.Tags = extension.Tags;
        AzureIndexDocument azureIndexDocument2 = azureIndexDocument1;
        List<string> tags = extension.Tags;
        List<string> list = tags != null ? tags.Where<string>((Func<string, bool>) (x => !x.StartsWith("__"))).ToList<string>() : (List<string>) null;
        azureIndexDocument2.SearchableTags = list;
        if (extension.IsVsCodeExtension() && !azureIndexDocument1.SearchableTags.IsNullOrEmpty<string>())
          azureIndexDocument1.SearchableTags = azureIndexDocument1.SearchableTags.ConvertAll<string>((Converter<string, string>) (tag => tag.ToLower()));
        azureIndexDocument1.Categories = extension.Categories;
        azureIndexDocument1.ExtensionFlags = ((IEnumerable<string>) extension.Flags.ToString().Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).ToList<string>();
        azureIndexDocument1.LastUpdated = new DateTime?(extension.LastUpdated);
        azureIndexDocument1.PublishedDate = new DateTime?(extension.PublishedDate);
        azureIndexDocument1.ReleasedDate = new DateTime?(extension.ReleaseDate);
        azureIndexDocument1.DownloadCount = GalleryServerUtil.GetDownloadCount(extension.Statistics);
        if (usePublisherDomainInfo)
          azureIndexDocument1.IsDomainVerified = new bool?(extension.Publisher.IsDomainVerified);
        if (useNewIndexDefinition)
          azureIndexDocument1.InstallCount = new double?(GalleryServerUtil.GetInstallCount(extension.Statistics));
        azureIndexDocument1.WeightedRating = GalleryServerUtil.ExtractStatisticsValue(extension.Statistics, "weightedRating");
        azureIndexDocument1.TrendingScore = GalleryServerUtil.ExtractStatisticsValue(extension.Statistics, "trendingweekly");
        azureIndexDocument1.Publisher = extension.Publisher.Serialize<PublisherFacts>();
        azureIndexDocument1.PublisherFlags = ((IEnumerable<string>) extension.Publisher.Flags.ToString().Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).ToList<string>();
        azureIndexDocument1.InstallationTargets = extension.InstallationTargets.Serialize<List<InstallationTarget>>();
        ExtensionDeploymentTechnology deploymentTechnology = ExtensionDeploymentTechnology.Vsix;
        bool isVsix = deploymentTechnology.Equals((object) extension.DeploymentType);
        azureIndexDocument1.InstallationTargetList = this.GetModifiedInstallationTargetList(extension.InstallationTargets, useProductArchitectureInfo, isVsix);
        if (azureIndexDocument1.InstallationTargetList.IsNullOrEmpty<string>())
        {
          TeamFoundationTracingService.TraceRaw(12060108, TraceLevel.Error, "gallery", "SearchConverter", "Extension " + extension.Publisher.PublisherName + "." + extension.ExtensionName + " does not contain any valid installation targets. Skipping indexing of this extension.");
        }
        else
        {
          AzureIndexDocument azureIndexDocument3 = azureIndexDocument1;
          deploymentTechnology = extension.DeploymentType;
          string str = deploymentTechnology.ToString();
          azureIndexDocument3.DeploymentType = str;
          if (!extension.Statistics.IsNullOrEmpty<ExtensionStatistic>())
            azureIndexDocument1.Statistics = extension.Statistics.Serialize<List<ExtensionStatistic>>();
          if (!extension.SharedWith.IsNullOrEmpty<ExtensionShare>())
          {
            azureIndexDocument1.SharedWith = extension.SharedWith.Serialize<List<ExtensionShare>>();
            azureIndexDocument1.OrgSharedWithIds = extension.SharedWith.Where<ExtensionShare>((Func<ExtensionShare, bool>) (x => x.Type.Equals("account", StringComparison.OrdinalIgnoreCase))).Select<ExtensionShare, string>((Func<ExtensionShare, string>) (x => x.Id)).ToList<string>();
            azureIndexDocument1.EnterpriseSharedWithIds = extension.SharedWith.Where<ExtensionShare>((Func<ExtensionShare, bool>) (x => x.Type.Equals("organization", StringComparison.OrdinalIgnoreCase))).Select<ExtensionShare, string>((Func<ExtensionShare, string>) (x => x.Id)).ToList<string>();
          }
          if (!extension.Metadata.IsNullOrEmpty<ExtensionMetadata>())
          {
            azureIndexDocument1.Metadata = extension.Metadata.Serialize<List<ExtensionMetadata>>();
            azureIndexDocument1.SearchableMetadata = extension.Metadata.Select<ExtensionMetadata, string>((Func<ExtensionMetadata, string>) (x => (x.Key + ":" + x.Value).ToLower())).ToList<string>();
            azureIndexDocument1.ProjectType = extension.Metadata.FirstOrDefault<ExtensionMetadata>((Func<ExtensionMetadata, bool>) (x => string.Equals(x.Key, "ProjectType", StringComparison.OrdinalIgnoreCase)))?.Value;
          }
          if (!extension.Lcids.IsNullOrEmpty<int>())
            azureIndexDocument1.Lcids = extension.Lcids.Select<int, string>((Func<int, string>) (x => x.ToString((IFormatProvider) CultureInfo.InvariantCulture))).ToList<string>();
          if (extension.IsVsCodeExtension())
          {
            azureIndexDocument1.TargetPlatforms = GalleryServerUtil.GetTargetPlatformsFromExtensionVersions(extensionVersionList1);
            if (!azureIndexDocument1.Tags.IsNullOrEmpty<string>() && azureIndexDocument1.Tags.Contains("__web_extension") && !azureIndexDocument1.TargetPlatforms.Contains("web"))
              azureIndexDocument1.TargetPlatforms.Add("web");
          }
          indexObject.Add(azureIndexDocument1);
        }
      }
      return indexObject;
    }

    private List<string> GetModifiedInstallationTargetList(
      List<InstallationTarget> installationTargets,
      bool useProductArchitectureInfo,
      bool isVsix)
    {
      ISet<string> stringSet = (ISet<string>) new HashSet<string>();
      if (!GalleryInstallationTargets.VsInstallationTargets.Overlaps((IEnumerable<InstallationTarget>) installationTargets))
        return installationTargets.Select<InstallationTarget, string>((Func<InstallationTarget, string>) (x => x.Target)).ToList<string>();
      foreach (InstallationTarget installationTarget in installationTargets)
      {
        if (string.Equals(installationTarget.Target, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase))
          stringSet.Add(installationTarget.Target);
        else if (installationTarget.MinVersion != (Version) null && installationTarget.MaxVersion != (Version) null)
        {
          int major = installationTarget.MinVersion.Major;
          int minor = installationTarget.MinVersion.Minor;
          int num1 = installationTarget.MaxVersion.Major == int.MaxValue ? this.maxSupportedVsIdeMajorVersion : installationTarget.MaxVersion.Major;
          int num2 = installationTarget.MaxVersion.Minor == int.MaxValue ? minor + this.maxSupportedVsIdeMinorVersionIncrement : installationTarget.MaxVersion.Minor;
          int majorVersion = major;
          int num3 = installationTarget.MinInclusive || installationTarget.MaxVersion.Build > 0 ? minor : minor + 1;
          while (majorVersion < num1)
          {
            if (!this.supportedVsIdeMajorVersions.Contains<int>(majorVersion))
            {
              ++majorVersion;
            }
            else
            {
              for (int minorVersion = num3; minorVersion < this.maxSupportedVsIdeMinorVersionIncrement; ++minorVersion)
              {
                if (useProductArchitectureInfo)
                {
                  this.AddToInstallationTargetSet(installationTarget, stringSet, majorVersion, minorVersion, isVsix);
                }
                else
                {
                  string str = installationTarget.Target + "_" + majorVersion.ToString() + "." + minorVersion.ToString();
                  stringSet.Add(str);
                }
              }
              num3 = 0;
              ++majorVersion;
            }
          }
          if (this.supportedVsIdeMajorVersions.Contains<int>(majorVersion))
          {
            int num4 = installationTarget.MaxInclusive || installationTarget.MaxVersion.Build > 0 ? num2 : num2 - 1;
            for (int minorVersion = num3; minorVersion <= num4; ++minorVersion)
            {
              if (useProductArchitectureInfo)
              {
                this.AddToInstallationTargetSet(installationTarget, stringSet, majorVersion, minorVersion, isVsix);
              }
              else
              {
                string str = installationTarget.Target + "_" + majorVersion.ToString() + "." + minorVersion.ToString();
                stringSet.Add(str);
              }
            }
          }
        }
        if (!stringSet.IsNullOrEmpty<string>())
          stringSet.Add(installationTarget.Target);
      }
      return stringSet.ToList<string>();
    }

    private void AddToInstallationTargetSet(
      InstallationTarget target,
      ISet<string> installationTargetSet,
      int majorVersion,
      int minorVersion,
      bool isVsix)
    {
      if (isVsix)
      {
        string str = target.Target + "_" + majorVersion.ToString() + "." + minorVersion.ToString() + "_" + target.ProductArchitecture;
        installationTargetSet.Add(str);
      }
      else
      {
        foreach (string productArchitecture in (IEnumerable<string>) this.supportedProductArchitectures)
        {
          if (majorVersion >= 17 || "x86".Equals(productArchitecture))
          {
            string str = target.Target + "_" + majorVersion.ToString() + "." + minorVersion.ToString() + "_" + productArchitecture;
            installationTargetSet.Add(str);
          }
        }
      }
    }

    public virtual ExtensionQueryResult ConvertSearchResultToExtensionQueryResult(
      object searchResults,
      ExtensionQueryFlags queryFlags,
      ExtensionQueryResultMetadataFlags metadataFlags,
      object searchResultsForCategoryMetadata = null,
      bool useNewIndexDefinition = false,
      object searchResultForTargetPlatformMetadata = null)
    {
      ExtensionQueryResult extensionQueryResult = new ExtensionQueryResult();
      ExtensionFilterResult extensionFilterResult = new ExtensionFilterResult();
      List<PublishedExtension> publishedExtensionList = new List<PublishedExtension>();
      List<ExtensionFilterResultMetadata> filterResultMetadataList1 = (List<ExtensionFilterResultMetadata>) null;
      extensionFilterResult.Extensions = publishedExtensionList;
      extensionFilterResult.ResultMetadata = filterResultMetadataList1;
      extensionQueryResult.Results = new List<ExtensionFilterResult>()
      {
        extensionFilterResult
      };
      DocumentSearchResult<AzureIndexDocument> documentSearchResult1 = searchResults as DocumentSearchResult<AzureIndexDocument>;
      documentSearchResult4 = (DocumentSearchResult<AzureIndexDocument>) null;
      if (documentSearchResult1 == null)
        throw new ExternalSearchException("Could not convert the search results object into DocumentSearchResult<AzureIndexDocument> object.");
      switch (searchResultsForCategoryMetadata)
      {
        case null:
        case DocumentSearchResult<AzureIndexDocument> documentSearchResult4:
          List<SearchResult<AzureIndexDocument>> list = ((DocumentSearchResultBase<SearchResult<AzureIndexDocument>, AzureIndexDocument>) documentSearchResult1).Results.ToList<SearchResult<AzureIndexDocument>>();
          for (int index1 = 0; index1 < list.Count; ++index1)
          {
            AzureIndexDocument document = ((SearchResultBase<AzureIndexDocument>) list[index1]).Document;
            PublishedExtension publishedExtension1 = new PublishedExtension();
            publishedExtension1.ExtensionId = new Guid(document.ExtensionId);
            publishedExtension1.ExtensionName = document.ExtensionName;
            publishedExtension1.Publisher = JsonUtilities.Deserialize<PublisherFacts>(document.Publisher);
            List<ServerExtensionVersion> enumerable = JsonUtilities.Deserialize<List<ServerExtensionVersion>>(document.ValidatedVersions);
            if (!enumerable.IsNullOrEmpty<ServerExtensionVersion>())
            {
              List<ExtensionVersion> extensionVersionList = JsonUtilities.Deserialize<List<ExtensionVersion>>(document.ValidatedVersions);
              for (int index2 = 0; index2 < enumerable.Count; ++index2)
              {
                ServerExtensionVersion extensionVersion1 = enumerable[index2];
                ExtensionVersion extensionVersion2 = extensionVersionList[index2];
                int index3 = 0;
                while (true)
                {
                  int num = index3;
                  int? count = extensionVersion1.Files?.Count;
                  int valueOrDefault = count.GetValueOrDefault();
                  if (num < valueOrDefault & count.HasValue)
                  {
                    extensionVersion2.Files[index3].FileId = extensionVersion1.Files[index3].SerializableFileId;
                    ++index3;
                  }
                  else
                    break;
                }
              }
              publishedExtension1.Versions = extensionVersionList;
              if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeLatestVersionOnly) && publishedExtension1.Versions.Count > 1)
                publishedExtension1.Versions = GalleryUtil.GetLatestExtensionVersionForSupportedTargetPlatforms(publishedExtension1.Versions);
              for (int index4 = 0; index4 < publishedExtension1.Versions.Count; ++index4)
              {
                ExtensionVersion version = publishedExtension1.Versions[index4];
                if (!version.AssetUri.Equals(version.FallbackAssetUri, StringComparison.OrdinalIgnoreCase))
                {
                  version.IsCdnEnabled = true;
                  int num = version.AssetUri.LastIndexOf("/");
                  if (num > 0)
                    version.CdnDirectory = version.AssetUri.Substring(num + 1);
                }
              }
              publishedExtension1.DisplayName = document.ExtensionDisplayName;
              publishedExtension1.ShortDescription = document.ShortDescription;
              if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeCategoryAndTags))
              {
                publishedExtension1.Categories = document.Categories;
                publishedExtension1.Tags = document.Tags;
              }
              string json = string.Join(",", (IEnumerable<string>) document.ExtensionFlags).Serialize<string>();
              publishedExtension1.Flags = JsonUtilities.Deserialize<PublishedExtensionFlags>(json);
              PublishedExtension publishedExtension2 = publishedExtension1;
              DateTime? nullable = document.LastUpdated;
              DateTime dateTime1;
              if (!nullable.HasValue)
              {
                dateTime1 = new DateTime();
              }
              else
              {
                nullable = document.LastUpdated;
                dateTime1 = nullable.Value;
              }
              publishedExtension2.LastUpdated = dateTime1;
              PublishedExtension publishedExtension3 = publishedExtension1;
              nullable = document.PublishedDate;
              DateTime dateTime2;
              if (!nullable.HasValue)
              {
                dateTime2 = new DateTime();
              }
              else
              {
                nullable = document.PublishedDate;
                dateTime2 = nullable.Value;
              }
              publishedExtension3.PublishedDate = dateTime2;
              PublishedExtension publishedExtension4 = publishedExtension1;
              nullable = document.ReleasedDate;
              DateTime dateTime3;
              if (!nullable.HasValue)
              {
                dateTime3 = new DateTime();
              }
              else
              {
                nullable = document.ReleasedDate;
                dateTime3 = nullable.Value;
              }
              publishedExtension4.ReleaseDate = dateTime3;
              if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeStatistics) && document.Statistics != null)
                publishedExtension1.Statistics = JsonUtilities.Deserialize<List<ExtensionStatistic>>(document.Statistics);
              if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeInstallationTargets))
                publishedExtension1.InstallationTargets = JsonUtilities.Deserialize<List<InstallationTarget>>(document.InstallationTargets);
              if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeMetadata) && document.Metadata != null)
                publishedExtension1.Metadata = JsonUtilities.Deserialize<List<ExtensionMetadata>>(document.Metadata);
              if (queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeLcids) && document.Lcids != null)
                publishedExtension1.Lcids = document.Lcids.Select<string, int>((Func<string, int>) (x => Convert.ToInt32(x))).ToList<int>();
              ExtensionDeploymentTechnology result = ExtensionDeploymentTechnology.Exe;
              publishedExtension1.DeploymentType = Enum.TryParse<ExtensionDeploymentTechnology>(document.DeploymentType, out result) ? result : (ExtensionDeploymentTechnology) 0;
              if ((queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedAccounts) || queryFlags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedOrganizations)) && !document.SharedWith.IsNullOrEmpty<char>())
                publishedExtension1.SharedWith = JsonUtilities.Deserialize<List<ExtensionShare>>(document.SharedWith);
              publishedExtensionList.Add(publishedExtension1);
            }
            else
              TeamFoundationTracingService.TraceRaw(12060109, TraceLevel.Error, "gallery", "SearchConverter", "Extension " + publishedExtension1.Publisher.PublisherName + "." + publishedExtension1.ExtensionName + " does not contain any validated version. Skipping adding it to the search results.");
          }
          List<ExtensionFilterResultMetadata> filterResultMetadataList2 = new List<ExtensionFilterResultMetadata>();
          if (metadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultCount))
          {
            ExtensionFilterResultMetadata filterResultMetadata = new ExtensionFilterResultMetadata()
            {
              MetadataType = QueryMetadataConstants.ResultCount,
              MetadataItems = new List<MetadataItem>()
              {
                new MetadataItem()
                {
                  Name = QueryMetadataConstants.TotalCount,
                  Count = (int) ((DocumentSearchResultBase<SearchResult<AzureIndexDocument>, AzureIndexDocument>) documentSearchResult1).Count.Value
                }
              }
            };
            filterResultMetadataList2.Add(filterResultMetadata);
          }
          DocumentSearchResult<AzureIndexDocument> documentSearchResult2 = (DocumentSearchResult<AzureIndexDocument>) null;
          if (documentSearchResult4 != null && metadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludePreCategoryFilterCategories))
            documentSearchResult2 = documentSearchResult4;
          else if (metadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludePreCategoryFilterCategories) || metadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultSetCategories))
            documentSearchResult2 = documentSearchResult1;
          if (documentSearchResult2 != null)
          {
            ExtensionFilterResultMetadata filterResultMetadata = new ExtensionFilterResultMetadata()
            {
              MetadataType = metadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultSetCategories) ? QueryMetadataConstants.ResultSetCategories : QueryMetadataConstants.Categories,
              MetadataItems = ((Dictionary<string, IList<FacetResult>>) ((DocumentSearchResultBase<SearchResult<AzureIndexDocument>, AzureIndexDocument>) documentSearchResult2).Facets)["Categories"].Select<FacetResult, MetadataItem>((Func<FacetResult, MetadataItem>) (x => new MetadataItem()
              {
                Name = x.Value.ToString(),
                Count = x.Count.HasValue ? (int) x.Count.Value : 0
              })).ToList<MetadataItem>()
            };
            filterResultMetadataList2.Add(filterResultMetadata);
          }
          if (metadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeResultSetProjectType))
          {
            ExtensionFilterResultMetadata filterResultMetadata = new ExtensionFilterResultMetadata()
            {
              MetadataType = QueryMetadataConstants.ResultSetProjectTypes,
              MetadataItems = ((Dictionary<string, IList<FacetResult>>) ((DocumentSearchResultBase<SearchResult<AzureIndexDocument>, AzureIndexDocument>) documentSearchResult1).Facets)["ProjectType"].Select<FacetResult, MetadataItem>((Func<FacetResult, MetadataItem>) (x => new MetadataItem()
              {
                Name = x.Value.ToString(),
                Count = x.Count.HasValue ? (int) x.Count.Value : 0
              })).ToList<MetadataItem>()
            };
            filterResultMetadataList2.Add(filterResultMetadata);
          }
          if (metadataFlags.HasFlag((Enum) ExtensionQueryResultMetadataFlags.IncludeTargetPlatforms))
          {
            if (searchResultForTargetPlatformMetadata != null)
            {
              if (!(searchResultForTargetPlatformMetadata is DocumentSearchResult<AzureIndexDocument> documentSearchResult3))
                throw new ExternalSearchException("Could not convert the search results object for retrieving target platforms info into DocumentSearchResult<AzureIndexDocument> object.");
            }
            else
              documentSearchResult3 = documentSearchResult1;
            ExtensionFilterResultMetadata filterResultMetadata = new ExtensionFilterResultMetadata()
            {
              MetadataType = QueryMetadataConstants.TargetPlatforms,
              MetadataItems = ((Dictionary<string, IList<FacetResult>>) ((DocumentSearchResultBase<SearchResult<AzureIndexDocument>, AzureIndexDocument>) documentSearchResult3).Facets)["TargetPlatforms"].Select<FacetResult, MetadataItem>((Func<FacetResult, MetadataItem>) (x => new MetadataItem()
              {
                Name = x.Value.ToString(),
                Count = x.Count.HasValue ? (int) x.Count.Value : 0
              })).ToList<MetadataItem>()
            };
            filterResultMetadataList2.Add(filterResultMetadata);
          }
          extensionQueryResult.Results[0].ResultMetadata = filterResultMetadataList2.Count > 0 ? filterResultMetadataList2 : (List<ExtensionFilterResultMetadata>) null;
          return extensionQueryResult;
        default:
          throw new ExternalSearchException("Could not convert the search results object for retrieving category info into DocumentSearchResult<AzureIndexDocument> object.");
      }
    }
  }
}
