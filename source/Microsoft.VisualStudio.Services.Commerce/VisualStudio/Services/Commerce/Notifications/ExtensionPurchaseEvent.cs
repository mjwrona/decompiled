// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Notifications.ExtensionPurchaseEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce.Notifications
{
  [DataContract]
  public class ExtensionPurchaseEvent : CommerceNotificationEvent
  {
    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string Quantity { get; set; }

    [DataMember]
    public string Price { get; set; }

    [DataMember]
    public string RequestorEmail { get; set; }

    [DataMember]
    public string RequestMessage { get; set; }

    [DataMember]
    public string BuyMoreLink { get; set; }

    [DataMember]
    public string AcceptRequestLink { get; set; }
  }
}
