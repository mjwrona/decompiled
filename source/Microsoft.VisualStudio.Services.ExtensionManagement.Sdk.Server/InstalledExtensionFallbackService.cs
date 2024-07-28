// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.InstalledExtensionFallbackService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class InstalledExtensionFallbackService : 
    IInstalledExtensionFallbackService,
    IVssFrameworkService
  {
    private static readonly ExtensionFile[] s_emptyFileList = Array.Empty<ExtensionFile>();
    private static readonly string s_area = nameof (InstalledExtensionFallbackService);
    private static readonly string s_layer = "IVssFrameworkService";
    private int m_extensionRefreshTaskDelay = 60;
    private int m_refreshAttempts;
    private bool m_extensionRefreshTaskQueued;
    private ILockName m_extensionRefreshLockName;
    private int m_loadedDataVersion;
    private int m_currentDataVersion;
    private int m_refreshInProgress;
    private Dictionary<string, Dictionary<string, InstalledExtension>> m_fallbackExtensions;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.m_extensionRefreshLockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) "extensionrefresh"));
      this.m_fallbackExtensions = (Dictionary<string, Dictionary<string, InstalledExtension>>) null;
      requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/Extensions/...");
      this.RefreshExtensions(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<CachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public IEnumerable<Dictionary<string, InstalledExtension>> GetInstalledExtensions(
      IVssRequestContext requestContext)
    {
      if (this.m_fallbackExtensions == null)
        return (IEnumerable<Dictionary<string, InstalledExtension>>) this.RefreshExtensions(requestContext).Values;
      if (this.m_currentDataVersion == this.m_loadedDataVersion)
        return (IEnumerable<Dictionary<string, InstalledExtension>>) this.m_fallbackExtensions.Values;
      if (Interlocked.CompareExchange(ref this.m_refreshInProgress, 1, 0) == 0)
        this.QueueExtensionRefresh(requestContext);
      requestContext.Trace(10013700, TraceLevel.Info, InstalledExtensionFallbackService.s_area, InstalledExtensionFallbackService.s_layer, "Returning expired value since refresh is currently in progress");
      return (IEnumerable<Dictionary<string, InstalledExtension>>) this.m_fallbackExtensions.Values;
    }

    private Dictionary<string, Dictionary<string, InstalledExtension>> RefreshExtensions(
      IVssRequestContext requestContext)
    {
      try
      {
        int currentDataVersion = this.m_currentDataVersion;
        Dictionary<string, Dictionary<string, InstalledExtension>> extensionsByLanguage = new Dictionary<string, Dictionary<string, InstalledExtension>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (this.LoadExtensions(requestContext, extensionsByLanguage))
        {
          this.m_refreshAttempts = 0;
          if (currentDataVersion == this.m_currentDataVersion)
          {
            this.m_loadedDataVersion = this.m_currentDataVersion;
            this.m_fallbackExtensions = extensionsByLanguage;
          }
        }
        else
          this.QueueExtensionRefresh(requestContext);
        return extensionsByLanguage;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013705, TraceLevel.Error, InstalledExtensionFallbackService.s_area, InstalledExtensionFallbackService.s_layer, ex);
        throw;
      }
      finally
      {
        this.m_refreshInProgress = 0;
      }
    }

    private void QueueExtensionRefresh(IVssRequestContext requestContext)
    {
      ITeamFoundationTaskService service = requestContext.GetService<ITeamFoundationTaskService>();
      using (requestContext.Lock(this.m_extensionRefreshLockName))
      {
        if (!this.m_extensionRefreshTaskQueued)
        {
          DateTime startTime = DateTime.UtcNow + TimeSpan.FromSeconds((double) this.m_extensionRefreshTaskDelay * Math.Pow(2.0, (double) Math.Min(this.m_refreshAttempts, 6)));
          ++this.m_refreshAttempts;
          requestContext.Trace(10013710, TraceLevel.Info, InstalledExtensionFallbackService.s_area, InstalledExtensionFallbackService.s_layer, "Queueing extension refresh. Attempt: {0}", (object) this.m_refreshAttempts);
          service.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshExtensionTask), (object) null, startTime, 0));
          this.m_extensionRefreshTaskQueued = true;
        }
        else
          requestContext.Trace(10013715, TraceLevel.Info, InstalledExtensionFallbackService.s_area, InstalledExtensionFallbackService.s_layer, "Extension refresh already queued.");
      }
    }

    private bool LoadExtensions(
      IVssRequestContext requestContext,
      Dictionary<string, Dictionary<string, InstalledExtension>> extensionsByLanguage)
    {
      bool flag = true;
      int currentDataVersion = this.m_currentDataVersion;
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      CachedRegistryService service1 = context.GetService<CachedRegistryService>();
      ITeamFoundationFileService service2 = context.GetService<ITeamFoundationFileService>();
      IVssRequestContext requestContext1 = context;
      RegistryQuery query = (RegistryQuery) string.Format("{0}/**", (object) "/Configuration/Extensions");
      RegistryEntryCollection registryEntryCollection = service1.ReadEntries(requestContext1, query);
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        try
        {
          string[] strArray = registryEntry.Path.Split(ExtensionSdkServerConstants.RegistrySeparators, StringSplitOptions.RemoveEmptyEntries);
          string key1 = (string) null;
          int result = 0;
          if (strArray.Length == 5 && strArray[4].Equals("FileId", StringComparison.OrdinalIgnoreCase))
          {
            key1 = string.Empty;
            int.TryParse(registryEntry.Value, out result);
          }
          else if (strArray.Length == 6 && strArray[5].Equals("FileId", StringComparison.OrdinalIgnoreCase))
          {
            key1 = strArray[4];
            int.TryParse(registryEntry.Value, out result);
          }
          if (result != 0)
          {
            string key2 = string.Format("{0}.{1}", (object) strArray[2], (object) strArray[3]);
            string version = "999.999.999.999";
            string str = "999.999.999.999";
            string path = string.Format("{0}/{1}/{2}/Version", (object) "/Configuration/Extensions", (object) strArray[2], (object) strArray[3]);
            if (registryEntryCollection.ContainsPath(path))
              str = registryEntryCollection[path].Value;
            using (Stream manifestStream = service2.RetrieveFile(requestContext, (long) result, false, out byte[] _, out long _, out CompressionType _))
            {
              IDictionary<string, object> extensionProperties = (IDictionary<string, object>) ExtensionUtil.GetExtensionProperties(PublishedExtensionFlags.BuiltIn | PublishedExtensionFlags.Trusted | PublishedExtensionFlags.Public | PublishedExtensionFlags.MultiVersion);
              extensionProperties.Add("::Version", (object) str);
              ExtensionManifest extensionManifest = ExtensionUtil.LoadManifest(strArray[2], strArray[3], version, manifestStream, extensionProperties);
              InstalledExtension installedExtension1 = new InstalledExtension();
              installedExtension1.InstallState = new InstalledExtensionState()
              {
                Flags = ExtensionStateFlags.BuiltIn,
                LastUpdated = DateTime.UtcNow
              };
              installedExtension1.Flags = ExtensionFlags.BuiltIn | ExtensionFlags.Trusted;
              installedExtension1.Version = version;
              installedExtension1.PublisherName = strArray[2];
              installedExtension1.PublisherDisplayName = strArray[2];
              installedExtension1.ExtensionName = strArray[3];
              installedExtension1.ExtensionDisplayName = strArray[3];
              installedExtension1.BaseUri = extensionManifest.BaseUri;
              installedExtension1.Contributions = extensionManifest.Contributions;
              installedExtension1.ContributionTypes = extensionManifest.ContributionTypes;
              installedExtension1.EventCallbacks = extensionManifest.EventCallbacks;
              installedExtension1.ManifestVersion = extensionManifest.ManifestVersion;
              installedExtension1.Scopes = extensionManifest.Scopes;
              installedExtension1.ServiceInstanceType = extensionManifest.ServiceInstanceType;
              installedExtension1.LastPublished = DateTime.MinValue;
              installedExtension1.Files = (IEnumerable<ExtensionFile>) InstalledExtensionFallbackService.s_emptyFileList;
              installedExtension1.Demands = extensionManifest.Demands;
              InstalledExtension installedExtension2 = installedExtension1;
              Dictionary<string, InstalledExtension> dictionary;
              if (!extensionsByLanguage.TryGetValue(key2, out dictionary))
              {
                dictionary = new Dictionary<string, InstalledExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                extensionsByLanguage[key2] = dictionary;
              }
              dictionary[key1] = installedExtension2;
            }
          }
        }
        catch (Exception ex)
        {
          flag = false;
          requestContext.TraceException(10013720, InstalledExtensionFallbackService.s_area, InstalledExtensionFallbackService.s_layer, ex);
        }
      }
      return flag;
    }

    private void RefreshExtensionTask(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.Trace(10013725, TraceLevel.Info, InstalledExtensionFallbackService.s_area, InstalledExtensionFallbackService.s_layer, "Processing extension refresh.");
      this.m_extensionRefreshTaskQueued = false;
      this.RefreshExtensions(requestContext);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Interlocked.Increment(ref this.m_currentDataVersion);
    }
  }
}
