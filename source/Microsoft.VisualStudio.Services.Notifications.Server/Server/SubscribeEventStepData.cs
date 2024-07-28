// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscribeEventStepData
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.Common;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class SubscribeEventStepData
  {
    [XmlAttribute("userId")]
    public string UserId { get; set; }

    [XmlAttribute("eventType")]
    public string EventType { get; set; }

    [XmlAttribute("filter")]
    public string FilterExpression { get; set; }

    [XmlAttribute("tag")]
    public string Tag { get; set; }

    public DeliveryPreference Preference { get; set; }
  }
}
