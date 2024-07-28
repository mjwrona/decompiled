// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.VersionDiscoveryService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class VersionDiscoveryService : IVssFrameworkService, IVersionDiscoveryService
  {
    private const string s_area = "VersionDiscoveryService";
    private const string s_layer = "Service";
    private List<SupportedExtension> m_supportedExtensions;
    private Dictionary<string, SupportedExtension> m_supportedVersionMap;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/Extensions/...");
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<CachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public List<SupportedExtension> LoadSupportedVersions(IVssRequestContext requestContext)
    {
      List<SupportedExtension> supportedExtensionList = this.m_supportedExtensions;
      if (supportedExtensionList == null)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        RegistryEntryCollection registryEntryCollection = vssRequestContext.GetService<CachedRegistryService>().ReadEntries(vssRequestContext, (RegistryQuery) string.Format("{0}/**", (object) "/Configuration/Extensions"));
        supportedExtensionList = new List<SupportedExtension>();
        foreach (RegistryEntry registryEntry in registryEntryCollection)
        {
          string[] strArray = registryEntry.Path.Split(ExtensionSdkServerConstants.RegistrySeparators, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length == 5 && strArray[4].Equals("Version", StringComparison.OrdinalIgnoreCase))
            supportedExtensionList.Add(new SupportedExtension()
            {
              Publisher = strArray[2],
              Extension = strArray[3],
              Version = registryEntry.Value
            });
        }
        this.m_supportedExtensions = supportedExtensionList;
      }
      return supportedExtensionList;
    }

    public Dictionary<string, SupportedExtension> GetSupportedVersionMap(
      IVssRequestContext requestContext)
    {
      Dictionary<string, SupportedExtension> supportedVersionMap = this.m_supportedVersionMap;
      if (supportedVersionMap == null)
      {
        List<SupportedExtension> source = this.LoadSupportedVersions(requestContext);
        supportedVersionMap = source == null ? new Dictionary<string, SupportedExtension>() : source.ToDictionary<SupportedExtension, string>((Func<SupportedExtension, string>) (x => GalleryUtil.CreateFullyQualifiedName(x.Publisher, x.Extension)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_supportedVersionMap = supportedVersionMap;
      }
      return supportedVersionMap;
    }

    public bool IsExtensionSupported(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      Dictionary<string, SupportedExtension> supportedVersionMap = this.GetSupportedVersionMap(requestContext);
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      SupportedExtension supportedExtension = (SupportedExtension) null;
      string key = fullyQualifiedName;
      ref SupportedExtension local = ref supportedExtension;
      return supportedVersionMap.TryGetValue(key, out local);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.Trace(10013560, TraceLevel.Info, nameof (VersionDiscoveryService), "Service", "OnRegistryChanged:: Clearing version discovery cache");
      this.m_supportedExtensions = (List<SupportedExtension>) null;
      this.m_supportedVersionMap = (Dictionary<string, SupportedExtension>) null;
      this.LoadSupportedVersions(requestContext);
    }
  }
}
