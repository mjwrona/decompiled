// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionChannelMapping
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  internal static class SubscriptionChannelMapping
  {
    internal static Dictionary<string, Type> SupportedChannels = new Dictionary<string, Type>()
    {
      {
        "User",
        typeof (UserSubscriptionChannel)
      },
      {
        "EmailHtml",
        typeof (EmailHtmlSubscriptionChannel)
      },
      {
        "EmailPlaintext",
        typeof (EmailPlaintextSubscriptionChannel)
      },
      {
        "Soap",
        typeof (SoapSubscriptionChannel)
      },
      {
        "MessageQueue",
        typeof (MessageQueueSubscriptionChannel)
      },
      {
        "ServiceHooks",
        typeof (ServiceHooksSubscriptionChannel)
      },
      {
        "Block",
        typeof (BlockSubscriptionChannel)
      },
      {
        "UserSystem",
        typeof (UserSystemSubscriptionChannel)
      },
      {
        "ServiceBus",
        typeof (ServiceBusSubscriptionChannel)
      },
      {
        "Group",
        typeof (GroupSubscriptionChannel)
      }
    };
  }
}
