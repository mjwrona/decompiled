// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationApiConstants
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [GenerateAllConstants(null)]
  public static class NotificationApiConstants
  {
    public const string AreaId = "1790FF0F-C188-4554-8DE8-26A4CEB751A2";
    internal const string InstanceType = "00000003-0000-8888-8000-000000000000";
    public const string AreaName = "notification";

    [GenerateAllConstants(null)]
    public static class EventTypesResource
    {
      public static readonly Guid LocationId = new Guid("cc84fb5f-6247-4c7a-aeae-e5a3c3fddb21");
      public const string Name = "EventTypes";
    }

    [GenerateAllConstants(null)]
    public static class EventTypesFieldsResource
    {
      public static readonly Guid LocationId = new Guid("b5bbdd21-c178-4398-b6db-0166d910028a");
      public const string LocationIdString = "b5bbdd21-c178-4398-b6db-0166d910028a";
      public const string Name = "EventTypeFieldValuesQuery";
    }

    [GenerateAllConstants(null)]
    public static class SubscriptionTemplatesResource
    {
      public static readonly Guid LocationId = new Guid("fa5d24ba-7484-4f3d-888d-4ec6b1974082");
      public const string Name = "SubscriptionTemplates";
    }

    [GenerateAllConstants(null)]
    public static class SubscriptionsResource
    {
      public const string LocationIdString = "70F911D6-ABAC-488C-85B3-A206BF57E165";
      public static readonly Guid LocationId = new Guid("70F911D6-ABAC-488C-85B3-A206BF57E165");
      public const string Name = "Subscriptions";
    }

    [GenerateAllConstants(null)]
    public static class UserSettingsResource
    {
      public const string LocationIdString = "ed5a3dff-aeb5-41b1-b4f7-89e66e58b62e";
      public static readonly Guid LocationId = new Guid("ed5a3dff-aeb5-41b1-b4f7-89e66e58b62e");
      public const string Name = "UserSettings";
    }

    [GenerateAllConstants(null)]
    public static class SubscriptionsQueryResource
    {
      public static readonly Guid LocationId = new Guid("6864DB85-08C0-4006-8E8E-CC1BEBE31675");
      public const string Name = "SubscriptionQuery";
    }

    [GenerateAllConstants(null)]
    public static class EventsResource
    {
      public static readonly Guid LocationId = new Guid("14C57B7A-C0E6-4555-9F51-E067188FDD8E");
      public const string Name = "Events";
    }

    [GenerateAllConstants(null)]
    public static class BatchNotificationOperationsResource
    {
      public static readonly Guid LocationId = new Guid("8F3C6AB2-5BAE-4537-B16E-F84E0955599E");
      public const string Name = "BatchNotificationOperations";
    }

    [GenerateAllConstants(null)]
    public static class StatisticsQueryResource
    {
      public static readonly Guid LocationId = new Guid("77878ce9-6391-49af-aa9d-768ac784461f");
      public const string Name = "StatisticsQuery";
    }

    [GenerateAllConstants(null)]
    public static class SubscribersResource
    {
      public static readonly Guid LocationId = new Guid("4d5caff1-25ba-430b-b808-7a1f352cc197");
      public const string Name = "Subscribers";
    }

    [GenerateAllConstants(null)]
    public static class SettingsResource
    {
      public static readonly Guid LocationId = new Guid("cbe076d8-2803-45ff-8d8d-44653686ea2a");
      public const string Name = "Settings";
    }

    [GenerateAllConstants(null)]
    public static class NotificationReasonsResource
    {
      public static readonly Guid LocationId = new Guid("19824fa9-1c76-40e6-9cce-cf0b9ca1cb60");
      public const string Name = "NotificationReasons";
    }

    [GenerateAllConstants(null)]
    public static class SubscriptionDiagnosticsResource
    {
      public static readonly Guid LocationId = new Guid("20f1929d-4be7-4c2e-a74e-d47640ff3418");
      public const string Name = "Diagnostics";
    }

    [GenerateAllConstants(null)]
    public static class NotificationDiagnosticLogsResource
    {
      public static readonly Guid LocationId = new Guid("991842f3-eb16-4aea-ac81-81353ef2b75c");
      public const string Name = "DiagnosticLogs";
    }

    [GenerateAllConstants(null)]
    public static class EventTransformsResource
    {
      public static readonly Guid LocationId = new Guid("9463a800-1b44-450e-9083-f948ea174b45");
      public const string Name = "EventTransforms";
    }

    [GenerateAllConstants(null)]
    public static class TokenNotificationEventResource
    {
      public static readonly Guid LocationId = new Guid("31dc86a2-67e8-4452-99a4-2b301ba28291");
      public const string Name = "TokenNotificationEvent";
    }
  }
}
