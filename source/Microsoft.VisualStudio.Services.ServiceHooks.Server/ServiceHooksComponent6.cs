// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksComponent6
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class ServiceHooksComponent6 : ServiceHooksComponent5
  {
    public override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription UpdateSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      bool updateProbationRetries,
      bool requestedByUser)
    {
      this.m_dataspaceId = this.GetDataspaceIdFromPublisherInputs(subscription.PublisherInputs);
      this.PrepareStoredProcedure("hooks.prc_UpdateSubscription");
      this.BindGuid("@subscriptionId", subscription.Id);
      this.BindInt("@status", (int) (short) subscription.Status);
      this.BindString("@publisherId", subscription.PublisherId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@eventType", subscription.EventType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@resourceVersion", subscription.ResourceVersion, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@eventDescription", subscription.EventDescription, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerId", subscription.ConsumerId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerActionId", subscription.ConsumerActionId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@actionDescription", subscription.ActionDescription, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (!updateProbationRetries)
        this.BindNullValue("@probationRetries", SqlDbType.TinyInt);
      else
        this.BindByte("@probationRetries", subscription.ProbationRetries);
      this.BindGuid("@modifiedBy", Guid.Parse(subscription.ModifiedBy.Id));
      this.BindBoolean("@requestedByUser", requestedByUser);
      this.BindSubscriptionInputsTable("@inputs", subscription);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.GetSubscriptionBinder());
      rc.AddBinder<SubscriptionInputValue>((ObjectBinder<SubscriptionInputValue>) new SubscriptionInputBinder());
      return ServiceHooksComponent.BindSubscriptionDeep(rc);
    }

    public override void UpdateSubscriptionStatus(
      Guid subscriptionId,
      SubscriptionStatus status,
      Guid modifiedByIdentity,
      bool resetProbationRetries,
      bool incrementProbationRetries,
      bool requestedByUser)
    {
      this.PrepareStoredProcedure("hooks.prc_UpdateSubscriptionStatus");
      this.BindGuid("@subscriptionId", subscriptionId);
      this.BindInt("@status", (int) (short) status);
      this.BindBoolean("@resetProbationRetries", resetProbationRetries);
      this.BindBoolean("@incrementProbationRetries", incrementProbationRetries);
      this.BindGuid("@modifiedBy", modifiedByIdentity);
      this.BindBoolean("@requestedByUser", requestedByUser);
      this.ExecuteNonQuery();
    }
  }
}
