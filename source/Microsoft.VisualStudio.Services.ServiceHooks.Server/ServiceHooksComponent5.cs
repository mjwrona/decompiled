// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksComponent5
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class ServiceHooksComponent5 : ServiceHooksComponent4
  {
    protected int m_dataspaceId;

    public override Subscription CreateSubscription(Subscription subscription)
    {
      this.m_dataspaceId = this.GetDataspaceIdFromPublisherInputs(subscription.PublisherInputs);
      return base.CreateSubscription(subscription);
    }

    public override Subscription UpdateSubscription(
      Subscription subscription,
      bool updateProbationRetries,
      bool requestedByUser)
    {
      this.m_dataspaceId = this.GetDataspaceIdFromPublisherInputs(subscription.PublisherInputs);
      return base.UpdateSubscription(subscription, updateProbationRetries, requestedByUser);
    }

    public override void CreateNotification(
      Notification notification,
      bool allowFullContent,
      out int payloadLength)
    {
      this.m_dataspaceId = this.GetDataspaceIdFromPublisherInputs(notification.Details != null ? notification.Details.PublisherInputs : (IDictionary<string, string>) null);
      base.CreateNotification(notification, allowFullContent, out payloadLength);
    }

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure)
    {
      SqlCommand sqlCommand = base.PrepareStoredProcedure(storedProcedure);
      if (this.m_dataspaceId <= 0)
        return sqlCommand;
      this.BindInt("@dataspaceId", this.m_dataspaceId);
      return sqlCommand;
    }

    protected int GetDataspaceIdFromPublisherInputs(IDictionary<string, string> publisherInputs)
    {
      string g;
      return publisherInputs == null || !publisherInputs.TryGetValue("projectId", out g) ? this.GetDataspaceId(Guid.Empty) : this.GetDataspaceId(new Guid(g), true);
    }

    protected override SubscriptionBinder GetSubscriptionBinder() => (SubscriptionBinder) new SubscriptionBinder5();
  }
}
