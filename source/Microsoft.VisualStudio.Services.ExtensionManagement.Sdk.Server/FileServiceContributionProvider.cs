// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FileServiceContributionProvider
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class FileServiceContributionProvider : IContributionProvider, IAssetProvider
  {
    private static readonly string s_area = nameof (FileServiceContributionProvider);
    private static readonly string s_layer = "IVssFrameworkService";
    private ContributionProviderDetails m_details;
    private string m_providerName;
    private string m_publisherDisplayName;
    private ContributionData m_contributionData;
    private LocalContributionDetails m_contributionDetails;

    public FileServiceContributionProvider(LocalContributionDetails contributionDetails)
    {
      this.m_publisherDisplayName = string.IsNullOrEmpty(contributionDetails.PublisherDisplayName) ? contributionDetails.PublisherName : contributionDetails.PublisherDisplayName;
      this.m_providerName = GalleryUtil.CreateFullyQualifiedName(contributionDetails.PublisherName, contributionDetails.ExtensionName);
      this.m_contributionDetails = contributionDetails;
      this.m_contributionData = new ContributionData()
      {
        Contributions = this.m_contributionDetails.Contributions,
        ContributionTypes = this.m_contributionDetails.ContributionTypes,
        Constraints = this.m_contributionDetails.Constraints
      };
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
      long contentLength = 0;
      byte[] hashValue = new byte[16];
      ExtensionAssetDetails extensionAssetDetails = this.QueryAssetDetails(requestContext, assetType);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>();
      ITeamFoundationFileService service = vssRequestContext.GetService<ITeamFoundationFileService>();
      requestContext.Trace(100136281, TraceLevel.Info, FileServiceContributionProvider.s_area, FileServiceContributionProvider.s_layer, "Querying for asset from provider: {0}  for asset: {1}", (object) this.m_providerName, (object) assetType);
      if (extensionAssetDetails.FileId > 0)
      {
        contentType = extensionAssetDetails.ContentType;
        return Task.FromResult<Stream>(service.RetrieveFile(vssRequestContext, (long) extensionAssetDetails.FileId, false, out hashValue, out contentLength, out compressionType));
      }
      requestContext.Trace(100136282, TraceLevel.Info, FileServiceContributionProvider.s_area, FileServiceContributionProvider.s_layer, "Asset not found from provider: {0}  for asset: {1}", (object) this.m_providerName, (object) assetType);
      throw new ContributionAssetNotFoundException(ExtMgmtResources.AssetNotFound((object) this.m_providerName, (object) assetType));
    }

    public ExtensionAssetDetails QueryAssetDetails(
      IVssRequestContext requestContext,
      string assetType)
    {
      ExtensionAssetDetails extensionAssetDetails = new ExtensionAssetDetails();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      List<CultureInfo> acceptedCultures = RequestLanguage.GetAcceptedCultures(requestContext);
      int val1 = 0;
      int val2 = 0;
      int num = int.MaxValue;
      string registryPathPattern1 = string.Format("/Configuration/LocalContributions/{0}/{1}/Assets/**/FileId", (object) this.m_contributionDetails.PublisherName, (object) this.m_contributionDetails.ExtensionName);
      string registryPathPattern2 = string.Format("/Configuration/LocalContributions/{0}/{1}/Assets/_/{2}/ContentType", (object) this.m_contributionDetails.PublisherName, (object) this.m_contributionDetails.ExtensionName, (object) assetType);
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(vssRequestContext, new RegistryQuery(registryPathPattern1));
      string str = (string) null;
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        string[] pathParts = registryEntry.Path.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (!LocalExtensionUtil.AssetTypeFromPath(pathParts).Equals(assetType, StringComparison.OrdinalIgnoreCase))
        {
          str = (string) null;
        }
        else
        {
          int result = 0;
          str = pathParts[5];
          if (int.TryParse(registryEntry.Value, out result))
          {
            if (LocalExtensionRegistry.IsDefaultLanguage(str))
            {
              val1 = result;
            }
            else
            {
              foreach (CultureInfo cultureA in acceptedCultures)
              {
                int matchingDistance = GalleryUtil.GetCultureMatchingDistance(cultureA, CultureInfo.GetCultureInfo(str));
                if (matchingDistance >= 0 && matchingDistance < num)
                {
                  val2 = result;
                  num = matchingDistance;
                  if (matchingDistance == 0)
                    goto label_15;
                }
              }
            }
          }
        }
      }
label_15:
      extensionAssetDetails.FileId = Math.Max(val1, val2);
      extensionAssetDetails.ContentType = service.GetValue(vssRequestContext, new RegistryQuery(registryPathPattern2), (string) null);
      extensionAssetDetails.Language = str;
      return extensionAssetDetails;
    }

    public Dictionary<string, string> QueryAssetLocations(
      IVssRequestContext requestContext,
      string assetType)
    {
      requestContext.Trace(100136273, TraceLevel.Info, FileServiceContributionProvider.s_area, FileServiceContributionProvider.s_layer, "Querying for asset locations from provider: {0}  for asset: {1}", (object) this.m_providerName, (object) assetType);
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string relativePath = string.Format("{0}/{1}/{2}/{3}/_/{4}", (object) "ext", (object) this.m_contributionDetails.PublisherName, (object) this.m_contributionDetails.ExtensionName, (object) this.m_contributionDetails.Version, (object) assetType);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        ICdnLocationService service = vssRequestContext.GetService<ICdnLocationService>();
        if (!string.IsNullOrEmpty(service.GetCdnUrl(vssRequestContext, relativePath)))
        {
          string cdnUrl = service.GetCdnUrl(vssRequestContext, relativePath);
          requestContext.Trace(100136274, TraceLevel.Info, FileServiceContributionProvider.s_area, FileServiceContributionProvider.s_layer, "CDN asset location from provider: {0}  for asset: {1} location: {2}", (object) this.m_providerName, (object) assetType, (object) cdnUrl);
          dictionary1.Add("Cdn", cdnUrl);
        }
      }
      ILocationService service1 = requestContext.GetService<ILocationService>();
      Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
      dictionary2["providerName"] = (object) string.Format("{0}.{1}", (object) this.m_contributionDetails.PublisherName, (object) this.m_contributionDetails.ExtensionName);
      dictionary2["version"] = (object) this.m_contributionDetails.Version;
      dictionary2[nameof (assetType)] = (object) assetType;
      IVssRequestContext requestContext1 = requestContext;
      Guid assetsLocationId = ContributionsResourceIds.LocalExtensionAssetsLocationId;
      Dictionary<string, object> routeValues = dictionary2;
      string str = new UriBuilder(service1.GetResourceUri(requestContext1, "Extensions", assetsLocationId, (object) routeValues)).Uri.ToString();
      dictionary1.Add("Local", str);
      requestContext.Trace(100136275, TraceLevel.Info, FileServiceContributionProvider.s_area, FileServiceContributionProvider.s_layer, "Local asset location from provider: {0}  for asset: {1} location: {2}", (object) this.m_providerName, (object) assetType, (object) str);
      return dictionary1;
    }
  }
}
