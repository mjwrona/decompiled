// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTransformContext
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DataContract]
  public class NotificationTransformContext
  {
    public NotificationTransformContext() => this.SystemInputs = new Dictionary<string, string>();

    [DataMember]
    public string EventType { get; set; }

    [DataMember]
    public IFieldContainer EventFieldContainer { get; set; }

    [DataMember]
    public NotificationDeliveryDetails DeliveryDetails { get; set; }

    [DataMember]
    public string Channel { get; set; }

    [DataMember(Name = "System")]
    public Dictionary<string, string> SystemInputs { get; set; }
  }
}
