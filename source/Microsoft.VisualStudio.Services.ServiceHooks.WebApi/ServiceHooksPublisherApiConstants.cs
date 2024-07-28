// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksPublisherApiConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [GenerateAllConstants(null)]
  public static class ServiceHooksPublisherApiConstants
  {
    public const string AllEvents = "*";
    public const string AreaName = "hooks";
    public static readonly Guid ConsumersLocationId = new Guid("4301C514-5F34-4F5D-A145-F0EA7B5B7D19");
    public static readonly Guid ConsumerActionsLocationId = new Guid("C3428E90-7A69-4194-8ED8-0F153185EE0D");
    public static readonly Guid InputValuesQueryLocationId = new Guid("140ED26D-ED51-4583-A1BD-0DD3FDD708BD");
    public static readonly Guid NotificationsLocationId = new Guid("0C62D343-21B0-4732-997B-017FDE84DC28");
    public static readonly Guid NotificationsQueryLocationId = new Guid("1A57562F-160A-4B5C-9185-905E95B39D36");
    public static readonly Guid PublisherEventTypesLocationId = new Guid("DB4777CD-8E08-4A84-8BA3-C974EA033718");
    public static readonly Guid PublishersLocationId = new Guid("1E83A210-5B53-43BC-90F0-D476A4E5D731");
    public static readonly Guid PublishersQueryLocationId = new Guid("99B44A8A-65A8-4670-8F3E-E7F7842CCE64");
    public static readonly Guid PublisherInputValuesQueryLocationId = new Guid("D815D352-A566-4DC1-A3E3-FD245ACF688C");
    public static readonly Guid SubscriptionsLocationId = new Guid("FC50D02A-849F-41FB-8AF1-0A5216103269");
    public static readonly Guid SubscriptionsQueryLocationId = new Guid("C7C3C1CF-9E05-4C0D-A425-A0F922C2C6ED");
    public static readonly Guid TestNotificationsLocationId = new Guid("1139462C-7E27-4524-A997-31B9B73551FE");
    public static readonly Guid ExternalEventsLocationId = new Guid("E0E0A1C9-BEEB-4FB7-A8C8-B18E3161A50E");
    public static readonly Guid SubscriptionDiagnosticsNotificationTracingLocationId = new Guid("3B36BCB5-02AD-43C6-BBFA-6DFC6F8E9D68");
  }
}
