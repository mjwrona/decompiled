// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FrameworkNotificationAdminSettingsService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.Notifications.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class FrameworkNotificationAdminSettingsService : NotificationAdminSettingsServiceBase
  {
    private FrameworkNotificationAdminSettingsService.GetSettingsHandler CallGetSettings = FrameworkNotificationAdminSettingsService.\u003C\u003EO.\u003C0\u003E__RemoteGetSettings ?? (FrameworkNotificationAdminSettingsService.\u003C\u003EO.\u003C0\u003E__RemoteGetSettings = new FrameworkNotificationAdminSettingsService.GetSettingsHandler(FrameworkNotificationAdminSettingsService.RemoteGetSettings));
    private FrameworkNotificationAdminSettingsService.UpdateSettingsHandler CallUpdateSettings = FrameworkNotificationAdminSettingsService.\u003C\u003EO.\u003C1\u003E__RemoteUpdateSettings ?? (FrameworkNotificationAdminSettingsService.\u003C\u003EO.\u003C1\u003E__RemoteUpdateSettings = new FrameworkNotificationAdminSettingsService.UpdateSettingsHandler(FrameworkNotificationAdminSettingsService.RemoteUpdateSettings));
    private FrameworkNotificationAdminSettingsService.NotifyFrameworkSettingsChangeHandler PerformNotifySettingsChange = FrameworkNotificationAdminSettingsService.\u003C\u003EO.\u003C2\u003E__NotifySettingsChange ?? (FrameworkNotificationAdminSettingsService.\u003C\u003EO.\u003C2\u003E__NotifySettingsChange = new FrameworkNotificationAdminSettingsService.NotifyFrameworkSettingsChangeHandler(FrameworkNotificationAdminSettingsService.NotifySettingsChange));
    private NotificationAdminSettings m_cachedSettings;
    private DateTime m_nextUpdateTime = DateTime.MinValue;
    private Guid m_settingsGeneration = Guid.NewGuid();
    protected object m_lock = new object();
    private int m_cacheLifetime = 300;
    private static readonly string s_settingsGeneration = NotificationFrameworkConstants.AdminSettingsServiceRoot + "/" + NotificationFrameworkConstants.AdminSettingsGeneration;

    protected override void ValidateEnvironment(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceInstanceType() == ServiceInstanceTypes.TFS)
        throw new InvalidOperationException("FrameworkNotificationAdminSettingsService can only be instantiated on non-TFS services in a hosted deployment");
    }

    protected override NotificationAdminSettings GetCollectionSettings(
      IVssRequestContext requestContext)
    {
      NotificationAdminSettings collectionSettings = (NotificationAdminSettings) null;
      Guid guid = requestContext.GetService<IVssRegistryService>().GetValue<Guid>(requestContext, (RegistryQuery) FrameworkNotificationAdminSettingsService.s_settingsGeneration, this.m_settingsGeneration);
      lock (this.m_lock)
      {
        if (guid.Equals(this.m_settingsGeneration))
        {
          if (this.m_nextUpdateTime >= DateTime.UtcNow)
            collectionSettings = this.m_cachedSettings;
        }
      }
      if (collectionSettings == null)
      {
        collectionSettings = this.CallGetSettings(requestContext);
        bool flag;
        lock (this.m_lock)
        {
          this.m_nextUpdateTime = DateTime.UtcNow.AddSeconds((double) this.m_cacheLifetime);
          flag = this.m_cachedSettings == null || !this.m_cachedSettings.Equals((object) collectionSettings);
          this.m_cachedSettings = collectionSettings;
          this.m_settingsGeneration = guid;
        }
        if (flag)
          this.NotifyListeners(requestContext);
      }
      return collectionSettings;
    }

    protected override NotificationAdminSettings UpdateCollectionSettings(
      IVssRequestContext requestContext,
      NotificationAdminSettingsUpdateParameters updateParameters)
    {
      NotificationAdminSettings notificationAdminSettings = this.CallUpdateSettings(requestContext, updateParameters);
      bool flag;
      Guid settingsGeneration;
      lock (this.m_lock)
      {
        flag = !this.m_cachedSettings.Equals((object) notificationAdminSettings);
        this.m_cachedSettings = notificationAdminSettings;
        this.m_nextUpdateTime = DateTime.UtcNow.AddSeconds((double) this.m_cacheLifetime);
        if (flag)
          this.m_settingsGeneration = Guid.NewGuid();
        settingsGeneration = this.m_settingsGeneration;
      }
      if (flag)
        this.PerformNotifySettingsChange(requestContext, settingsGeneration);
      return notificationAdminSettings;
    }

    private static NotificationAdminSettings RemoteGetSettings(IVssRequestContext requestContext)
    {
      try
      {
        return requestContext.GetClient<NotificationHttpClient>(ServiceInstanceTypes.TFS).GetSettingsAsync(cancellationToken: requestContext.CancellationToken).SyncResult<NotificationAdminSettings>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002151, "Notifications", "AdminSettingsService", ex);
        throw new RemoteAdminSettingsException("Couldn't read remote admin settings", ex);
      }
    }

    private static NotificationAdminSettings RemoteUpdateSettings(
      IVssRequestContext requestContext,
      NotificationAdminSettingsUpdateParameters updateParameters)
    {
      try
      {
        return requestContext.GetClient<NotificationHttpClient>(ServiceInstanceTypes.TFS).UpdateSettingsAsync(updateParameters, cancellationToken: requestContext.CancellationToken).SyncResult<NotificationAdminSettings>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002151, "Notifications", "AdminSettingsService", ex);
        throw new RemoteAdminSettingsException("Couldn't update remote admin settings", ex);
      }
    }

    private static void NotifySettingsChange(
      IVssRequestContext requestContext,
      Guid settingsGeneration)
    {
      requestContext.GetService<IVssRegistryService>().SetValue<Guid>(requestContext, NotificationFrameworkConstants.AdminSettingsServiceRoot + "/SettingsGeneration", settingsGeneration);
    }

    internal void TestSetupHandlers(
      FrameworkNotificationAdminSettingsService.GetSettingsHandler getSettingsHandler = null,
      FrameworkNotificationAdminSettingsService.UpdateSettingsHandler updateSettingsHandler = null)
    {
      this.PerformNotifySettingsChange = new FrameworkNotificationAdminSettingsService.NotifyFrameworkSettingsChangeHandler(this.NotifySettingsChangeForTesting);
      this.CallGetSettings = getSettingsHandler == null ? (FrameworkNotificationAdminSettingsService.GetSettingsHandler) (context => new NotificationAdminSettings()
      {
        DefaultGroupDeliveryPreference = DefaultGroupDeliveryPreference.EachMember
      }) : getSettingsHandler;
      if (updateSettingsHandler != null)
        this.CallUpdateSettings = updateSettingsHandler;
      else
        this.CallUpdateSettings = (FrameworkNotificationAdminSettingsService.UpdateSettingsHandler) ((context, updateParams) => new NotificationAdminSettings()
        {
          DefaultGroupDeliveryPreference = updateParams.DefaultGroupDeliveryPreference.Value
        });
    }

    internal void TestSetupVars(
      NotificationAdminSettings adminSettings = null,
      DateTime? nextUpdateTime = null,
      Guid? generation = null)
    {
      if (adminSettings != null)
        this.m_cachedSettings = adminSettings;
      if (nextUpdateTime.HasValue)
        this.m_nextUpdateTime = nextUpdateTime.Value;
      if (!generation.HasValue)
        return;
      this.m_settingsGeneration = generation.Value;
    }

    private void NotifySettingsChangeForTesting(
      IVssRequestContext requestContext,
      Guid settingsGeneration)
    {
      this.NotifyListeners(requestContext);
    }

    internal delegate NotificationAdminSettings GetSettingsHandler(IVssRequestContext requestContext);

    internal delegate NotificationAdminSettings UpdateSettingsHandler(
      IVssRequestContext requestContext,
      NotificationAdminSettingsUpdateParameters updateParameters);

    private delegate void NotifyFrameworkSettingsChangeHandler(
      IVssRequestContext requestContext,
      Guid settingsGeneration);
  }
}
