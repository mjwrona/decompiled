// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.NotificationEventId
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class NotificationEventId
  {
    public static readonly int NotificationExceptionBaseEventId = 60000;
    public static readonly int NotificationBacklogStatusUnhealthyException = NotificationEventId.NotificationExceptionBaseEventId + 1;
    public static readonly int EventBacklogStatusUnhealthyException = NotificationEventId.NotificationExceptionBaseEventId + 2;
  }
}
