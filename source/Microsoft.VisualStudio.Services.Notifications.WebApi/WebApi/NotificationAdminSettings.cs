// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationAdminSettings
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public class NotificationAdminSettings
  {
    [DataMember]
    public DefaultGroupDeliveryPreference DefaultGroupDeliveryPreference { get; set; }

    public override bool Equals(object obj) => obj is NotificationAdminSettings notificationAdminSettings && notificationAdminSettings.DefaultGroupDeliveryPreference == this.DefaultGroupDeliveryPreference;

    public override int GetHashCode() => (int) this.DefaultGroupDeliveryPreference;

    public static NotificationSubscriberDeliveryPreference TranslateDefaultGroupDeliveryPreference(
      DefaultGroupDeliveryPreference defaultGroupDeliveryPreference)
    {
      if (defaultGroupDeliveryPreference == DefaultGroupDeliveryPreference.NoDelivery)
        return NotificationSubscriberDeliveryPreference.NoDelivery;
      if (defaultGroupDeliveryPreference == DefaultGroupDeliveryPreference.EachMember)
        return NotificationSubscriberDeliveryPreference.EachMember;
      throw new ArgumentOutOfRangeException(nameof (defaultGroupDeliveryPreference));
    }
  }
}
