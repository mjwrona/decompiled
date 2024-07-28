// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationDiagnosticLogConverter
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public class NotificationDiagnosticLogConverter : TypeStringLookupConverter
  {
    private static Dictionary<string, Type> s_supportedFilters = new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "EventProcessing",
        typeof (EventProcessingLog)
      },
      {
        "NotificationDelivery",
        typeof (NotificationDeliveryLog)
      },
      {
        "SubscriptionTraceEventProcessing",
        typeof (SubscriptionTraceEventProcessingLog)
      },
      {
        "SubscriptionTraceNotificationDelivery",
        typeof (SubscriptionTraceNotificationDeliveryLog)
      }
    };

    protected override Dictionary<string, Type> TypeMap => NotificationDiagnosticLogConverter.s_supportedFilters;

    protected override Type BaseType => typeof (INotificationDiagnosticLog);

    protected override string TypeFieldName => "logType";

    protected override object CreateUnsupportedTypeObject(string typeName) => (object) new InvalidDiagnosticLog();

    protected override bool LimitConvertToBaseType => true;
  }
}
