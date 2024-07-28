// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notification.FrameworkPersistedNotificationTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Notification
{
  internal static class FrameworkPersistedNotificationTracePoints
  {
    public static int ServiceSaveNotificationsEnter = 110000;
    public static int ServiceSaveNotificationsException = 110001;
    public static int ServiceSaveNotificationsLeave = 110002;
    public static int ServiceGetRecipientNotificationsEnter = 110003;
    public static int ServiceGetRecipientNotificationsException = 110004;
    public static int ServiceGetRecipientNotificationsLeave = 110005;
    public static int ServiceGetRecipientMetadataEnter = 110006;
    public static int ServiceGetRecipientMetadataException = 110007;
    public static int ServiceGetRecipientMetadataLeave = 110008;
    public static int ServiceUpdateRecipientMetadataEnter = 110009;
    public static int ServiceUpdateRecipientMetadataException = 110010;
    public static int ServiceUpdateRecipientMetadataLeave = 110011;
    public static readonly string Area = "Notification";
  }
}
