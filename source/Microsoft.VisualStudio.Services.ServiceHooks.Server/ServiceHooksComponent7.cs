// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksComponent7
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class ServiceHooksComponent7 : ServiceHooksComponent6
  {
    public override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      this.m_dataspaceId = this.GetDataspaceIdFromPublisherInputs(subscription.PublisherInputs);
      this.PrepareStoredProcedure("hooks.prc_CreateSubscription");
      this.BindGuid("@subscriptionId", subscription.Id);
      this.BindInt("@status", (int) (short) subscription.Status);
      this.BindString("@publisherId", subscription.PublisherId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@eventType", subscription.EventType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@resourceVersion", subscription.ResourceVersion, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@eventDescription", subscription.EventDescription, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerId", subscription.ConsumerId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerActionId", subscription.ConsumerActionId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@actionDescription", subscription.ActionDescription, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindByte("@probationRetries", subscription.ProbationRetries);
      this.BindGuid("@createdBy", Guid.Parse(subscription.CreatedBy.Id));
      this.BindInt("@scope", 0);
      this.BindSubscriptionInputsTable("@inputs", subscription);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.GetSubscriptionBinder());
      rc.AddBinder<SubscriptionInputValue>((ObjectBinder<SubscriptionInputValue>) new SubscriptionInputBinder());
      return ServiceHooksComponent.BindSubscriptionDeep(rc);
    }

    protected override SubscriptionBinder GetSubscriptionBinder() => (SubscriptionBinder) new SubscriptionBinder7();
  }
}
