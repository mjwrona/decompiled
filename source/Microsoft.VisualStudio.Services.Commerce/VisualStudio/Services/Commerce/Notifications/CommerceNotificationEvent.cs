// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Notifications.CommerceNotificationEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce.Notifications
{
  [DataContract]
  public abstract class CommerceNotificationEvent
  {
    [DataMember]
    public string NotificationType { get; set; }

    [DataMember]
    public string ExtensionName { get; set; }

    [DataMember]
    public string AccountName { get; set; }

    [DataMember]
    public string AccountLink { get; set; }

    [DataMember]
    public string ManageExtensionUsersLink { get; set; }

    [DataMember]
    public string SubscriptionName { get; set; }
  }
}
