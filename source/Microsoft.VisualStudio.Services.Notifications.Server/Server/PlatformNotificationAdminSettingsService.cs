// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.PlatformNotificationAdminSettingsService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class PlatformNotificationAdminSettingsService : NotificationAdminSettingsServiceBase
  {
    internal PlatformNotificationAdminSettingsService.NotifyPlatformSettingsChangeHandler PerformNotifySettingsChange = PlatformNotificationAdminSettingsService.\u003C\u003EO.\u003C0\u003E__NotifySettingsChange ?? (PlatformNotificationAdminSettingsService.\u003C\u003EO.\u003C0\u003E__NotifySettingsChange = new PlatformNotificationAdminSettingsService.NotifyPlatformSettingsChangeHandler(PlatformNotificationAdminSettingsService.NotifySettingsChange));

    protected override void ValidateEnvironment(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !(requestContext.ServiceInstanceType() == ServiceInstanceTypes.TFS))
        throw new InvalidOperationException("PlatformNotificationAdminSettingsService can only be instantiated on TFS");
    }

    protected override NotificationAdminSettings GetCollectionSettings(
      IVssRequestContext requestContext)
    {
      NotificationAdminSettings collectionSettings = new NotificationAdminSettings()
      {
        DefaultGroupDeliveryPreference = DefaultGroupDeliveryPreference.EachMember
      };
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      string str = vssRequestContext.GetService<ISettingsService>().GetValue<string>(vssRequestContext, SettingsUserScope.AllUsers, "NotificationDefaultGroupDeliveryPreference");
      if (!string.IsNullOrEmpty(str))
      {
        DefaultGroupDeliveryPreference result;
        if (!Enum.TryParse<DefaultGroupDeliveryPreference>(str, true, out result))
        {
          requestContext.Trace(1002150, TraceLevel.Warning, "Notifications", "AdminSettingsService", "Unsupported DefaultGroupDeliveryPreference string '" + str + "'");
          result = DefaultGroupDeliveryPreference.NoDelivery;
        }
        collectionSettings.DefaultGroupDeliveryPreference = result;
      }
      return collectionSettings;
    }

    protected override NotificationAdminSettings UpdateCollectionSettings(
      IVssRequestContext requestContext,
      NotificationAdminSettingsUpdateParameters updateParameters)
    {
      NotificationAdminSettings originalSettings = NotificationSubscriptionSecurityUtils.CallerHasAdminPermissions(requestContext, 2) ? this.GetCollectionSettings(requestContext) : throw new UnauthorizedAccessException(CoreRes.UnauthorizedAdminSettings());
      DefaultGroupDeliveryPreference? deliveryPreference = updateParameters.DefaultGroupDeliveryPreference;
      if (deliveryPreference.HasValue)
      {
        deliveryPreference = updateParameters.DefaultGroupDeliveryPreference;
        this.ValidateGroupDeliveryPreference(deliveryPreference.Value);
        ISettingsService service = requestContext.GetService<ISettingsService>();
        IVssRequestContext requestContext1 = requestContext;
        SettingsUserScope allUsers = SettingsUserScope.AllUsers;
        deliveryPreference = updateParameters.DefaultGroupDeliveryPreference;
        // ISSUE: variable of a boxed type
        __Boxed<DefaultGroupDeliveryPreference> local = (Enum) deliveryPreference.Value;
        service.SetValue(requestContext1, allUsers, "NotificationDefaultGroupDeliveryPreference", (object) local);
      }
      NotificationAdminSettings collectionSettings = this.GetCollectionSettings(requestContext);
      if (!collectionSettings.Equals((object) originalSettings))
      {
        this.PerformNotifySettingsChange(requestContext);
        NotificationAuditing.PublishUpdateAdminSettingsEvent(requestContext, originalSettings, collectionSettings);
      }
      return collectionSettings;
    }

    private void ValidateGroupDeliveryPreference(DefaultGroupDeliveryPreference deliveryPreference)
    {
      if (deliveryPreference != DefaultGroupDeliveryPreference.NoDelivery && deliveryPreference != DefaultGroupDeliveryPreference.EachMember)
        throw new UnsupportedDeliveryPreference(CoreRes.UnsupportedUpdateDeliveryPreference());
    }

    private static void NotifySettingsChange(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().SetValue<Guid>(requestContext, NotificationFrameworkConstants.AdminSettingsServiceRoot + "/SettingsGeneration", Guid.NewGuid());

    internal void TestSetup() => this.PerformNotifySettingsChange = new PlatformNotificationAdminSettingsService.NotifyPlatformSettingsChangeHandler(this.NotifySettingsChangeForTesting);

    private void NotifySettingsChangeForTesting(IVssRequestContext requestContext) => this.NotifyListeners(requestContext);

    internal delegate void NotifyPlatformSettingsChangeHandler(IVssRequestContext requestContext);
  }
}
