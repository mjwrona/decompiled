// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionChannel
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  internal static class SubscriptionChannel
  {
    internal static ISubscriptionChannel Create(string channel, string address)
    {
      Type type;
      if (!SubscriptionChannelMapping.SupportedChannels.TryGetValue(channel, out type))
        throw new ArgumentException(NotificationsWebApiResources.UnsupportedChannel((object) channel));
      ISubscriptionChannel instance = (ISubscriptionChannel) Activator.CreateInstance(type, true);
      if (instance is SubscriptionChannelWithAddress channelWithAddress && !string.IsNullOrEmpty(address))
        channelWithAddress.Address = address;
      return instance;
    }

    internal static string GetAddress(ISubscriptionChannel subscriptionChannel)
    {
      if (!(subscriptionChannel is SubscriptionChannelWithAddress channelWithAddress))
        return (string) null;
      return !channelWithAddress.UseCustomAddress ? string.Empty : channelWithAddress.Address;
    }
  }
}
