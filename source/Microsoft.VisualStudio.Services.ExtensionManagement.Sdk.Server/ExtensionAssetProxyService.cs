// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionAssetProxyService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ExtensionAssetProxyService : IVssFrameworkService
  {
    private bool m_cacheEnabled;
    private const string c_area = "ExtensionAssetProxyService";
    private const string c_layer = "Extensions";
    private const string RegistryRoot = "/Configuration/ExtensionAssets/";
    private const string FileCacheEnabled = "/Configuration/ExtensionAssets/FileCacheEnabled";
    private static readonly RegistryQuery s_fileCacheEnabledQuery = new RegistryQuery("/Configuration/ExtensionAssets/FileCacheEnabled");

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/ExtensionAssets/...");
      this.LoadConfiguration(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<CachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void LoadConfiguration(IVssRequestContext requestContext)
    {
      this.m_cacheEnabled = requestContext.GetService<CachedRegistryService>().GetValue<bool>(requestContext, in ExtensionAssetProxyService.s_fileCacheEnabledQuery, true);
      requestContext.Trace(10013604, TraceLevel.Info, nameof (ExtensionAssetProxyService), "Extensions", "LoadConfiguration:: Extension asset cache enabled: {0}", (object) this.m_cacheEnabled);
    }

    private bool IsCacheEnabled(IVssRequestContext requestContext)
    {
      requestContext.Trace(10013605, TraceLevel.Info, nameof (ExtensionAssetProxyService), "Extensions", "Extension asset cache enabled: {0}", (object) this.m_cacheEnabled);
      return this.m_cacheEnabled;
    }

    public Stream QueryAsset(
      IVssRequestContext requestContext,
      string providerName,
      string version,
      string assetType,
      out string contentType,
      out CompressionType compressionType)
    {
      contentType = (string) null;
      compressionType = CompressionType.None;
      ExtensionAssetDetails extensionAssetDetails = this.QueryAssetDetails(requestContext, providerName, version, assetType);
      contentType = extensionAssetDetails.ContentType;
      compressionType = CompressionType.None;
      if (this.IsCacheEnabled(requestContext))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationFileCacheService service = vssRequestContext.GetService<TeamFoundationFileCacheService>();
        using (ExtensionAssetDownloadState assetDownloadState = new ExtensionAssetDownloadState(vssRequestContext))
        {
          try
          {
            FileInformation fileInfo = new FileInformation(vssRequestContext.ServiceHost.InstanceId, extensionAssetDetails.FileId, (byte[]) null);
            service.RetrieveFile<FileInformation>(vssRequestContext, fileInfo, (IDownloadState<FileInformation>) assetDownloadState, false);
            return (Stream) assetDownloadState.FileStream;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10013623, nameof (ExtensionAssetProxyService), "Extensions", ex);
            requestContext.Trace(10013624, TraceLevel.Error, nameof (ExtensionAssetProxyService), "Extensions", "Failed to load extension file cache service.  Falling back to source: {0} {1} {2}", (object) providerName, (object) version, (object) assetType);
            if (assetDownloadState.FileStream != null)
              assetDownloadState.Dispose();
          }
        }
      }
      else
        requestContext.Trace(10013625, TraceLevel.Info, nameof (ExtensionAssetProxyService), "Extensions", "Cache disabled: Loading extension from source: {0} {1} {2}", (object) providerName, (object) version, (object) assetType);
      return this.QueryAssetResult(requestContext, providerName, version, assetType);
    }

    private Stream QueryAssetResult(
      IVssRequestContext requestContext,
      string providerName,
      string version,
      string assetType)
    {
      requestContext.Trace(10013645, TraceLevel.Info, nameof (ExtensionAssetProxyService), "Extensions", "Loading asset from source: {0} {1} {2}", (object) providerName, (object) version, (object) assetType);
      string contentType = (string) null;
      CompressionType compressionType = CompressionType.None;
      return this.QueryAssetProvider(requestContext, providerName, version).QueryAsset(requestContext, assetType, out contentType, out compressionType).GetAwaiter().GetResult();
    }

    private ExtensionAssetDetails QueryAssetDetails(
      IVssRequestContext requestContext,
      string providerName,
      string version,
      string assetType)
    {
      requestContext.Trace(10013650, TraceLevel.Info, nameof (ExtensionAssetProxyService), "Extensions", "Loading asset details: {0} {1} {2}", (object) providerName, (object) version, (object) assetType);
      return this.QueryAssetProvider(requestContext, providerName, version).QueryAssetDetails(requestContext, assetType);
    }

    private IAssetProvider QueryAssetProvider(
      IVssRequestContext requestContext,
      string providerName,
      string version)
    {
      requestContext.Trace(10013655, TraceLevel.Info, nameof (ExtensionAssetProxyService), "Extensions", "Loading asset provider {0} for version {1}.", (object) providerName, (object) version);
      IContributionProvider contributionProvider = requestContext.GetExtension<IFileServiceContributionSource>(ExtensionLifetime.Service).QueryProvider(requestContext, providerName);
      ContributionProviderDetails contributionProviderDetails = contributionProvider.QueryProviderDetails(requestContext);
      if (string.IsNullOrEmpty(contributionProviderDetails.Version) || !contributionProviderDetails.Version.Equals(version, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Trace(10013660, TraceLevel.Error, nameof (ExtensionAssetProxyService), "Extensions", "Unable to find provider {0} for version {1}.", (object) providerName, (object) version);
        throw new ProviderNotFoundException(ExtMgmtResources.AssetVersionNotFound((object) version, (object) contributionProviderDetails.Version));
      }
      return contributionProvider is IAssetProvider assetProvider ? assetProvider : throw new AssetProviderNotFoundException(ExtMgmtResources.AssetProviderNotFound((object) contributionProvider, (object) version));
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.Trace(10013665, TraceLevel.Info, nameof (ExtensionAssetProxyService), "Extensions", "Extension asset settings have changed.");
      this.LoadConfiguration(requestContext);
    }
  }
}
