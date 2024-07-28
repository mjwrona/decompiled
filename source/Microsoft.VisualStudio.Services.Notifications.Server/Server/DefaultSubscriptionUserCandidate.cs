// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DefaultSubscriptionUserCandidate
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [DataContract]
  public class DefaultSubscriptionUserCandidate
  {
    [DataMember]
    public Guid SubscriberId { get; set; }

    [DataMember]
    public string SubscriptionName { get; set; }

    public override bool Equals(object obj)
    {
      bool flag = base.Equals(obj);
      DefaultSubscriptionUserCandidate subscriptionUserCandidate = obj as DefaultSubscriptionUserCandidate;
      if (!flag && subscriptionUserCandidate != null && this.SubscriberId.Equals(subscriptionUserCandidate.SubscriberId) && this.SubscriptionName.Equals(subscriptionUserCandidate.SubscriptionName))
        flag = true;
      return flag;
    }

    public override int GetHashCode() => 0 + this.SubscriberId.GetHashCode() + this.SubscriptionName?.GetHashCode().Value;
  }
}
