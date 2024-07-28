// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherAssetService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class PublisherAssetService : IPublisherAssetService, IVssFrameworkService
  {
    private const string RequestTokenCacheKey = "Gallery.AssetTokens";
    private readonly Dictionary<string, PublisherAssetService.DefaultAsset> m_defaultAssets = new Dictionary<string, PublisherAssetService.DefaultAsset>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private PublisherAssetConfiguration m_configuration;
    private Microsoft.VisualStudio.Services.Identity.Identity m_servicePrincipalsGroup;
    internal string m_scheme = Uri.UriSchemeHttps;
    private int m_tokenExpirationSeconds = 604800;
    private const string s_layer = "ApiController";
    private readonly HashSet<string> m_AssetsToPublishForBuiltIn = new HashSet<string>()
    {
      "Microsoft.VisualStudio.Services.VSIXPackage",
      "Microsoft.VisualStudio.Services.Manifest",
      "Microsoft.VisualStudio.Services.Icons.Default",
      "Microsoft.VisualStudio.Services.Icons.Small"
    };

    internal PublisherAssetConfiguration Configuration
    {
      set => this.m_configuration = value;
    }

    internal Microsoft.VisualStudio.Services.Identity.Identity ServicePrincipalsGroup
    {
      set => this.m_servicePrincipalsGroup = value;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      service.ReadEntries(systemRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherAsset/*");
      this.LoadConfiguration(systemRequestContext);
      this.LoadDefaultAssets(systemRequestContext);
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PublisherAsset/*");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));

    public PublisherAssetConfiguration GetConfiguration(IVssRequestContext requestContext) => this.m_configuration;

    public void GetPublisherFromToken(
      IVssRequestContext requestContext,
      string assetToken,
      out string publisherName,
      out string extensionName)
    {
      JwtClaims jwtToken = requestContext.GetService<IGalleryJwtTokenService>().ParseJwtToken(requestContext, "AssetSigningKey", assetToken);
      publisherName = (string) null;
      extensionName = (string) null;
      if (jwtToken == null)
        return;
      jwtToken.ExtraClaims.TryGetValue("pn", out publisherName);
      jwtToken.ExtraClaims.TryGetValue("en", out extensionName);
    }

    public string GetTokenForPublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      JwtClaims jwtClaims = new JwtClaims()
      {
        Expiration = new DateTime?(DateTime.UtcNow.AddSeconds((double) this.m_tokenExpirationSeconds)),
        ExtraClaims = new Dictionary<string, string>()
        {
          {
            "pn",
            publisherName
          },
          {
            "en",
            extensionName
          }
        }
      };
      return requestContext.GetService<IGalleryJwtTokenService>().GenerateJwtToken(requestContext, "AssetSigningKey", jwtClaims);
    }

    public bool DirectPackageRequestSupported(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      return !this.IsCDNDownloadEnabled(requestContext, extension);
    }

    public bool DirectAssetRequestSupported(
      IVssRequestContext requestContext,
      ExtensionAsset extensionAsset,
      bool redirect = false)
    {
      bool flag = false;
      string str = "";
      HttpContext current = HttpContext.Current;
      if (current != null)
        str = current.Items[(object) "PublisherAsset"] as string;
      if (redirect && this.IsCDNDownloadEnabled(requestContext, extensionAsset.Extension))
        flag = false;
      else if (extensionAsset.AssetFile != null && extensionAsset.AssetFile.IsDefault || current == null || string.IsNullOrEmpty(this.m_configuration.Host))
        flag = true;
      else if (str != null && str.Equals(extensionAsset.Extension.Publisher.PublisherName, StringComparison.OrdinalIgnoreCase))
      {
        flag = true;
      }
      else
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
        requestContext.GetService<IdentityService>();
        if (userIdentity != null)
          flag = this.IsServicePrincipal(requestContext, userIdentity.Descriptor);
        if (!flag && authenticatedDescriptor != (IdentityDescriptor) null)
          flag = this.IsServicePrincipal(requestContext, authenticatedDescriptor);
      }
      return flag;
    }

    internal virtual bool IsServicePrincipal(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      return ServicePrincipals.IsServicePrincipal(requestContext, descriptor);
    }

    public PackageDetails UploadAssets(
      IVssRequestContext requestContext,
      Stream extensionPackageStream,
      PackageDetails packageDetails,
      HashSet<string> IncludeAssetTypes = null,
      HashSet<string> ExcludeAssetTypes = null)
    {
      requestContext.TraceEnter(12061055, "Gallery", "ApiController", "UploadAsset");
      ConcurrentDictionary<string, int> existingPaths = new ConcurrentDictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UploadAssetsAsync"))
      {
        if (IncludeAssetTypes == null && ExcludeAssetTypes == null)
          requestContext.Trace(12061098, TraceLevel.Warning, "gallery", nameof (UploadAssets), "Async Asset Upload Enabled: IncludeAssets and ExcludeAsset both are null.");
        if (packageDetails.Manifest.Metadata.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) && IncludeAssetTypes == null && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetCDN") && string.IsNullOrWhiteSpace(packageDetails.Manifest.AssetCDNRoot))
          requestContext.Trace(12061098, TraceLevel.Warning, "gallery", nameof (UploadAssets), "Async Asset Upload Enabled: AssetCDNRoot is null or empty.");
      }
      if (IncludeAssetTypes != null && ExcludeAssetTypes != null)
        throw new ArgumentException("Please specify either IncludeAssetTypes or ExcludeAssetTypes. Both parameters cannot be used simultaneously");
      if (packageDetails == null || packageDetails.Manifest == null || packageDetails.Manifest.Metadata == null || packageDetails.Manifest.Metadata.Identity == null)
        throw new ArgumentNullException(nameof (packageDetails));
      string extensionVersionAssetRoot = packageDetails.Manifest.AssetCDNRoot;
      if (string.IsNullOrWhiteSpace(extensionVersionAssetRoot))
        extensionVersionAssetRoot = Math.Truncate(DateTime.Now.ToUniversalTime().Subtract(GalleryConstants.BeginningOfTime).TotalMilliseconds).ToString();
      bool assetUploadedToCDN = false;
      bool multiLangAssets = this.HasExtensionMultiLangAssets(packageDetails);
      try
      {
        IVssDeploymentServiceHost deploymentHost = requestContext.ServiceHost.DeploymentServiceHost;
        bool isServicingContext = requestContext.IsServicingContext;
        packageDetails = VSIXPackage.Parse(extensionPackageStream, (IImageResizeUtility) new ImageResizeUtils(requestContext), requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MultithreadExtensionsAssetUpload"), (Func<ManifestFile, Stream, bool>) ((manifestFile, stream) =>
        {
          bool flag = false;
          using (IVssRequestContext localDeploymentRequestContext = isServicingContext ? deploymentHost.CreateServicingContext() : deploymentHost.CreateSystemContext())
          {
            GalleryUtil.CheckAssetType(manifestFile.AssetType);
            PublishedExtensionFlags flags = packageDetails.Manifest.Metadata.Flags;
            IEnumerable<InstallationTarget> installation = (IEnumerable<InstallationTarget>) packageDetails.Manifest.Installation;
            if (flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn) && localDeploymentRequestContext.ExecutionEnvironment.IsDevFabricDeployment && localDeploymentRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.LimitedAssetForBuiltInExtensions") && !this.m_AssetsToPublishForBuiltIn.Contains(manifestFile.AssetType))
            {
              localDeploymentRequestContext.Trace(12061055, TraceLevel.Info, "gallery", "SkipAssetUpload", string.Format("Skipping asset upload: PublisherName-{0}, ExtensionName-{1}, Version-{2}, AssetType-{3}", (object) packageDetails.Manifest.Metadata.Identity.PublisherName, (object) packageDetails.Manifest.Metadata.Identity.ExtensionName, (object) packageDetails.Manifest.Metadata.Identity.Version, (object) manifestFile.AssetType));
            }
            else
            {
              if (localDeploymentRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.IconUploadRestriction") && flags.HasFlag((Enum) PublishedExtensionFlags.Public) && !flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn) && !flags.HasFlag((Enum) PublishedExtensionFlags.System) && GalleryUtil.InstallationTargetsHasVSTS(installation) && string.Equals(manifestFile.AssetType, "Microsoft.VisualStudio.Services.Icons.Default", StringComparison.OrdinalIgnoreCase) && manifestFile.ContentType != null && string.Compare(manifestFile.ContentType, "image/svg+xml", StringComparison.OrdinalIgnoreCase) != 0)
              {
                if (stream.Length == 0L)
                  throw new InvalidPackageFormatException(GalleryResources.PublicExtensionDefaultIconAsset());
                using (Bitmap bitmap = new Bitmap(stream))
                {
                  if (bitmap.Height >= 128)
                  {
                    if (bitmap.Width == bitmap.Height)
                      goto label_13;
                  }
                  throw new InvalidPackageFormatException(GalleryResources.PublicExtensionDefaultIconAsset());
                }
label_13:
                stream.Seek(0L, SeekOrigin.Begin);
              }
              if (ExcludeAssetTypes != null)
              {
                if (ExcludeAssetTypes.Contains<string>(manifestFile.AssetType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
                  goto label_30;
              }
              if (IncludeAssetTypes != null)
              {
                if (!IncludeAssetTypes.Contains<string>(manifestFile.AssetType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
                  goto label_30;
              }
              if (!manifestFile.Addressable)
              {
                if (!VSIXPackage.PersistedAssetTypes.Contains(manifestFile.AssetType))
                  goto label_30;
              }
              ITeamFoundationFileService fileService = localDeploymentRequestContext.GetService<ITeamFoundationFileService>();
              localDeploymentRequestContext.Trace(12061055, TraceLevel.Info, "gallery", "UploadAsset", string.Format("Upload asset: PublisherName-{0}, ExtensionName-{1}, Version-{2}, AssetType-{3}, Content-Type-{4}, assetRoot-{5}", (object) packageDetails.Manifest.Metadata.Identity.PublisherName, (object) packageDetails.Manifest.Metadata.Identity.ExtensionName, (object) packageDetails.Manifest.Metadata.Identity.Version, (object) manifestFile.AssetType, (object) manifestFile.ContentType, (object) extensionVersionAssetRoot));
              manifestFile.FileId = existingPaths.GetOrAdd(manifestFile.FullPath, (Func<string, int>) (path =>
              {
                using (localDeploymentRequestContext.CreateTimeToFirstPageExclusionBlock())
                {
                  try
                  {
                    return fileService.UploadFile(localDeploymentRequestContext, stream, OwnerId.Gallery, Guid.Empty);
                  }
                  catch (CircuitBreakerShortCircuitException ex)
                  {
                    requestContext.TraceException(12061055, "gallery", "ApiController", (Exception) ex);
                    Thread.Sleep(10000);
                    return fileService.UploadFile(localDeploymentRequestContext, stream, OwnerId.Gallery, Guid.Empty);
                  }
                }
              }));
              if (!multiLangAssets)
              {
                using (localDeploymentRequestContext.CreateTimeToFirstPageExclusionBlock())
                  assetUploadedToCDN = PublisherAssetService.UploadAssetToCDN(localDeploymentRequestContext, packageDetails, manifestFile, stream, extensionVersionAssetRoot);
              }
              flag = true;
            }
          }
label_30:
          return flag;
        }));
      }
      catch (InvalidPackageFormatException ex)
      {
        requestContext.Trace(12061000, TraceLevel.Error, "gallery", nameof (PublisherAssetService), "ActivityId=" + requestContext.ActivityId.ToString() + "--Exception=" + ex.Message);
        throw;
      }
      catch (InvalidOperationException ex)
      {
        System.Text.RegularExpressions.Match match = Regex.Match(ex.Message, "\\(([^)]*)\\)");
        if (match != null && match.Groups != null && match.Groups.Count > 1 && match.Groups[1].Value != null && ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
        {
          requestContext.TraceException(12061055, TraceLevel.Error, "gallery", "UploadAsset", (Exception) ex);
          throw new InvalidPackageFormatException(GalleryResources.InvalidContentPackageManifest((object) match.Groups[1].Value, (object) ex.InnerException.Message.Replace("Instance validation error:", "")));
        }
        requestContext.TraceException(12061055, TraceLevel.Error, "gallery", "UploadAsset", (Exception) ex);
        throw new InvalidPackageFormatException(ex.Message);
      }
      finally
      {
        requestContext.TraceLeave(12061055, "Gallery", "ApiController", "UploadAsset");
      }
      if (assetUploadedToCDN)
        packageDetails.Manifest.AssetCDNRoot = extensionVersionAssetRoot;
      return packageDetails;
    }

    public int UploadVsixSignatureToFileService(
      IVssRequestContext requestContext,
      Stream signatureAssetStream)
    {
      requestContext.TraceEnter(12061055, "Gallery", "ApiController", nameof (UploadVsixSignatureToFileService));
      int fileService = -1;
      try
      {
        IVssDeploymentServiceHost deploymentServiceHost = requestContext.ServiceHost.DeploymentServiceHost;
        using (IVssRequestContext vssRequestContext = requestContext.IsServicingContext ? deploymentServiceHost.CreateServicingContext() : deploymentServiceHost.CreateSystemContext())
        {
          ITeamFoundationFileService service = vssRequestContext.GetService<ITeamFoundationFileService>();
          try
          {
            using (vssRequestContext.CreateTimeToFirstPageExclusionBlock())
            {
              vssRequestContext.Trace(12061055, TraceLevel.Info, "gallery", nameof (UploadVsixSignatureToFileService), "Uploading signature file to fileService");
              signatureAssetStream.Seek(0L, SeekOrigin.Begin);
              fileService = service.UploadFile(vssRequestContext, signatureAssetStream, OwnerId.Gallery, Guid.Empty);
              vssRequestContext.Trace(12061055, TraceLevel.Info, "gallery", nameof (UploadVsixSignatureToFileService), string.Format("File Uploaded to fileService: FileId-{0}", (object) fileService));
            }
          }
          catch (CircuitBreakerShortCircuitException ex)
          {
            requestContext.TraceException(12061055, "gallery", "ApiController", (Exception) ex);
            Thread.Sleep(10000);
            signatureAssetStream.Seek(0L, SeekOrigin.Begin);
            fileService = service.UploadFile(vssRequestContext, signatureAssetStream, OwnerId.Gallery, Guid.Empty);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061055, TraceLevel.Error, "gallery", nameof (UploadVsixSignatureToFileService), ex);
      }
      finally
      {
        requestContext.TraceLeave(12061055, "Gallery", "ApiController", nameof (UploadVsixSignatureToFileService));
      }
      return fileService;
    }

    public bool UploadVsixSignatureToBlobStorage(
      IVssRequestContext requestContext,
      Stream signatureAssetStream,
      PublishedExtension publishedExtension,
      string cdnRootDirectory,
      string assetType)
    {
      requestContext.TraceEnter(12061055, "Gallery", "ApiController", nameof (UploadVsixSignatureToBlobStorage));
      bool blobStorage = false;
      try
      {
        IVssDeploymentServiceHost deploymentServiceHost = requestContext.ServiceHost.DeploymentServiceHost;
        using (IVssRequestContext requestContext1 = requestContext.IsServicingContext ? deploymentServiceHost.CreateServicingContext() : deploymentServiceHost.CreateSystemContext())
        {
          using (requestContext1.CreateTimeToFirstPageExclusionBlock())
            blobStorage = PublisherAssetService.UploadSignedPackageToCDN(requestContext1, publishedExtension, signatureAssetStream, cdnRootDirectory, assetType, CompressionType.GZip);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061055, TraceLevel.Error, "gallery", nameof (UploadVsixSignatureToBlobStorage), ex);
      }
      finally
      {
        requestContext.TraceLeave(12061055, "Gallery", "ApiController", nameof (UploadVsixSignatureToBlobStorage));
      }
      return blobStorage;
    }

    private static bool UploadSignedPackageToCDN(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension,
      Stream stream,
      string extensionVersionAssetRoot,
      string assetType,
      CompressionType compressionType)
    {
      ICDNService service = requestContext.GetService<ICDNService>();
      bool cdn = false;
      CDNPathUtil cdnAssetPath = new CDNPathUtil()
      {
        PublisherName = publishedExtension.Publisher.PublisherName,
        ExtensionName = publishedExtension.ExtensionName,
        ExtensionVersion = publishedExtension.Versions[0].Version,
        AssetRoot = extensionVersionAssetRoot
      };
      if (compressionType == CompressionType.GZip)
      {
        stream.Seek(0L, SeekOrigin.Begin);
        cdn = service.UploadExtensionAssetWithStream(requestContext, cdnAssetPath, stream, assetType, "application/zip", (string) null);
      }
      return cdn;
    }

    private static bool UploadAssetToCDN(
      IVssRequestContext requestContext,
      PackageDetails packageDetails,
      ManifestFile manifestFile,
      Stream stream,
      string extensionVersionAssetRoot)
    {
      requestContext.TraceEnter(12061055, "Gallery", "ApiController", nameof (UploadAssetToCDN));
      bool cdn = false;
      bool flag1 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetCDN");
      bool flag2 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UploadPrivateExtensionAssetsToCdn");
      bool flag3 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetUploadToCDNPostValidation");
      requestContext.Trace(12061055, TraceLevel.Info, "gallery", "UploadAsset", string.Format("Upload asset CDN: Extension is public-{0}, AssetCDN feature flag enabled-{1}", (object) packageDetails.Manifest.Metadata.Flags.HasFlag((Enum) PublishedExtensionFlags.Public).ToString(), (object) flag1.ToString()));
      if ((packageDetails.Manifest.Metadata.Flags.HasFlag((Enum) PublishedExtensionFlags.Public) | flag2) & flag1 && !flag3)
      {
        ICDNService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ICDNService>();
        stream.Seek(0L, SeekOrigin.Begin);
        CDNPathUtil cdnAssetPath = new CDNPathUtil()
        {
          PublisherName = packageDetails.Manifest.Metadata.Identity.PublisherName,
          ExtensionName = packageDetails.Manifest.Metadata.Identity.ExtensionName,
          ExtensionVersion = packageDetails.Manifest.Metadata.Identity.Version,
          AssetRoot = extensionVersionAssetRoot
        };
        if (string.Compare(manifestFile.ContentType, "application/zip", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(manifestFile.AssetType, "Microsoft.VisualStudio.Services.VSIXPackage", StringComparison.OrdinalIgnoreCase) == 0)
        {
          cdn = service.UploadExtensionAssetWithStream(requestContext, cdnAssetPath, stream, manifestFile.AssetType, manifestFile.ContentType, (string) null);
        }
        else
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            using (GZipStream destination = new GZipStream((Stream) memoryStream, CompressionMode.Compress, true))
              stream.CopyTo((Stream) destination);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            cdn = service.UploadExtensionAssetWithStream(requestContext, cdnAssetPath, (Stream) memoryStream, manifestFile.AssetType, manifestFile.ContentType);
          }
        }
      }
      requestContext.TraceLeave(12061055, "Gallery", "ApiController", nameof (UploadAssetToCDN));
      return cdn;
    }

    private bool HasExtensionMultiLangAssets(PackageDetails packageDetails)
    {
      bool flag = false;
      if (packageDetails != null && packageDetails.Manifest != null && packageDetails.Manifest.Assets != null)
      {
        foreach (ExtensionFile asset in packageDetails.Manifest.Assets)
        {
          if (!string.IsNullOrWhiteSpace(asset.Language))
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadConfiguration(requestContext);
    }

    private void LoadConfiguration(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherAsset/*");
      this.m_configuration = new PublisherAssetConfiguration()
      {
        Host = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PublisherAsset/Host", (string) null),
        CDNHost = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PublisherAsset/CDNHost", (string) null),
        ChinaCDNHost = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PublisherAsset/ChinaCDNHost", (string) null),
        VirtualPath = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PublisherAsset/Path", "/")
      };
      string accessPoint = requestContext.GetService<ILocationService>().GetPublicAccessMapping(requestContext)?.AccessPoint;
      if (string.IsNullOrWhiteSpace(accessPoint))
        return;
      this.m_scheme = new Uri(accessPoint).Scheme;
    }

    public void LoadDefaultAssets(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      foreach (RegistryEntry readEntry in service.ReadEntries(systemRequestContext, (RegistryQuery) string.Format("/Configuration/Service/Gallery/Assets/{0}", (object) "*")))
      {
        int result;
        if (int.TryParse(readEntry.Value, out result))
        {
          PublisherAssetService.DefaultAsset defaultAsset = new PublisherAssetService.DefaultAsset()
          {
            ContentType = "application/octet-stream",
            FileId = result
          };
          string str = service.GetValue<string>(systemRequestContext, (RegistryQuery) string.Format("{0}/ContentType", (object) readEntry.Path), false, (string) null);
          if (!string.IsNullOrEmpty(str))
            defaultAsset.ContentType = str;
          this.m_defaultAssets[readEntry.Name] = defaultAsset;
        }
      }
    }

    public ExtensionAsset QueryAsset(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      Guid validationId,
      IEnumerable<AssetInfo> assetTypes,
      string accountToken,
      string assetToken,
      bool acceptDefault,
      string targetPlatform = null)
    {
      string versionQuery = requestContext.GetService<IPublishedExtensionService>().GetVersionQuery(version);
      PublishedExtension extension;
      using (PublishedExtensionComponent component = requestContext.CreateComponent<PublishedExtensionComponent>())
        extension = component.QueryExtensionById(extensionId, versionQuery, validationId, ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeMetadata | ExtensionQueryFlags.IncludeSharedOrganizations);
      if (extension.IsVsExtension())
      {
        foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
        {
          if (extensionMetadata.Key.Equals("DeploymentTechnology") && extensionMetadata.Value.Equals("Referral Link"))
          {
            foreach (AssetInfo assetType in assetTypes)
            {
              if (assetType.AssetType.Equals("Microsoft.VisualStudio.Ide.Payload"))
                return new ExtensionAsset()
                {
                  Extension = extension,
                  AssetFile = (ExtensionFile) null
                };
            }
          }
        }
      }
      return this.QueryAsset(requestContext, extension, version, assetTypes, accountToken, assetToken, acceptDefault, targetPlatform);
    }

    public ExtensionAsset QueryAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      Guid validationId,
      IEnumerable<AssetInfo> assetTypes,
      string accountToken,
      string assetToken,
      bool acceptDefault,
      string targetPlatform = null)
    {
      IPublishedExtensionService service1 = requestContext.GetService<IPublishedExtensionService>();
      QueryExtensionCacheService service2 = requestContext.GetService<QueryExtensionCacheService>();
      string version1 = version;
      string versionQuery = service1.GetVersionQuery(version1);
      PublishedExtension extension = service2.QueryExtension(requestContext, publisherName, extensionName, versionQuery, validationId, ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeSharedAccounts | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeMetadata | ExtensionQueryFlags.IncludeSharedOrganizations);
      if (extension.IsVsExtension())
      {
        foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
        {
          if (extensionMetadata.Key.Equals("DeploymentTechnology") && extensionMetadata.Value.Equals("Referral Link") && assetTypes != null)
          {
            foreach (AssetInfo assetType in assetTypes)
            {
              if (assetType != null && !assetType.AssetType.IsNullOrEmpty<char>() && assetType.AssetType.Equals("Microsoft.VisualStudio.Ide.Payload"))
                return new ExtensionAsset()
                {
                  Extension = extension,
                  AssetFile = (ExtensionFile) null
                };
            }
          }
        }
      }
      return this.QueryAsset(requestContext, extension, version, assetTypes, accountToken, assetToken, acceptDefault, targetPlatform);
    }

    public Uri GetAssetUri(
      IVssRequestContext requestContext,
      ExtensionAsset extensionAsset,
      bool cdnSupported = false)
    {
      ILocationService service1 = requestContext.GetService<ILocationService>();
      Guid identifier = GalleryResourceIds.AssetLocationByNameId;
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      routeValues["publisherName"] = (object) extensionAsset.Extension.Publisher.PublisherName;
      routeValues["extensionName"] = (object) extensionAsset.Extension.ExtensionName;
      routeValues["assetType"] = (object) extensionAsset.AssetFile.AssetType;
      routeValues["version"] = (object) extensionAsset.Extension.Versions[0].Version;
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.RemoveAssetTokenFromAssetUrls") && !extensionAsset.AssetFile.IsPublic)
      {
        routeValues["assetToken"] = (object) this.GetTokenForPublisher(requestContext, extensionAsset.Extension.Publisher.PublisherName, extensionAsset.Extension.ExtensionName);
        identifier = GalleryResourceIds.PrivateAssetLocationId;
      }
      UriBuilder uriBuilder;
      if (cdnSupported && this.IsCDNDownloadEnabled(requestContext, extensionAsset.Extension))
      {
        ICDNService service2 = requestContext.To(TeamFoundationHostType.Deployment).GetService<ICDNService>();
        string hostName = extensionAsset.Extension.Publisher.PublisherName + this.m_configuration.CDNHost;
        if (this.IsMooncakeEnabled(requestContext) && requestContext.UserAgent != null && requestContext.UserAgent.StartsWith("VSCode", StringComparison.OrdinalIgnoreCase) && PublisherAssetService.IsChinaRequest(requestContext))
          hostName = extensionAsset.Extension.Publisher.PublisherName + this.m_configuration.ChinaCDNHost;
        uriBuilder = new UriBuilder(this.m_scheme, hostName);
        uriBuilder.Path = service2.GetExtensionAssetCDNPath(extensionAsset);
      }
      else
      {
        uriBuilder = new UriBuilder(service1.GetResourceUri(requestContext, "gallery", identifier, (object) routeValues));
        if (!string.IsNullOrEmpty(this.m_configuration.Host))
          uriBuilder.Host = extensionAsset.Extension.Publisher.PublisherName + this.m_configuration.Host;
      }
      return uriBuilder.Uri;
    }

    public Uri GetAssetUri(IVssRequestContext requestContext, string baseUri, string assetType)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(baseUri, nameof (baseUri));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(assetType, nameof (assetType));
      return new Uri(baseUri + "/" + assetType);
    }

    private bool IsCDNDownloadEnabled(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      bool flag = false;
      if (!string.IsNullOrWhiteSpace(this.m_configuration.CDNHost) && extension.Versions[0].IsCdnEnabled && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetCDN") && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ReturnCDNURL") && (requestContext.UserAgent != null && requestContext.UserAgent.StartsWith("VSCode", StringComparison.OrdinalIgnoreCase) || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ReturnCDNURLNonVSCode")))
        flag = true;
      return flag;
    }

    private bool IsMooncakeEnabled(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (!string.IsNullOrWhiteSpace(this.m_configuration.ChinaCDNHost) && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetCdnChina"))
        flag = true;
      return flag;
    }

    private static bool IsChinaRequest(IVssRequestContext requestContext)
    {
      string requestCountryCode = requestContext.GetService<IGeoLocationService>().GetRequestCountryCode(requestContext);
      return !string.IsNullOrEmpty(requestCountryCode) && requestCountryCode.Equals("CN", StringComparison.OrdinalIgnoreCase);
    }

    private ExtensionAsset QueryAsset(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version,
      IEnumerable<AssetInfo> assetTypes,
      string accountToken,
      string assetToken,
      bool acceptDefault,
      string targetPlatform = null)
    {
      ExtensionFile extensionFile1 = (ExtensionFile) null;
      string key = (string) null;
      string name = (string) null;
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.RemoveAssetTokenFromAssetUrls"))
        GallerySecurity.CheckAssetPermissions(requestContext, extension, accountToken, assetToken);
      if (extension.Versions == null || extension.Versions.Count == 0)
        throw new ExtensionVersionNotFoundException(GalleryWebApiResources.ExtensionVersionNotFound((object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), (object) version.ToString()));
      if (extension.IsVsCodeExtension())
      {
        if (targetPlatform != null)
        {
          GalleryServerUtil.ValidateIfExtensionVersionEverSupportedTargetPlatform(requestContext, extension, version, targetPlatform);
          extension.Versions = extension.Versions.Where<ExtensionVersion>((Func<ExtensionVersion, bool>) (extensionVersion => extensionVersion.Version == version && extensionVersion.TargetPlatform == targetPlatform)).ToList<ExtensionVersion>();
        }
        else
        {
          List<ExtensionVersion> list = extension.Versions.Where<ExtensionVersion>((Func<ExtensionVersion, bool>) (extensionVersion => extensionVersion.Version == version && extensionVersion.TargetPlatform == null)).ToList<ExtensionVersion>();
          extension.Versions = list.Count > 0 ? list : extension.Versions.Take<ExtensionVersion>(1).ToList<ExtensionVersion>();
        }
      }
      else if (extension.IsVsExtension())
      {
        if (!string.IsNullOrWhiteSpace(targetPlatform))
        {
          if (!GalleryServerUtil.IsVersionHasSupportForTargetPlatform(extension, version, targetPlatform))
            throw new ExtensionVersionHasNoSupportForRequestedTargetPlatformsException(GalleryResources.ErrorExtensionVersionHasNoSupportForRequestedTargetPlatform((object) version, (object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), (object) targetPlatform));
          extension.Versions.RemoveAll((Predicate<ExtensionVersion>) (extensionVersion => !string.Equals(extensionVersion.TargetPlatform, targetPlatform, StringComparison.OrdinalIgnoreCase)));
        }
      }
      else
        targetPlatform = (string) null;
      if (extension.Versions[0].Files != null)
      {
        ExtensionFile extensionFile2 = (ExtensionFile) null;
        int num = int.MaxValue;
        foreach (AssetInfo assetType in assetTypes)
        {
          key = assetType.AssetType;
          name = assetType.Language;
          foreach (ExtensionFile file in extension.Versions[0].Files)
          {
            if (file.AssetType.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
              if (requestContext.ExecutionEnvironment.IsHostedDeployment && key.Equals("Microsoft.VisualStudio.Services.VSIXPackage") && extension.IsPaid() && !this.IsPaidExtensionDownloadSupported(requestContext.UserAgent))
                throw new ExtensionAssetNotFoundException(GalleryWebApiResources.ExtensionAssetNotFound((object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), (object) version.ToString(), (object) key));
              if (extensionFile2 == null)
                extensionFile2 = file;
              if (file.Language == null)
              {
                extensionFile2 = file;
                if (name == null)
                {
                  num = 0;
                  break;
                }
              }
              else if (name != null)
              {
                int matchingDistance = GalleryUtil.GetCultureMatchingDistance(CultureInfo.GetCultureInfo(name), CultureInfo.GetCultureInfo(file.Language));
                if (matchingDistance != -1 && matchingDistance < num)
                {
                  extensionFile1 = file;
                  num = matchingDistance;
                }
                if (num == 0)
                  break;
              }
            }
          }
          if (num == 0)
            break;
        }
        if (extensionFile1 == null && extensionFile2 != null)
          extensionFile1 = extensionFile2;
      }
      if (extensionFile1 == null)
      {
        if (key == null)
          key = (assetTypes.FirstOrDefault<AssetInfo>((Func<AssetInfo, bool>) (tempAssetInfo => tempAssetInfo.AssetType != null)) ?? throw new ExtensionAssetNotFoundException(GalleryWebApiResources.ExtensionAssetNotFound((object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), (object) version.ToString(), (object) string.Empty))).AssetType;
        PublisherAssetService.DefaultAsset defaultAsset;
        if (!acceptDefault || !this.m_defaultAssets.TryGetValue(key, out defaultAsset))
          throw new ExtensionAssetNotFoundException(GalleryWebApiResources.ExtensionAssetNotFound((object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), (object) version.ToString(), (object) key));
        extensionFile1 = new ExtensionFile()
        {
          AssetType = key,
          ContentType = defaultAsset.ContentType,
          FileId = defaultAsset.FileId,
          IsDefault = true,
          Language = name
        };
      }
      extensionFile1.IsPublic = extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public);
      return new ExtensionAsset()
      {
        Extension = extension,
        AssetFile = extensionFile1
      };
    }

    public void DeleteExtensionAssets(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version = null)
    {
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ITeamFoundationFileService service1 = requestContext.GetService<ITeamFoundationFileService>();
      ICDNService service2 = requestContext.GetService<ICDNService>();
      foreach (ExtensionVersion version1 in extension.Versions)
      {
        if (string.IsNullOrEmpty(version) || string.Equals(version, version1.Version))
          PublisherAssetService.DeleteAssetsPerExtensionVersion(requestContext, service1, service2, extension, version1);
      }
    }

    private static void DeleteAssetsPerExtensionVersion(
      IVssRequestContext requestContext,
      ITeamFoundationFileService fileService,
      ICDNService cdnService,
      PublishedExtension extension,
      ExtensionVersion extensionVersion)
    {
      if (extensionVersion.Files == null)
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add(CustomerIntelligenceProperty.Action, "DeleteExtensionAssets");
        intelligenceData.Add("ExtensionId", (object) extension.ExtensionId);
        intelligenceData.Add("ExtensionName", extension.ExtensionName);
        intelligenceData.Add("PublisherName", extension.Publisher.PublisherName);
        intelligenceData.Add("ExtensionVersion", extensionVersion.Version);
        intelligenceData.Add("Message", "This extension version has no files associated at the time of asset deletion");
        intelligenceData.AddGalleryUserIdentifier(requestContext);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Publisher", intelligenceData);
      }
      else
      {
        requestContext.Trace(12061069, TraceLevel.Info, "gallery", "DeleteExtensionAssets", string.Format("Deleting Assets for Extension: {0}.{1}, Version: {2}.", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) extensionVersion.Version));
        requestContext.Trace(12061069, TraceLevel.Info, "gallery", "DeleteExtensionAssets", "Attempting to deleting assets from File Service.");
        List<int> fileIds = new List<int>();
        foreach (ExtensionFile file in extensionVersion.Files)
          fileIds.Add(file.FileId);
        try
        {
          fileService.DeleteFiles(requestContext, (IEnumerable<int>) fileIds);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12061069, "gallery", "DeleteExtensionAssets", ex);
        }
        if (!extensionVersion.IsCdnEnabled || string.IsNullOrEmpty(extensionVersion.CdnDirectory))
          return;
        requestContext.Trace(12061069, TraceLevel.Info, "gallery", "DeleteExtensionAssets", "Attempting to deleting assets from CDN.");
        CDNPathUtil cdnAssetPath = new CDNPathUtil()
        {
          PublisherName = extension.Publisher.PublisherName,
          ExtensionName = extension.ExtensionName,
          ExtensionVersion = extensionVersion.Version,
          AssetRoot = extensionVersion.CdnDirectory
        };
        foreach (ExtensionFile file in extensionVersion.Files)
        {
          try
          {
            if (!cdnService.DeleteExtensionAsset(requestContext, cdnAssetPath, file.AssetType))
              requestContext.Trace(12061069, TraceLevel.Error, "gallery", "DeleteExtensionAssets", string.Format("Failed to delete asset {0}.", (object) file.AssetType));
          }
          catch (Exception ex)
          {
            requestContext.Trace(12061069, TraceLevel.Error, "gallery", "DeleteExtensionAssets", string.Format("Exception while deleting asset {0}. \nException details : {1}.", (object) file.AssetType, (object) ex.Message));
          }
        }
      }
    }

    private bool IsPaidExtensionDownloadSupported(string userAgent) => string.IsNullOrEmpty(userAgent) || !userAgent.Equals("VSServices/15.103.25603.0 (TfsJobAgent.exe)", StringComparison.InvariantCultureIgnoreCase);

    private class DefaultAsset
    {
      public string ContentType { get; set; }

      public int FileId { get; set; }
    }
  }
}
