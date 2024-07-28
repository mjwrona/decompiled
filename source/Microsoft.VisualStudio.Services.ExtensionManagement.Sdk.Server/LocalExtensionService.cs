// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.LocalExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal abstract class LocalExtensionService : ILocalExtensionsService, IVssFrameworkService
  {
    private static readonly string s_area = "BaseLocalExtensionsService";
    private static readonly string s_layer = "IVssFrameworkService";
    private ILockName m_extensionRefreshLockName;
    private bool m_extensionRefreshTaskQueued;
    private int m_extensionRefreshTaskDelay;
    private int m_refreshAttempts;
    private const int c_extensionRefreshTaskDelayInSeconds = 60;
    private int m_loaded;
    private int m_current;
    private Dictionary<string, LocalExtension> m_localExtensions;

    public virtual void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      this.m_extensionRefreshLockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) "extensionrefresh"));
      this.m_localExtensions = (Dictionary<string, LocalExtension>) null;
      this.m_current = 0;
      this.m_loaded = -1;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/LocalContributionProviders/**");
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsRegistryChanged), "/Service/LocalExtensions/Settings/ExtensionRefreshDelay");
      requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false, false);
      this.LoadConfiguration(requestContext);
    }

    public virtual void ServiceEnd(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      service.UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsRegistryChanged));
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
    }

    public abstract Dictionary<string, LocalExtension> GetExtensions(
      IVssRequestContext requestContext);

    internal abstract Dictionary<string, LocalExtension> RefreshExtensions(
      IVssRequestContext requestContext);

    internal virtual void QueueExtensionRefresh(IVssRequestContext requestContext, bool immediate)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.RefreshOnUserRequest"))
      {
        this.RefreshExtensions(requestContext);
      }
      else
      {
        ITeamFoundationTaskService service = requestContext.GetService<ITeamFoundationTaskService>();
        using (requestContext.Lock(this.m_extensionRefreshLockName))
        {
          if (!this.m_extensionRefreshTaskQueued)
          {
            DateTime startTime = DateTime.UtcNow;
            if (!immediate)
              startTime = DateTime.UtcNow + TimeSpan.FromSeconds((double) this.m_extensionRefreshTaskDelay * Math.Pow(2.0, (double) Math.Min(this.m_refreshAttempts, 6)));
            ++this.m_refreshAttempts;
            requestContext.Trace(10013335, TraceLevel.Info, LocalExtensionService.s_area, LocalExtensionService.s_layer, "Queueing extension refresh. Attempt: {0}", (object) this.m_refreshAttempts);
            service.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshExtensionTask), (object) null, startTime, 0));
            this.m_extensionRefreshTaskQueued = true;
          }
          else
            requestContext.Trace(10013336, TraceLevel.Info, LocalExtensionService.s_area, LocalExtensionService.s_layer, "Extension refresh already queued.");
        }
      }
    }

    private void RefreshExtensionTask(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.Trace(10013337, TraceLevel.Info, LocalExtensionService.s_area, LocalExtensionService.s_layer, "Processing extension refresh.");
      this.m_extensionRefreshTaskQueued = false;
      this.RefreshExtensions(requestContext);
    }

    internal virtual void LoadProviders(
      IVssRequestContext requestContext,
      string query,
      Dictionary<string, LocalExtensionProvider> localProviders)
    {
      foreach (RegistryEntry readEntry in requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, new RegistryQuery(query)))
      {
        string[] strArray = readEntry.Path.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length >= 4)
        {
          string key = strArray[2];
          LocalExtensionProvider extensionProvider;
          if (!localProviders.TryGetValue(key, out extensionProvider))
          {
            extensionProvider = new LocalExtensionProvider();
            extensionProvider.ProviderName = strArray[2];
            extensionProvider.DisplayName = readEntry.Value;
            localProviders[key] = extensionProvider;
          }
        }
      }
    }

    private void LoadConfiguration(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery query = new RegistryQuery("/Service/LocalExtensions/Settings/ExtensionRefreshDelay");
      this.m_extensionRefreshTaskDelay = service.GetValue<int>(requestContext, in query, 60);
    }

    internal bool IsDataCurrent() => this.CurrentDataVersion == this.LoadedDataVersion;

    internal void InvalidateCache() => Interlocked.Increment(ref this.m_current);

    internal void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.InvalidateCache();
    }

    private void OnForceFlush(IVssRequestContext requestContext, Guid eventClass, string eventData) => this.InvalidateCache();

    internal void OnSettingsRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadConfiguration(requestContext);
    }

    internal void ResetRefreshAttempts() => this.m_refreshAttempts = 0;

    internal int CurrentDataVersion
    {
      get => this.m_current;
      set => this.m_current = value;
    }

    internal int LoadedDataVersion
    {
      get => this.m_loaded;
      set => this.m_loaded = value;
    }

    internal Dictionary<string, LocalExtension> LocalExtensions
    {
      get => this.m_localExtensions;
      set => this.m_localExtensions = value;
    }
  }
}
