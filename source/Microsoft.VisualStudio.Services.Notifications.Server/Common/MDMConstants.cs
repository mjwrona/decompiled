// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.MDMConstants
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class MDMConstants
  {
    public const string MDMNotificationJobScope = "NotificationJob";
    public const string EventProcessingDelayKPI = "EventProcessingDelayInMs";
    public const string EventProcessingDelayKPIDesc = "Time taken to start processing an event";
    public const string MDMDeliveryJobscope = "NotificationDeliveryJob";
    public const string DeliveryDelayKPI = "NotificationDeliveryDelayInMs";
    public const string DeliveryDelayWithRetriesKPI = "NotificationDeliveryDelayWithRetriesInMs";
    public const string TotalProcessingTimeKPI = "EventProcessingTimeInMs";
    public const string TotalProcessingTimeWithRetriesKPI = "EventProcessingTimeWithRetriesInMs";
    public const string DeliveryDelayKPIDesc = "Time taken to start deliverying a notification";
  }
}
