// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageBusSubscriptionInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MessageBusSubscriptionInfo
  {
    public string MessageBusName { get; set; }

    public string SubscriptionName { get; set; }

    internal string Namespace { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public TimeSpan? AutoDeleteOnIdle { get; set; }

    public int MaxDeliveryCount { get; set; }

    public string NamespacePoolName { get; set; }

    public MessageBusSubscriptionInfo()
    {
    }

    public MessageBusSubscriptionInfo(MessageBusSubscriptionInfo subscriptionInfoToClone)
    {
      this.AutoDeleteOnIdle = subscriptionInfoToClone.AutoDeleteOnIdle;
      this.CreatedAt = subscriptionInfoToClone.CreatedAt;
      this.LastAccessedAt = subscriptionInfoToClone.LastAccessedAt;
      this.MaxDeliveryCount = subscriptionInfoToClone.MaxDeliveryCount;
      this.MessageBusName = subscriptionInfoToClone.MessageBusName;
      this.Namespace = subscriptionInfoToClone.Namespace;
      this.SubscriptionName = subscriptionInfoToClone.SubscriptionName;
      this.NamespacePoolName = subscriptionInfoToClone.NamespacePoolName;
    }

    public override bool Equals(object obj) => obj is MessageBusSubscriptionInfo subscriptionInfo && VssStringComparer.MessageBusName.Equals(this.MessageBusName, subscriptionInfo.MessageBusName) && VssStringComparer.MessageBusSubscriptionName.Equals(this.SubscriptionName, subscriptionInfo.SubscriptionName) && VssStringComparer.NamespaceName.Equals(this.Namespace, subscriptionInfo.Namespace);

    public override int GetHashCode() => this.MessageBusName != null ? VssStringComparer.MessageBusName.GetHashCode(this.MessageBusName) ^ (this.SubscriptionName == null ? 0 : VssStringComparer.MessageBusSubscriptionName.GetHashCode(this.SubscriptionName)) ^ (this.Namespace == null ? 0 : VssStringComparer.NamespaceName.GetHashCode(this.Namespace)) : 0;

    public override string ToString() => this.Namespace != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}::{1}::{2}", (object) this.Namespace, (object) this.MessageBusName, (object) this.SubscriptionName) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}::{1}", (object) this.MessageBusName, (object) this.SubscriptionName);
  }
}
