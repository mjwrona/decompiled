// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.DeliveryTypeChannelMapper
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class DeliveryTypeChannelMapper
  {
    public static string GetChannelName(DeliveryType deliveryType)
    {
      switch (deliveryType)
      {
        case DeliveryType.EmailHtml:
          return "EmailHtml";
        case DeliveryType.EmailPlaintext:
          return "EmailPlaintext";
        case DeliveryType.Soap:
          return "Soap";
        case DeliveryType.MessageQueue:
          return "MessageQueue";
        case DeliveryType.ServiceHooks:
          return "ServiceHooks";
        case DeliveryType.PersistedNotification:
          return "PersistedNotification";
        default:
          return string.Empty;
      }
    }

    public static DeliveryType GetDeliveryType(string channel)
    {
      if (channel != null)
      {
        switch (channel.Length)
        {
          case 4:
            switch (channel[0])
            {
              case 'S':
                if (channel == "Soap")
                  return DeliveryType.Soap;
                goto label_17;
              case 'U':
                if (channel == "User")
                  break;
                goto label_17;
              default:
                goto label_17;
            }
            break;
          case 9:
            if (channel == "EmailHtml")
              break;
            goto label_17;
          case 12:
            switch (channel[0])
            {
              case 'M':
                if (channel == "MessageQueue")
                  return DeliveryType.MessageQueue;
                goto label_17;
              case 'S':
                if (channel == "ServiceHooks")
                  return DeliveryType.ServiceHooks;
                goto label_17;
              default:
                goto label_17;
            }
          case 14:
            if (channel == "EmailPlaintext")
              return DeliveryType.EmailPlaintext;
            goto label_17;
          case 21:
            if (channel == "PersistedNotification")
              return DeliveryType.PersistedNotification;
            goto label_17;
          default:
            goto label_17;
        }
        return DeliveryType.EmailHtml;
      }
label_17:
      return (DeliveryType) 2147483647;
    }
  }
}
