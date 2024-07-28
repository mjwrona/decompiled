// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationAdminSettingsServiceBase
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal abstract class NotificationAdminSettingsServiceBase : 
    INotificationAdminSettingsServiceInternal,
    INotificationAdminSettingsService,
    IVssFrameworkService
  {
    protected const string s_area = "Notifications";
    protected const string s_layer = "AdminSettingsService";

    public virtual void ServiceStart(IVssRequestContext requestContext)
    {
      this.ValidateEnvironment(requestContext);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), NotificationFrameworkConstants.AdminSettingsServiceRoot + "/...");
    }

    public virtual void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.NotifyListeners(requestContext);
    }

    protected abstract void ValidateEnvironment(IVssRequestContext requestContext);

    protected NotificationAdminSettings GetSettingsDefault() => new NotificationAdminSettings()
    {
      DefaultGroupDeliveryPreference = DefaultGroupDeliveryPreference.EachMember
    };

    public NotificationAdminSettings GetSettings(IVssRequestContext requestContext)
    {
      NotificationAdminSettings notificationAdminSettings = (NotificationAdminSettings) null;
      if (requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          notificationAdminSettings = this.GetCollectionSettings(requestContext);
        else if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          requestContext.Trace(1002152, TraceLevel.Error, "Notifications", "Settings", string.Format("GetSettings: service host is not collection or deployment {0}", (object) requestContext.ServiceHost.HostType));
      }
      return notificationAdminSettings ?? this.GetSettingsDefault();
    }

    public NotificationAdminSettings UpdateSettings(
      IVssRequestContext requestContext,
      NotificationAdminSettingsUpdateParameters updateParameters)
    {
      NotificationAdminSettings notificationAdminSettings = (NotificationAdminSettings) null;
      if (requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          notificationAdminSettings = this.UpdateCollectionSettings(requestContext, updateParameters);
        else
          requestContext.Trace(1002152, TraceLevel.Error, "Notifications", "Settings", string.Format("UpdateSettings: service host is not collection {0}", (object) requestContext.ServiceHost.HostType));
      }
      else
        requestContext.Trace(1002153, TraceLevel.Error, "Notifications", "Settings", "UpdateSettings: service instance type is SPS.");
      return notificationAdminSettings ?? this.GetSettingsDefault();
    }

    protected abstract NotificationAdminSettings GetCollectionSettings(
      IVssRequestContext requestContext);

    protected abstract NotificationAdminSettings UpdateCollectionSettings(
      IVssRequestContext requestContext,
      NotificationAdminSettingsUpdateParameters updateParameters);

    public event SettingsChangedHandler SettingsChanged;

    protected void NotifyListeners(IVssRequestContext requestContext)
    {
      SettingsChangedHandler settingsChanged = this.SettingsChanged;
      if (settingsChanged == null)
        return;
      settingsChanged(requestContext);
    }
  }
}
