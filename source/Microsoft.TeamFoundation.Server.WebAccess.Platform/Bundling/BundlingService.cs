// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundlingService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class BundlingService : 
    VssMemoryCacheService<string, BundledFileGenerator>,
    IVssFrameworkService
  {
    public const string BundlePrefix = "vss-bundle-";
    private const string c_bundlingRegistryKey = "/Configuration/WebAccess/Bundling";
    private const string c_bundleToInvalidateRegistryPrefix = "BundleToInvalidate-";
    private const string c_enableIntegrityHash = "VisualStudio.Services.WebPlatform.EnableIntegrityHashForLegacyBundles";
    internal static readonly string s_bundlingControllerName = "Bundling";
    internal static readonly string s_area = nameof (BundlingService);
    internal static readonly string s_layer = "WebPlatform";
    private static readonly string s_defaultBlobPrefix = "bundles/";
    private static readonly string s_cssBlobPrefixFormat = "v/{0}/_cssbundles/{1}/";
    private static readonly string s_blobStorageConnectionStringOverrideKey = "BlobStorageConnectionStringOverride";
    private static readonly string s_javascriptContentType = "application/x-javascript";
    private static readonly string s_cssContentType = "text/css";
    private string m_cdnContainerName;
    private IBlobProvider m_cdnBlobProvider;
    private TeamFoundationFileService m_fileService;
    private static ConcurrentDictionary<string, BundledFilePublisher> s_bundlePublishers = new ConcurrentDictionary<string, BundledFilePublisher>();
    private ConcurrentDictionary<string, string> m_invalidatedBundles;

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_fileService = systemRequestContext.GetService<TeamFoundationFileService>();
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        this.ConfigureCdnBlobProvider(systemRequestContext);
      CachedRegistryService service = systemRequestContext.GetService<CachedRegistryService>();
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/WebAccess/CDN/...");
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnBundlingRegistryChanged), "/Configuration/WebAccess/Bundling/...");
      this.m_invalidatedBundles = new ConcurrentDictionary<string, string>();
      this.PopulateBundleEntries(systemRequestContext, service.ReadEntries(systemRequestContext, new RegistryQuery("/Configuration/WebAccess/Bundling/...")));
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        "CDNStorageConnectionString"
      });
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      CachedRegistryService service = systemRequestContext.GetService<CachedRegistryService>();
      service.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      service.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnBundlingRegistryChanged));
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged));
      this.m_fileService = (TeamFoundationFileService) null;
      this.m_cdnBlobProvider = (IBlobProvider) null;
    }

    public BundledScriptFile RegisterScriptBundle(
      IVssRequestContext requestContext,
      ScriptBundleDefinition definition,
      UrlHelper urlHelper)
    {
      return this.GetOrCreateBundledFile<BundledScriptFile>(requestContext, (BundleDefinition) definition, urlHelper, (Func<IVssRequestContext, UrlHelper, BundledFile>) ((vssRequestContext, url) => (BundledFile) this.AddScriptBundle(vssRequestContext, definition, url, definition.Name)));
    }

    public BundledCSSFile RegisterCSSBundle(
      IVssRequestContext requestContext,
      CSSBundleDefinition definition,
      UrlHelper urlHelper)
    {
      return this.GetOrCreateBundledFile<BundledCSSFile>(requestContext, (BundleDefinition) definition, urlHelper, (Func<IVssRequestContext, UrlHelper, BundledFile>) ((vssRequestContext, url) => (BundledFile) this.AddCSSBundle(vssRequestContext, definition, url, definition.Name)));
    }

    private T GetOrCreateBundledFile<T>(
      IVssRequestContext requestContext,
      BundleDefinition definition,
      UrlHelper urlHelper,
      Func<IVssRequestContext, UrlHelper, BundledFile> createNewBundle)
      where T : BundledFile
    {
      string definitionHashCode = definition.GetDefinitionHashCode();
      string cacheKey;
      if (this.m_invalidatedBundles.TryGetValue(definitionHashCode, out cacheKey))
      {
        definition.SetCacheKey(cacheKey);
        definitionHashCode = definition.GetDefinitionHashCode();
      }
      BundledFileGenerator bundledFileGenerator = (BundledFileGenerator) null;
      if (!this.TryGetValue(requestContext, definitionHashCode, out bundledFileGenerator))
      {
        if (!definition.Diagnose)
        {
          BundlingService.HttpContextInfoForCi contextInfoForCi = this.GetHttpContextInfoForCi();
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("action", "BundleMemoryCacheMiss");
          intelligenceData.Add("definitionKey", definitionHashCode);
          intelligenceData.Add("dataForHash", definition.DataUsedForHash);
          intelligenceData.Add("routeAction", contextInfoForCi.Action);
          intelligenceData.Add("controller", contextInfoForCi.Controller);
          IVssRequestContext requestContext1 = requestContext;
          string layer = BundlingService.s_layer;
          string area = BundlingService.s_area;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(requestContext1, layer, area, properties);
        }
        bundledFileGenerator = new BundledFileGenerator(definitionHashCode, createNewBundle);
        this.TryAdd(requestContext, definitionHashCode, bundledFileGenerator);
      }
      return (T) bundledFileGenerator.GetFile(requestContext, urlHelper);
    }

    private BundlingService.HttpContextInfoForCi GetHttpContextInfoForCi()
    {
      BundlingService.HttpContextInfoForCi contextInfoForCi = new BundlingService.HttpContextInfoForCi();
      if (HttpContext.Current != null && HttpContext.Current.Request != null)
      {
        HttpRequest request = HttpContext.Current.Request;
        if (request.RequestContext != null && request.RequestContext.RouteData != null && request.RequestContext.RouteData.Values != null)
        {
          object obj1;
          request.RequestContext.RouteData.Values.TryGetValue("action", out obj1);
          if (obj1 != null)
            contextInfoForCi.Action = obj1.ToString();
          object obj2;
          request.RequestContext.RouteData.Values.TryGetValue("controller", out obj2);
          if (obj2 != null)
            contextInfoForCi.Controller = obj2.ToString();
        }
        contextInfoForCi.Url = !(contextInfoForCi.Controller == BundlingService.s_bundlingControllerName) ? request.Url : request.UrlReferrer;
      }
      return contextInfoForCi;
    }

    private BundledScriptFile AddScriptBundle(
      IVssRequestContext requestContext,
      ScriptBundleDefinition definition,
      UrlHelper urlHelper,
      string bundleFileName)
    {
      BundledScriptFile bundledScriptFile = new BundledScriptFile();
      bundledScriptFile.Definition = definition;
      Dictionary<string, string[]> contentCache = new Dictionary<string, string[]>();
      int bundleLength;
      string bundleHashCode = definition.GetBundleHashCode(requestContext, (BundledFile) bundledScriptFile, urlHelper, contentCache, out bundleLength);
      bundledScriptFile.Name = this.GenerateBundleFileFullName(bundleFileName, bundleHashCode);
      bundledScriptFile.ContentLength = bundleLength;
      if (string.IsNullOrEmpty(bundleHashCode))
      {
        bundledScriptFile.IsEmpty = true;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.EnableIntegrityHashForLegacyBundles"))
          bundledScriptFile.Integrity = BundlingService.CalculateIntegrity(requestContext, (BundleDefinition) definition, urlHelper, (BundledFile) bundledScriptFile, contentCache);
      }
      else
      {
        BundledFilePublisher bundleFilePublisher = this.GetBundleFilePublisher((BundleDefinition) definition, urlHelper, (BundledFile) bundledScriptFile, BundlingService.s_javascriptContentType, BundlingService.s_defaultBlobPrefix);
        bundledScriptFile = bundleFilePublisher.BundledFile as BundledScriptFile;
        if (bundledScriptFile.Integrity == null && requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.EnableIntegrityHashForLegacyBundles"))
          bundledScriptFile.Integrity = BundlingService.CalculateIntegrity(requestContext, (BundleDefinition) definition, urlHelper, (BundledFile) bundledScriptFile, contentCache);
        if (!bundleFilePublisher.IsBundlePublishCheckComplete)
          bundleFilePublisher.EnsureBundleIsPublished(requestContext, contentCache, definition.Diagnose);
      }
      if (bundledScriptFile.LocalUri == null)
        bundledScriptFile.LocalUri = this.GetLocalScriptUri(requestContext, bundledScriptFile.Name);
      return bundledScriptFile;
    }

    internal static string CalculateIntegrity(
      IVssRequestContext requestContext,
      BundleDefinition definition,
      UrlHelper urlHelper,
      BundledFile bundledFile,
      Dictionary<string, string[]> contentCache)
    {
      try
      {
        using (SHA256Cng shA256Cng = new SHA256Cng())
        {
          requestContext.TraceEnter(15060011, BundlingService.s_area, BundlingService.s_layer, nameof (CalculateIntegrity));
          byte[] bytes = Encoding.UTF8.GetBytes(definition.GetBundleContent(requestContext, bundledFile, urlHelper, contentCache));
          string base64String = Convert.ToBase64String(shA256Cng.ComputeHash(bytes));
          requestContext.Trace(15060012, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Bundle: {0}, Size: {1}, Integrity: {2}", (object) bundledFile.Name, (object) bytes.Length, (object) base64String);
          return "sha256-" + base64String;
        }
      }
      finally
      {
        requestContext.TraceLeave(15060020, BundlingService.s_area, BundlingService.s_layer, nameof (CalculateIntegrity));
      }
    }

    private BundledCSSFile AddCSSBundle(
      IVssRequestContext requestContext,
      CSSBundleDefinition definition,
      UrlHelper urlHelper,
      string bundleFileName)
    {
      string bundleNamePrefix = string.Format(BundlingService.s_cssBlobPrefixFormat, (object) StaticResources.Versioned.Version, (object) Uri.EscapeDataString(definition.ThemeName));
      BundledCSSFile bundledCssFile = new BundledCSSFile();
      bundledCssFile.Definition = definition;
      Dictionary<string, string[]> contentCache = new Dictionary<string, string[]>();
      int bundleLength;
      string bundleHashCode = definition.GetBundleHashCode(requestContext, (BundledFile) bundledCssFile, urlHelper, contentCache, out bundleLength);
      bundledCssFile.ContentLength = bundleLength;
      bundledCssFile.Name = this.GenerateBundleFileFullName(bundleFileName, bundleHashCode);
      if (string.IsNullOrEmpty(bundleHashCode))
      {
        bundledCssFile.IsEmpty = true;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.EnableIntegrityHashForLegacyBundles"))
          bundledCssFile.Integrity = BundlingService.CalculateIntegrity(requestContext, (BundleDefinition) definition, urlHelper, (BundledFile) bundledCssFile, contentCache);
      }
      else
      {
        BundledFilePublisher bundleFilePublisher = this.GetBundleFilePublisher((BundleDefinition) definition, urlHelper, (BundledFile) bundledCssFile, BundlingService.s_cssContentType, bundleNamePrefix);
        bundledCssFile = bundleFilePublisher.BundledFile as BundledCSSFile;
        if (bundledCssFile.Integrity == null && requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.EnableIntegrityHashForLegacyBundles"))
          bundledCssFile.Integrity = BundlingService.CalculateIntegrity(requestContext, (BundleDefinition) definition, urlHelper, (BundledFile) bundledCssFile, contentCache);
        if (!bundleFilePublisher.IsBundlePublishCheckComplete)
          bundleFilePublisher.EnsureBundleIsPublished(requestContext, contentCache);
      }
      if (bundledCssFile.LocalUri == null)
        bundledCssFile.LocalUri = this.GetLocalCssUri(requestContext, definition.ThemeName, bundledCssFile.Name, definition.StaticContentVersion);
      return bundledCssFile;
    }

    private BundledFilePublisher GetBundleFilePublisher(
      BundleDefinition definition,
      UrlHelper urlHelper,
      BundledFile bundledFile,
      string contentType,
      string bundleNamePrefix)
    {
      BundledFilePublisher bundleFilePublisher;
      if (!BundlingService.s_bundlePublishers.TryGetValue(bundledFile.Name, out bundleFilePublisher))
      {
        BundledFilePublisher bundledFilePublisher = new BundledFilePublisher(this.m_fileService, this.m_cdnBlobProvider, this.m_cdnContainerName, definition, urlHelper, bundledFile, contentType, bundleNamePrefix);
        if (BundlingService.s_bundlePublishers.TryAdd(bundledFile.Name, bundledFilePublisher))
          bundleFilePublisher = bundledFilePublisher;
        else
          BundlingService.s_bundlePublishers.TryGetValue(bundledFile.Name, out bundleFilePublisher);
      }
      return bundleFilePublisher;
    }

    private string GenerateBundleFileFullName(string bundleFileShortName, string bundleHash)
    {
      string bundleFileFullName = bundleFileShortName;
      if (!string.IsNullOrEmpty(bundleHash))
      {
        string str = bundleHash.Replace('/', '_').Replace('+', '-');
        bundleFileFullName = bundleFileFullName + "-v" + str;
      }
      return bundleFileFullName;
    }

    public string GetLocalCssUri(
      IVssRequestContext requestContext,
      string themeName,
      string bundledFileName,
      string staticContentVersion)
    {
      string localLocation = StaticResources.Versioned.CssBundles.GetLocalLocation(Uri.EscapeDataString(themeName) + "/" + bundledFileName, requestContext, staticContentVersion);
      return BundlingHelper.GetCacheableStaticUrl(requestContext, localLocation);
    }

    public string GetLocalScriptUri(IVssRequestContext requestContext, string bundleName)
    {
      string absolute = VirtualPathUtility.ToAbsolute("~/_public/_Bundling/Content?bundle=" + bundleName, requestContext.WebApplicationPath());
      return BundlingHelper.GetCacheableStaticUrl(requestContext, absolute);
    }

    public Stream RetrieveBundleContent(IVssRequestContext requestContext, string bundleFileName)
    {
      string methodName = nameof (RetrieveBundleContent);
      requestContext.TraceEnter(15060005, BundlingService.s_area, BundlingService.s_layer, methodName);
      try
      {
        int fileId;
        return !string.IsNullOrEmpty(bundleFileName) && bundleFileName.StartsWith("vss-bundle-", StringComparison.OrdinalIgnoreCase) && this.m_fileService.TryGetFileId(requestContext, OwnerId.WebAccess, bundleFileName, out fileId) ? this.m_fileService.RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _) : (Stream) null;
      }
      finally
      {
        requestContext.TraceLeave(15060006, BundlingService.s_area, BundlingService.s_layer, methodName);
      }
    }

    private void ConfigureCdnBlobProvider(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (!service.GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/WebAccess/CDN/Enabled", false))
      {
        this.m_cdnBlobProvider = (IBlobProvider) null;
      }
      else
      {
        this.m_cdnContainerName = service.GetValue(requestContext, (RegistryQuery) "/Configuration/WebAccess/CDN/StorageContainerName", (string) null);
        string typeName = service.GetValue(requestContext, (RegistryQuery) "/Configuration/WebAccess/CDN/RemoteBlobProvider", (string) null);
        if (string.IsNullOrEmpty(this.m_cdnContainerName) || string.IsNullOrEmpty(typeName))
        {
          this.m_cdnBlobProvider = (IBlobProvider) null;
        }
        else
        {
          StrongBoxItemInfo itemInfo = StrongBoxMigrationHelper.GetItemInfo(requestContext, "ConfigurationSecrets", "CDNStorageConnectionString", "StorageConnections", "CdnStorageAccount", false);
          if (itemInfo == null)
          {
            this.m_cdnBlobProvider = (IBlobProvider) null;
          }
          else
          {
            string str = requestContext.GetService<ITeamFoundationStrongBoxService>().GetString(requestContext, itemInfo);
            if (string.IsNullOrEmpty(str))
            {
              this.m_cdnBlobProvider = (IBlobProvider) null;
            }
            else
            {
              try
              {
                blobProvider = (IBlobProvider) null;
                Type type1 = Type.GetType(typeName);
                if (type1 != (Type) null)
                {
                  if (!(Activator.CreateInstance(type1) is IBlobProvider blobProvider))
                    requestContext.Trace(14449, TraceLevel.Warning, BundlingService.s_area, BundlingService.s_layer, "Type '" + typeName + "' does not implement IBlobProvider");
                }
                else
                  requestContext.Trace(14448, TraceLevel.Warning, BundlingService.s_area, BundlingService.s_layer, "Could not find type: " + typeName);
                if (blobProvider == null)
                {
                  string[] array = ((IEnumerable<string>) typeName.Split(',')).Select<string, string>((Func<string, string>) (part => part.Trim())).ToArray<string>();
                  string remoteBlobProviderType = array[0];
                  string remoteBlobProviderAssembly = array.Length >= 2 ? array[1] : (string) null;
                  blobProvider = requestContext.GetExtension<IBlobProvider>((Func<IBlobProvider, bool>) (x =>
                  {
                    Type type2 = x.GetType();
                    if (!type2.FullName.Equals(remoteBlobProviderType, StringComparison.Ordinal))
                      return false;
                    return string.IsNullOrEmpty(remoteBlobProviderAssembly) || remoteBlobProviderAssembly.Equals(type2.Assembly.GetName().Name, StringComparison.Ordinal);
                  }));
                }
                if (blobProvider != null)
                {
                  Type type3 = blobProvider.GetType();
                  requestContext.Trace(15060007, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Found plugin {0}", (object) type3.AssemblyQualifiedName);
                  blobProvider.ServiceStart(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
                  {
                    [BundlingService.s_blobStorageConnectionStringOverrideKey] = str
                  });
                  requestContext.Trace(15060008, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Plugin {0} started successfully", (object) type3);
                }
                else
                  requestContext.Trace(15060009, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Found no plugins implementing IBlobProvider type {0}.", (object) typeName);
                this.m_cdnBlobProvider = blobProvider;
              }
              catch (Exception ex)
              {
                requestContext.TraceException(15060010, BundlingService.s_area, BundlingService.s_layer, ex);
                throw;
              }
            }
          }
        }
      }
    }

    private void OnBundlingRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection bundleEntries)
    {
      if (bundleEntries != null)
      {
        string str1 = string.Join(" | ", bundleEntries.Select<RegistryEntry, string>((Func<RegistryEntry, string>) (re => re.Path)));
        string str2 = string.Join(" | ", bundleEntries.Select<RegistryEntry, string>((Func<RegistryEntry, string>) (re => re.Value)));
        requestContext.Trace(15060025, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, string.Format("Bundle removal request came in. Count: {0}, Paths: {1}, Value: {2}", (object) bundleEntries.Count, (object) str1, (object) str2));
        this.PopulateBundleEntries(requestContext, bundleEntries);
      }
      else
        requestContext.Trace(15060026, TraceLevel.Warning, BundlingService.s_area, BundlingService.s_layer, "An empty bundle removal request came in.");
    }

    private void PopulateBundleEntries(
      IVssRequestContext requestContext,
      RegistryEntryCollection bundleEntries)
    {
      foreach (RegistryEntry bundleEntry in bundleEntries)
      {
        if (bundleEntry.Name.StartsWith("BundleToInvalidate-", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(bundleEntry.Value))
        {
          foreach (BundleDefinition bundleDefinition in BundlingHelper.GetBundleDefinitionsFromQueryString(requestContext, bundleEntry.Value))
            this.m_invalidatedBundles[bundleDefinition.GetDefinitionHashCode()] = bundleEntry.Path;
        }
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.OnSettingsChanged(requestContext);
    }

    private void OnStrongBoxChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.OnSettingsChanged(requestContext);
    }

    private void OnSettingsChanged(IVssRequestContext requestContext)
    {
      this.Clear(requestContext);
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.ConfigureCdnBlobProvider(requestContext.Elevate());
    }

    internal class HttpContextInfoForCi
    {
      public Uri Url { get; set; }

      public string Controller { get; set; }

      public string Action { get; set; }
    }
  }
}
