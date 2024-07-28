// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageBusHighAvailabilitySubscribeEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MessageBusHighAvailabilitySubscribeEntry
  {
    public string MessageBusName { get; set; }

    public string SubscriptionName { get; set; }

    public MessageBusHighAvailabilitySubscribeEntry(MessageBusSubscriptionInfo subscription)
    {
      this.MessageBusName = subscription.MessageBusName;
      this.SubscriptionName = subscription.SubscriptionName;
    }

    public override bool Equals(object obj) => obj is MessageBusHighAvailabilitySubscribeEntry availabilitySubscribeEntry && VssStringComparer.MessageBusName.Equals(this.MessageBusName, availabilitySubscribeEntry.MessageBusName) && VssStringComparer.MessageBusSubscriptionName.Equals(this.SubscriptionName, availabilitySubscribeEntry.SubscriptionName);

    public override int GetHashCode() => this.MessageBusName != null ? VssStringComparer.MessageBusName.GetHashCode(this.MessageBusName) ^ (this.SubscriptionName == null ? 0 : VssStringComparer.MessageBusSubscriptionName.GetHashCode(this.SubscriptionName)) : 0;
  }
}
