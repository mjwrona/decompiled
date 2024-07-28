// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FileSystemContributionProvider
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class FileSystemContributionProvider : IContributionProvider, IAssetProvider
  {
    private static readonly string s_area = nameof (FileSystemContributionProvider);
    private static readonly string s_layer = "IVssFrameworkService";
    private static string[] MultiPartFileExtensions = new string[3]
    {
      ".min.js",
      ".min.css",
      ".js.map"
    };
    private Dictionary<string, List<FileExtensionAssetDetails>> m_assetMapping = new Dictionary<string, List<FileExtensionAssetDetails>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private ContributionProviderDetails m_details;
    private string m_providerName;
    private string m_publisherDisplayName;
    private ContributionData m_contributionData;
    private bool m_updatedContributionsFromDefault;
    private LocalContributionDetails m_contributionDetails;
    private string m_staticContentVersion;

    public FileSystemContributionProvider(
      LocalContributionDetails contributionDetails,
      bool includeDebugAssets)
    {
      this.m_staticContentVersion = StaticResources.Versioned.Version;
      this.m_publisherDisplayName = string.IsNullOrEmpty(contributionDetails.PublisherDisplayName) ? contributionDetails.PublisherName : contributionDetails.PublisherDisplayName;
      this.m_providerName = GalleryUtil.CreateFullyQualifiedName(contributionDetails.PublisherName, contributionDetails.ExtensionName);
      this.m_contributionDetails = contributionDetails;
      this.m_contributionData = new ContributionData()
      {
        Contributions = this.m_contributionDetails.Contributions,
        ContributionTypes = this.m_contributionDetails.ContributionTypes,
        Constraints = this.m_contributionDetails.Constraints
      };
      this.ProcessContributions(includeDebugAssets);
      this.m_details = new ContributionProviderDetails();
      this.m_details.Name = this.m_providerName;
      this.m_details.Version = this.m_contributionDetails.Version;
      this.m_details.DisplayName = this.m_publisherDisplayName;
      this.m_details.Properties.Add("::Version", this.m_contributionDetails.Version);
    }

    public IEnumerable<Contribution> Contributions => this.m_contributionData.Contributions;

    public IEnumerable<ContributionType> ContributionTypes => this.m_contributionData.ContributionTypes;

    public IEnumerable<ContributionConstraint> Constraints => this.m_contributionData.Constraints;

    public string ProviderName => this.m_providerName;

    public string ProviderDisplayName => this.m_publisherDisplayName;

    public ContributionData QueryContributionData(IVssRequestContext requestContext) => this.m_contributionData;

    public ContributionProviderDetails QueryProviderDetails(IVssRequestContext requestContext) => this.m_details;

    public Task<Stream> QueryAsset(
      IVssRequestContext requestContext,
      string assetType,
      out string contentType,
      out CompressionType compressionType)
    {
      compressionType = CompressionType.None;
      contentType = (string) null;
      FileExtensionAssetDetails bestAssetMatch = this.GetBestAssetMatch(requestContext, assetType);
      if (bestAssetMatch != null)
      {
        assetType = bestAssetMatch.Location;
        bool versioned = bestAssetMatch.Versioned;
        string fileContentLocation = this.GetFileContentLocation(requestContext, Path.Combine(this.m_providerName, assetType), versioned);
        if (!string.IsNullOrEmpty(bestAssetMatch.Lang))
          Path.Combine("L10N", bestAssetMatch.Lang);
        string fullPath = FileSpec.GetFullPath(fileContentLocation);
        if (File.Exists(fullPath))
        {
          MemoryStream memoryStream = new MemoryStream();
          using (Stream stream = (Stream) File.OpenRead(fullPath))
            stream.CopyTo((Stream) memoryStream);
          memoryStream.Seek(0L, SeekOrigin.Begin);
          return Task.FromResult<Stream>((Stream) memoryStream);
        }
      }
      requestContext.Trace(100136282, TraceLevel.Info, FileSystemContributionProvider.s_area, FileSystemContributionProvider.s_layer, "Asset not found from provider: {0} for asset: {1}", (object) this.m_providerName, (object) assetType);
      throw new ContributionAssetNotFoundException(ExtMgmtResources.AssetNotFound((object) this.m_providerName, (object) assetType));
    }

    public ExtensionAssetDetails QueryAssetDetails(
      IVssRequestContext requestContext,
      string assetType)
    {
      return (ExtensionAssetDetails) null;
    }

    public Dictionary<string, string> QueryAssetLocations(
      IVssRequestContext requestContext,
      string assetType)
    {
      requestContext.Trace(100136273, TraceLevel.Info, FileSystemContributionProvider.s_area, FileSystemContributionProvider.s_layer, "Querying for asset locations from provider: {0} for asset: {1}", (object) this.m_providerName, (object) assetType);
      bool isVersionedContent = true;
      FileExtensionAssetDetails bestAssetMatch = this.GetBestAssetMatch(requestContext, assetType);
      if (bestAssetMatch != null)
      {
        assetType = bestAssetMatch.Location;
        isVersionedContent = bestAssetMatch.Versioned;
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string relativePath = !isVersionedContent ? string.Format("{0}/{1}/{2}", (object) "ext", (object) this.m_providerName, (object) assetType) : string.Format("v/{0}/_ext/{1}/{2}", (object) this.m_staticContentVersion, (object) this.m_providerName, (object) assetType);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string cdnUrl = vssRequestContext.GetService<ICdnLocationService>().GetCdnUrl(vssRequestContext, relativePath);
        if (!string.IsNullOrEmpty(cdnUrl))
        {
          requestContext.Trace(100136274, TraceLevel.Info, FileSystemContributionProvider.s_area, FileSystemContributionProvider.s_layer, "CDN asset location from provider: {0}  for asset: {1} location: {2}", (object) this.m_providerName, (object) assetType, (object) cdnUrl);
          dictionary.Add("Cdn", cdnUrl);
        }
      }
      string localContentLocation = this.GetLocalContentLocation(requestContext, this.m_providerName + "/" + assetType, isVersionedContent);
      requestContext.Trace(100136275, TraceLevel.Info, FileSystemContributionProvider.s_area, FileSystemContributionProvider.s_layer, "Local asset location from provider: {0} for asset: {1} location: {2}", (object) this.m_providerName, (object) assetType, (object) localContentLocation);
      dictionary.Add("Local", localContentLocation);
      return dictionary;
    }

    internal void UpdateContributions(
      IEnumerable<Contribution> contributions,
      bool includeDebugAssets)
    {
      this.m_contributionData.Contributions = this.m_contributionData.Contributions == null ? contributions : this.m_contributionData.Contributions.Concat<Contribution>(contributions);
      this.ProcessContributions(includeDebugAssets);
    }

    internal void ReplaceContributions(
      IEnumerable<Contribution> contributions,
      bool includeDebugAssets)
    {
      this.m_contributionData.Contributions = contributions;
      this.ProcessContributions(includeDebugAssets);
    }

    internal void UpdateContributionsFromDefault(IContributionProvider defaultContributionProvider)
    {
      if (this.m_updatedContributionsFromDefault)
        return;
      using (IEnumerator<Contribution> enumerator = this.Contributions.GetEnumerator())
      {
        enumerator.MoveNext();
        foreach (Contribution contribution in defaultContributionProvider.Contributions)
        {
          if (contribution.Id == enumerator.Current.Id)
          {
            enumerator.Current.RestrictedTo = contribution.RestrictedTo;
            if (!enumerator.MoveNext())
              break;
          }
        }
      }
      this.m_updatedContributionsFromDefault = true;
    }

    private FileExtensionAssetDetails GetBestAssetMatch(
      IVssRequestContext requestContext,
      string assetType)
    {
      FileExtensionAssetDetails bestAssetMatch = (FileExtensionAssetDetails) null;
      FileExtensionAssetDetails extensionAssetDetails1 = (FileExtensionAssetDetails) null;
      int num = int.MaxValue;
      List<CultureInfo> acceptedCultures = RequestLanguage.GetAcceptedCultures(requestContext);
      List<FileExtensionAssetDetails> extensionAssetDetailsList;
      if (this.m_assetMapping.TryGetValue(assetType, out extensionAssetDetailsList))
      {
        foreach (CultureInfo cultureA in acceptedCultures)
        {
          foreach (FileExtensionAssetDetails extensionAssetDetails2 in extensionAssetDetailsList)
          {
            if (string.IsNullOrEmpty(extensionAssetDetails2.Lang))
            {
              extensionAssetDetails1 = extensionAssetDetails2;
            }
            else
            {
              int relationshipDistance = CultureResolution.GetCultureRelationshipDistance(cultureA, CultureInfo.GetCultureInfo(extensionAssetDetails2.Lang));
              if (relationshipDistance >= 0 && relationshipDistance < num)
              {
                bestAssetMatch = extensionAssetDetails2;
                num = relationshipDistance;
                if (relationshipDistance == 0)
                  return bestAssetMatch;
              }
            }
          }
        }
      }
      return bestAssetMatch ?? extensionAssetDetails1;
    }

    private string GetLocalContentLocation(
      IVssRequestContext requestContext,
      string relativePath,
      bool isVersionedContent)
    {
      string localContentLocation = !isVersionedContent ? StaticResources.Extensions.GetLocalLocation(relativePath, requestContext) : StaticResources.Versioned.Extensions.GetLocalLocation(relativePath, requestContext);
      if (!Uri.IsWellFormedUriString(localContentLocation, UriKind.Absolute))
      {
        string relativePath1 = VirtualPathUtility.ToAppRelative(localContentLocation, requestContext.WebApplicationPath()).TrimStart('~');
        ILocationService service = requestContext.GetService<ILocationService>();
        AccessMapping accessMapping = service.DetermineAccessMapping(requestContext);
        localContentLocation = service.LocationForAccessMapping(requestContext, relativePath1, RelativeToSetting.WebApplication, accessMapping);
      }
      return localContentLocation;
    }

    private string GetFileContentLocation(
      IVssRequestContext requestContext,
      string relativePath,
      bool isVersionedContent)
    {
      return isVersionedContent ? StaticResources.Versioned.Extensions.GetPhysicalLocation(relativePath, requestContext) : StaticResources.Extensions.GetPhysicalLocation(relativePath, requestContext);
    }

    private void ProcessContributions(bool includeDebugAssets)
    {
      Dictionary<string, List<FileExtensionAssetDetails>> assetMapping = new Dictionary<string, List<FileExtensionAssetDetails>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.m_contributionData.Contributions != null)
      {
        foreach (Contribution contribution in this.m_contributionData.Contributions)
        {
          if (!string.IsNullOrEmpty(contribution.Type) && contribution.Type.Equals("ms.vss-web.content"))
          {
            List<FileExtensionAssetDetails> property = contribution.GetProperty<List<FileExtensionAssetDetails>>("content");
            if (property != null)
            {
              for (int index1 = property.Count - 1; index1 >= 0; --index1)
              {
                FileExtensionAssetDetails detail = property[index1];
                string extension = Path.GetExtension(detail.Asset);
                string str1 = detail.HashCode;
                string str2 = detail.Type ?? string.Empty;
                if (!detail.Versioned && !string.IsNullOrEmpty(str1) && (str2.IndexOf("bundle", StringComparison.OrdinalIgnoreCase) != -1 || !string.Equals(extension, ".js") && !string.Equals(extension, ".map")))
                {
                  string fileName = Path.GetFileName(detail.Asset);
                  string str3 = string.Empty;
                  int length = detail.Asset.LastIndexOf("/");
                  if (length > 0)
                    str3 = detail.Asset.Substring(0, length);
                  int num = 0;
                  if (str1.Length > 6)
                    str1 = str1.Substring(0, 6);
                  for (int index2 = 0; index2 < FileSystemContributionProvider.MultiPartFileExtensions.Length; ++index2)
                  {
                    if (fileName.EndsWith(FileSystemContributionProvider.MultiPartFileExtensions[index2], StringComparison.OrdinalIgnoreCase))
                    {
                      num = FileSystemContributionProvider.MultiPartFileExtensions[index2].Length;
                      break;
                    }
                  }
                  if (num == 0)
                    num = Path.GetExtension(fileName).Length;
                  string str4 = fileName.Insert(fileName.Length - num, "." + str1.Replace("+", "_").Replace("/", "_").Replace("a", "c").Replace("A", "C"));
                  string path = string.Format("{0}/{1}", (object) str3.TrimEnd('/'), (object) str4.TrimStart('/'));
                  detail.Location = string.IsNullOrEmpty(detail.Lang) ? path : Path.Combine(Path.GetDirectoryName(path), "L10N", detail.Lang, Path.GetFileName(path)).Replace("\\", "/");
                }
                else
                  detail.Location = detail.Asset;
                if (!includeDebugAssets)
                {
                  if (!detail.Versioned)
                  {
                    bool flag = detail.Asset.EndsWith(".min.js") || detail.Asset.EndsWith(".js") && detail.Asset.Contains("/min/");
                    if (str2.IndexOf("bundle", StringComparison.OrdinalIgnoreCase) != -1 & flag || str2.IndexOf("bundle", StringComparison.OrdinalIgnoreCase) == -1 && !string.Equals(extension, ".js") && !string.Equals(extension, ".map"))
                      this.AddAsset(assetMapping, detail);
                    else
                      property.RemoveAt(index1);
                  }
                  else if (!str2.Equals("stylesheet.bundle") && !str2.Equals("stylesheet"))
                    this.AddAsset(assetMapping, detail);
                  else
                    property.RemoveAt(index1);
                }
                else
                  this.AddAsset(assetMapping, detail);
              }
              if (!includeDebugAssets)
                contribution.ReplaceProperty("content", (object) property);
            }
          }
        }
      }
      this.m_assetMapping = assetMapping;
    }

    private void AddAsset(
      Dictionary<string, List<FileExtensionAssetDetails>> assetMapping,
      FileExtensionAssetDetails detail)
    {
      if (!assetMapping.ContainsKey(detail.Asset))
        assetMapping.Add(detail.Asset, new List<FileExtensionAssetDetails>());
      assetMapping[detail.Asset].Add(detail);
    }
  }
}
