// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.MinimalApiVersionFor
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs
{
  internal static class MinimalApiVersionFor
  {
    public static ApiVersion BaiduSupport => ApiVersion.Eleven;

    public static ApiVersion AdmSupport => ApiVersion.Eight;

    public static ApiVersion NamespacesWithoutACS => ApiVersion.Nine;

    public static ApiVersion InstallationApi => ApiVersion.Nine;

    public static ApiVersion DisableNotificationHub => ApiVersion.Ten;

    public static ApiVersion PropertyBagSupport => ApiVersion.Thirteen;

    public static ApiVersion MessagingPremiumSKU => ApiVersion.Fourteen;
  }
}
