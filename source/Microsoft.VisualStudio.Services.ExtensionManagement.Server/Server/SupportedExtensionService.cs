// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.SupportedExtensionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class SupportedExtensionService : 
    VssMemoryCacheService<string, IDictionary<string, SupportedExtension>>,
    ISupportedExtensionService,
    IVssFrameworkService
  {
    private int m_loaded = -1;
    private int m_current;
    private const string s_area = "SupportedExtensionService";
    private const string s_layer = "Service";
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(60.0);
    private static readonly TimeSpan s_cleanupInterval = new TimeSpan(0, 5, 0);

    public SupportedExtensionService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, SupportedExtensionService.s_cleanupInterval)
    {
      this.InactivityInterval.Value = SupportedExtensionService.s_maxCacheInactivityAge;
    }

    internal void RegisterNotification(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService notificationService = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.GetService<ITeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      notificationService.RegisterNotification(requestContext, "Default", ExtensionManagementSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
      notificationService.RegisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      this.RegisterNotification(requestContext);
      base.ServiceStart(requestContext);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", ExtensionManagementSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), false);
      service.UnregisterNotification(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), false);
      base.ServiceEnd(requestContext);
    }

    public bool TryGetSupportedVersions(
      IVssRequestContext requestContext,
      string key,
      out IDictionary<string, SupportedExtension> versions)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_loaded == this.m_current)
        return this.TryGetValue(requestContext1, key, out versions);
      versions = (IDictionary<string, SupportedExtension>) null;
      return false;
    }

    public virtual IDictionary<string, SupportedExtension> FetchSupportedVersions(
      IVssRequestContext requestContext,
      string versionCheckUri,
      out bool isCheckSuccessful)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      isCheckSuccessful = true;
      int current = this.m_current;
      IDictionary<string, SupportedExtension> dictionary;
      try
      {
        requestContext.Trace(10013141, TraceLevel.Info, nameof (SupportedExtensionService), "Service", "Fetching supported versions from : {0}", (object) versionCheckUri);
        List<SupportedExtension> source;
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          source = vssRequestContext.GetService<IVersionDiscoveryService>().LoadSupportedVersions(vssRequestContext);
        }
        else
        {
          Uri baseUrl = new Uri(versionCheckUri);
          source = new ExtensionVersionHttpClient(baseUrl).GetSupportedVersionsAsync(baseUrl.ToString(), (object) requestContext).SyncResult<List<SupportedExtension>>();
        }
        for (int index = 0; index < source.Count; ++index)
        {
          SupportedExtension supportedExtension = source[index];
          if (!Version.TryParse(supportedExtension.Version, out Version _))
          {
            requestContext.Trace(10013142, TraceLevel.Info, nameof (SupportedExtensionService), "Service", "Supported version {0} from: {1} is an invalid format", (object) supportedExtension.Version, (object) versionCheckUri);
            source.RemoveAt(index--);
          }
        }
        dictionary = (IDictionary<string, SupportedExtension>) source.ToDictionary<SupportedExtension, string>((Func<SupportedExtension, string>) (s => GalleryUtil.CreateFullyQualifiedName(s.Publisher, s.Extension)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (current == this.m_current)
        {
          this.Set(vssRequestContext, versionCheckUri, dictionary);
          this.m_loaded = current;
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(10013138, TraceLevel.Error, nameof (SupportedExtensionService), "Service", "Error received fetching supported versions from: {0}", (object) versionCheckUri);
        requestContext.TraceException(10013139, nameof (SupportedExtensionService), "Service", ex);
        dictionary = (IDictionary<string, SupportedExtension>) new Dictionary<string, SupportedExtension>();
        isCheckSuccessful = false;
      }
      return dictionary;
    }

    private void OnForceFlush(IVssRequestContext requestContext, Guid eventClass, string eventData)
    {
      requestContext.Trace(10013099, TraceLevel.Info, nameof (SupportedExtensionService), "Service", "SupportedExtensionService.OnForceFlush processed");
      this.Invalidate(requestContext);
    }

    private void OnPublishedExtensionChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      try
      {
        ExtensionChangeNotification changeNotification = TeamFoundationSerializationUtility.Deserialize<ExtensionChangeNotification>(eventData);
        requestContext.Trace(10013096, TraceLevel.Info, nameof (SupportedExtensionService), "Service", "SupportedExtensionService.OnPublishedExtensionChanged received notification Id: {0}.{1} EventType: {2}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName, (object) changeNotification.EventType);
        if (!((PublishedExtensionFlags) changeNotification.Flags).HasFlag((Enum) PublishedExtensionFlags.MultiVersion) || changeNotification.EventType != ExtensionEventType.ExtensionDeleted && changeNotification.EventType != ExtensionEventType.ExtensionCreated && changeNotification.EventType != ExtensionEventType.ExtensionDisabled && changeNotification.EventType != ExtensionEventType.ExtensionEnabled && changeNotification.EventType != ExtensionEventType.ExtensionUpdated)
          return;
        this.Invalidate(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013290, nameof (SupportedExtensionService), "Service", ex);
      }
    }

    private void Invalidate(IVssRequestContext requestContext)
    {
      Interlocked.Increment(ref this.m_current);
      this.Clear(requestContext);
    }
  }
}
