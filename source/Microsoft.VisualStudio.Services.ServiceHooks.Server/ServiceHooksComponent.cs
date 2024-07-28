// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksComponent
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class ServiceHooksComponent : 
    TeamFoundationSqlResourceComponent,
    IServiceHooksComponent,
    IDisposable
  {
    internal const int DetailsContentLengthMax = 64000;
    internal const int InputIdLengthMax = 260;
    internal const int EventTypeLengthMax = 260;
    internal const int ConsumerIdLengthMax = 260;
    internal const int PublisherIdLengthMax = 260;
    internal const int InputValueLengthMax = 1024;
    internal const int ConsumerActionIdLengthMax = 260;
    internal const int RequestLengthMax = 64000;
    internal const int ResponseLengthMax = 64000;
    internal const int ErrorDetailLengthMax = 2147483647;
    internal const int ErrorMessageLengthMax = 2147483647;
    internal const int EventContentLengthMax = 2147483647;
    internal const int ConsumerInputsLengthMax = 2147483647;
    internal const int PublisherInputsLengthMax = 2147483647;
    private static readonly JsonSerializerSettings s_serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent>(1),
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent2>(2),
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent3>(3),
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent4>(4),
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent5>(5),
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent6>(6),
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent7>(7),
      (IComponentCreator) new ComponentCreator<ServiceHooksComponent8>(8)
    }, "ServiceHooks", "ServiceHooks");
    private static readonly SqlMetaData[] hooks__typ_SubscriptionInput = new SqlMetaData[4]
    {
      new SqlMetaData("InputId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("InputValue", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Scope", SqlDbType.Int),
      new SqlMetaData("SubscriptionId", SqlDbType.UniqueIdentifier)
    };

    public ServiceHooksComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual IList<NotificationResultsSummaryDetail> QueryNotificationSummary(
      Guid subscriptionId,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate)
    {
      this.PrepareStoredProcedure("hooks.prc_QueryNotificationSummary");
      this.BindGuid("@subscriptionId", subscriptionId);
      if (!status.HasValue)
        this.BindNullValue("@status", SqlDbType.SmallInt);
      else
        this.BindInt("@status", (int) status.Value);
      if (!result.HasValue)
        this.BindNullValue("@result", SqlDbType.SmallInt);
      else
        this.BindInt("@result", (int) result.Value);
      if (!minDate.HasValue)
        this.BindNullValue("@minCreatedDate", SqlDbType.DateTime);
      else
        this.BindDateTime("@minCreatedDate", minDate.Value);
      if (!maxDate.HasValue)
        this.BindNullValue("@maxCreatedDate", SqlDbType.DateTime);
      else
        this.BindDateTime("@maxCreatedDate", maxDate.Value);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<NotificationResultsSummaryDetail>((ObjectBinder<NotificationResultsSummaryDetail>) new NotificationResultSummaryBinder());
      return (IList<NotificationResultsSummaryDetail>) resultCollection.GetCurrent<NotificationResultsSummaryDetail>().Items;
    }

    public virtual void CleanupNotificationDetails(int deleteOlderThanDays)
    {
      this.PrepareStoredProcedure("hooks.prc_CleanupNotificationDetails", 3600);
      this.BindDateTime("@deleteBeforeDate", DateTime.UtcNow.AddDays((double) (-1 * deleteOlderThanDays)));
      this.ExecuteNonQuery();
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification ReadNotification(
      Guid subscriptionId,
      int notificationId)
    {
      this.PrepareStoredProcedure("hooks.prc_ReadNotification");
      this.BindInt("@notificationId", notificationId);
      this.BindGuid("@subscriptionId", subscriptionId);
      return this.FinishReadNotification(true);
    }

    public virtual IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> QueryNotifications(
      Guid? subscriptionId,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults)
    {
      this.PrepareStoredProcedure("hooks.prc_QueryNotifications");
      if (!subscriptionId.HasValue)
        this.BindNullValue("@subscriptionId", SqlDbType.UniqueIdentifier);
      else
        this.BindGuid("@subscriptionId", subscriptionId.Value);
      return this.FinishQueryNotifications(status, result, minDate, maxDate, maxResults);
    }

    public virtual IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> QueryNotifications(
      IEnumerable<Guid> subscriptionIds,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults,
      int? maxResultsPerSubScription)
    {
      this.PrepareStoredProcedure("hooks.prc_QueryNotificationsBatch");
      this.BindGuidTable("@subscriptionIds", subscriptionIds);
      if (!maxResultsPerSubScription.HasValue)
        this.BindNullValue("@maxResultsPerSubScription", SqlDbType.Int);
      else
        this.BindInt("@maxResultsPerSubScription", maxResultsPerSubScription.Value);
      return this.FinishQueryNotifications(status, result, minDate, maxDate, maxResults);
    }

    public virtual IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> QueryNotificationsWithDetails(
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults)
    {
      this.PrepareStoredProcedure("hooks.prc_QueryNotificationsWithDetails");
      return this.FinishQueryNotifications(status, result, minDate, maxDate, maxResults, true);
    }

    public virtual void CreateNotification(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      bool allowFullContent,
      out int payloadLength)
    {
      this.PrepareStoredProcedure("hooks.prc_CreateNotification");
      this.BindGuid("@subscriptionId", notification.SubscriptionId);
      this.BindGuid("@eventId", notification.EventId);
      this.BindInt("@status", (int) notification.Status);
      this.BindInt("@result", (int) notification.Result);
      NotificationDetails notificationDetails = notification.Details ?? new NotificationDetails();
      string parameterValue = JsonConvert.SerializeObject((object) notificationDetails.Event, ServiceHooksComponent.s_serializerSettings);
      payloadLength = parameterValue.Length;
      if (parameterValue.Length > 64000 && !allowFullContent)
      {
        object resource = notificationDetails.Event.Resource;
        notificationDetails.Event.Resource = (object) ServiceHooksResources.Truncated;
        parameterValue = JsonConvert.SerializeObject((object) notificationDetails.Event, ServiceHooksComponent.s_serializerSettings);
        notificationDetails.Event.Resource = resource;
      }
      this.BindString("@eventType", notificationDetails.EventType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@eventContent", parameterValue, int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@publisherId", notificationDetails.PublisherId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerId", notificationDetails.ConsumerId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerActionId", notificationDetails.ConsumerActionId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerInputs", JsonConvert.SerializeObject((object) notificationDetails.ConsumerInputs, ServiceHooksComponent.s_serializerSettings), int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@publisherInputs", JsonConvert.SerializeObject((object) notificationDetails.PublisherInputs, ServiceHooksComponent.s_serializerSettings), int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@request", notificationDetails.Request, 64000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@response", notificationDetails.Request, 64000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@errorMessage", notificationDetails.ErrorMessage, int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@errorDetail", notificationDetails.ErrorDetail, int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableDateTime("@queuedDate", notificationDetails.QueuedDate);
      this.BindNullableDateTime("@dequeuedDate", notificationDetails.DequeuedDate);
      this.BindNullableDateTime("@processedDate", notificationDetails.ProcessedDate);
      this.BindNullableDateTime("@completedDate", notificationDetails.CompletedDate);
      this.BindInt("@requestAttempts", notificationDetails.RequestAttempts);
      double? requestDuration = notificationDetails.RequestDuration;
      if (!requestDuration.HasValue)
      {
        this.BindNullValue("@requestDuration", SqlDbType.Float);
      }
      else
      {
        requestDuration = notificationDetails.RequestDuration;
        this.BindDouble("@requestDuration", requestDuration.Value);
      }
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification1 = this.FinishReadNotification(false);
      notification.Id = notification1.Id;
      notification.CreatedDate = notification1.CreatedDate;
      notification.ModifiedDate = notification1.ModifiedDate;
    }

    public virtual void UpdateNotification(
      Guid subscriptionId,
      int notificationId,
      NotificationStatus? status = null,
      NotificationResult? result = null,
      string request = null,
      string response = null,
      string errorMessage = null,
      string errorDetail = null,
      DateTime? queuedDate = null,
      DateTime? dequeuedDate = null,
      DateTime? processedDate = null,
      DateTime? completedDate = null,
      double? requestDuration = null,
      bool incrementRequestAttempts = false)
    {
      this.PrepareStoredProcedure("hooks.prc_UpdateNotification");
      this.BindGuid("@subscriptionId", subscriptionId);
      this.BindInt("@notificationId", notificationId);
      if (!status.HasValue)
        this.BindNullValue("@status", SqlDbType.SmallInt);
      else
        this.BindInt("@status", (int) status.Value);
      if (!result.HasValue)
        this.BindNullValue("@result", SqlDbType.SmallInt);
      else
        this.BindInt("@result", (int) result.Value);
      this.BindString("@request", request, 64000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@response", response, 64000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@errorMessage", errorMessage, int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@errorDetail", errorDetail, int.MaxValue, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableDateTime("@queuedDate", queuedDate);
      this.BindNullableDateTime("@dequeuedDate", dequeuedDate);
      this.BindNullableDateTime("@processedDate", processedDate);
      this.BindNullableDateTime("@completedDate", completedDate);
      this.BindBoolean("@incRequestAttempts", incrementRequestAttempts);
      if (!requestDuration.HasValue)
        this.BindNullValue("@requestDuration", SqlDbType.Float);
      else
        this.BindDouble("@requestDuration", requestDuration.Value);
      this.ExecuteNonQuery();
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription ReadSubscription(
      Guid subscriptionId)
    {
      return this.ReadSubscriptions((IEnumerable<Guid>) new Guid[1]
      {
        subscriptionId
      }).FirstOrDefault<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
    }

    public virtual IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> ReadSubscriptions(
      IEnumerable<Guid> subscriptionIds)
    {
      this.PrepareStoredProcedure("hooks.prc_ReadSubscriptions");
      this.BindGuidTable("@subscriptionIds", subscriptionIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.GetSubscriptionBinder());
      IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> items1 = (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>().Items;
      resultCollection.NextResult();
      resultCollection.AddBinder<SubscriptionInputValue>((ObjectBinder<SubscriptionInputValue>) new SubscriptionInputBinder());
      List<SubscriptionInputValue> items2 = resultCollection.GetCurrent<SubscriptionInputValue>().Items;
      IDictionary<Guid, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> dictionary = (IDictionary<Guid, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) items1.ToDictionary<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, Guid>((System.Func<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription, Guid>) (s => s.Id));
      foreach (SubscriptionInputValue subscriptionInputValue in (IEnumerable<SubscriptionInputValue>) items2)
      {
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription;
        if (dictionary.TryGetValue(subscriptionInputValue.SubscriptionId, out subscription))
        {
          if (subscriptionInputValue.Scope == SubscriptionInputScope.Consumer)
            subscription.ConsumerInputs[subscriptionInputValue.InputId] = subscriptionInputValue.InputValue;
          else
            subscription.PublisherInputs[subscriptionInputValue.InputId] = subscriptionInputValue.InputValue;
        }
      }
      return items1;
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      this.PrepareStoredProcedure("hooks.prc_CreateSubscription");
      this.BindGuid("@subscriptionId", subscription.Id);
      this.BindInt("@status", (int) (short) subscription.Status);
      this.BindString("@publisherId", subscription.PublisherId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@eventType", subscription.EventType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerId", subscription.ConsumerId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerActionId", subscription.ConsumerActionId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@createdBy", Guid.Parse(subscription.CreatedBy.Id));
      this.BindSubscriptionInputsTable("@inputs", subscription);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.GetSubscriptionBinder());
      rc.AddBinder<SubscriptionInputValue>((ObjectBinder<SubscriptionInputValue>) new SubscriptionInputBinder());
      return ServiceHooksComponent.BindSubscriptionDeep(rc);
    }

    public virtual void UpdateSubscriptionStatus(
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
      this.BindGuid("@modifiedBy", modifiedByIdentity);
      this.ExecuteNonQuery();
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription UpdateSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      bool updateProbationRetries,
      bool requestedByUser)
    {
      this.PrepareStoredProcedure("hooks.prc_UpdateSubscription");
      this.BindGuid("@subscriptionId", subscription.Id);
      this.BindInt("@status", (int) (short) subscription.Status);
      this.BindString("@publisherId", subscription.PublisherId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@eventType", subscription.EventType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerId", subscription.ConsumerId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@consumerActionId", subscription.ConsumerActionId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@modifiedBy", Guid.Parse(subscription.ModifiedBy.Id));
      this.BindSubscriptionInputsTable("@inputs", subscription);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.GetSubscriptionBinder());
      rc.AddBinder<SubscriptionInputValue>((ObjectBinder<SubscriptionInputValue>) new SubscriptionInputBinder());
      return ServiceHooksComponent.BindSubscriptionDeep(rc);
    }

    public virtual void DeleteSubscription(Guid subscriptionId)
    {
      this.PrepareStoredProcedure("hooks.prc_DeleteSubscription");
      this.BindGuid("@subscriptionId", subscriptionId);
      this.ExecuteNonQuery();
    }

    public virtual IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> QuerySubscriptions(
      SubscriptionStatus? status,
      string publisherId,
      string eventType,
      string consumerId,
      string consumerActionId)
    {
      this.PrepareStoredProcedure("hooks.prc_QuerySubscriptions");
      if (!status.HasValue)
        this.BindNullValue("@status", SqlDbType.Bit);
      else
        this.BindInt("@status", (int) (short) status.Value);
      if (publisherId == null)
        this.BindNullValue("@publisherId", SqlDbType.NVarChar);
      else
        this.BindString("@publisherId", publisherId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (eventType == null)
        this.BindNullValue("@eventType", SqlDbType.NVarChar);
      else
        this.BindString("@eventType", eventType, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (consumerId == null)
        this.BindNullValue("@consumerId", SqlDbType.NVarChar);
      else
        this.BindString("@consumerId", consumerId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (consumerActionId == null)
        this.BindNullValue("@consumerActionId", SqlDbType.NVarChar);
      else
        this.BindString("@consumerActionId", consumerActionId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.GetSubscriptionBinder());
      List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> items = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>().Items;
      Dictionary<Guid, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> dictionary = new Dictionary<Guid, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(items.Count);
      foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription in items)
        dictionary.Add(subscription.Id, subscription);
      resultCollection.NextResult();
      resultCollection.AddBinder<SubscriptionInputValue>((ObjectBinder<SubscriptionInputValue>) new SubscriptionInputBinder());
      foreach (SubscriptionInputValue subscriptionInputValue in resultCollection.GetCurrent<SubscriptionInputValue>().Items)
      {
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription;
        if (!dictionary.TryGetValue(subscriptionInputValue.SubscriptionId, out subscription))
          throw new DataMisalignedException(string.Format(ServiceHooksResources.Error_InvalidSubscriptionIdOnSubscriptionInputTemplate, (object) subscriptionInputValue.InputId, (object) subscriptionInputValue.SubscriptionId));
        if (subscriptionInputValue.Scope == SubscriptionInputScope.Consumer)
          subscription.ConsumerInputs[subscriptionInputValue.InputId] = subscriptionInputValue.InputValue;
        else
          subscription.PublisherInputs[subscriptionInputValue.InputId] = subscriptionInputValue.InputValue;
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) items;
    }

    public virtual ServiceHooksStats GetStats()
    {
      this.PrepareStoredProcedure("hooks.prc_GetStats");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ServiceHooksStats>((ObjectBinder<ServiceHooksStats>) new ServiceHooksStatsBinder());
      return resultCollection.GetCurrent<ServiceHooksStats>().Items.FirstOrDefault<ServiceHooksStats>();
    }

    protected static Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription BindSubscriptionDeep(
      ResultCollection rc)
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = rc.GetCurrent<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>().Items.First<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
      rc.NextResult();
      foreach (SubscriptionInputValue subscriptionInputValue in rc.GetCurrent<SubscriptionInputValue>().Items)
      {
        if (subscriptionInputValue.Scope == SubscriptionInputScope.Consumer)
          subscription.ConsumerInputs[subscriptionInputValue.InputId] = subscriptionInputValue.InputValue;
        else
          subscription.PublisherInputs[subscriptionInputValue.InputId] = subscriptionInputValue.InputValue;
      }
      return subscription;
    }

    protected void BindSubscriptionInputsTable(string parameterName, Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      List<SubscriptionInputValue> rows = new List<SubscriptionInputValue>(subscription.PublisherInputs.Count + subscription.ConsumerInputs.Count);
      foreach (KeyValuePair<string, string> publisherInput in (IEnumerable<KeyValuePair<string, string>>) subscription.PublisherInputs)
        rows.Add(new SubscriptionInputValue()
        {
          SubscriptionId = subscription.Id,
          Scope = SubscriptionInputScope.Publisher,
          InputId = publisherInput.Key,
          InputValue = publisherInput.Value
        });
      foreach (KeyValuePair<string, string> consumerInput in (IEnumerable<KeyValuePair<string, string>>) subscription.ConsumerInputs)
        rows.Add(new SubscriptionInputValue()
        {
          SubscriptionId = subscription.Id,
          Scope = SubscriptionInputScope.Consumer,
          InputId = consumerInput.Key,
          InputValue = consumerInput.Value
        });
      this.BindSubscriptionInputValue(parameterName, (IEnumerable<SubscriptionInputValue>) rows);
    }

    protected SqlParameter BindSubscriptionInputValue(
      string parameterName,
      IEnumerable<SubscriptionInputValue> rows)
    {
      rows = rows ?? Enumerable.Empty<SubscriptionInputValue>();
      System.Func<SubscriptionInputValue, SqlDataRecord> selector = (System.Func<SubscriptionInputValue, SqlDataRecord>) (inputValue =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServiceHooksComponent.hooks__typ_SubscriptionInput);
        sqlDataRecord.SetString(0, inputValue.InputId);
        sqlDataRecord.SetString(1, inputValue.InputValue);
        sqlDataRecord.SetInt32(2, (int) inputValue.Scope);
        sqlDataRecord.SetGuid(3, inputValue.SubscriptionId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "hooks.typ_subscriptionInput", rows.Select<SubscriptionInputValue, SqlDataRecord>(selector));
    }

    private Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification FinishReadNotification(
      bool includeDetails)
    {
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) new NotificationBinder());
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>().Items.FirstOrDefault<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>();
      if (includeDetails)
      {
        resultCollection.NextResult();
        resultCollection.AddBinder<NotificationDetails>((ObjectBinder<NotificationDetails>) new NotificationDetailsBinder());
        NotificationDetails notificationDetails = resultCollection.GetCurrent<NotificationDetails>().Items.FirstOrDefault<NotificationDetails>();
        if (notification != null && notificationDetails != null)
          notification.Details = notificationDetails;
      }
      return notification;
    }

    private IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> FinishQueryNotifications(
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults,
      bool includeDetails = false)
    {
      if (!status.HasValue)
        this.BindNullValue("@status", SqlDbType.SmallInt);
      else
        this.BindInt("@status", (int) status.Value);
      if (!result.HasValue)
        this.BindNullValue("@result", SqlDbType.SmallInt);
      else
        this.BindInt("@result", (int) result.Value);
      if (!minDate.HasValue)
        this.BindNullValue("@minCreatedDate", SqlDbType.DateTime);
      else
        this.BindDateTime("@minCreatedDate", minDate.Value);
      if (!maxDate.HasValue)
        this.BindNullValue("@maxCreatedDate", SqlDbType.DateTime);
      else
        this.BindDateTime("@maxCreatedDate", maxDate.Value);
      if (!maxResults.HasValue)
        maxResults = new int?(int.MaxValue);
      this.BindInt("@maxResults", maxResults.Value);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      if (includeDetails)
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) new NotificationWithDetailsBinder());
      else
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>((ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) new NotificationBinder());
      return (IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>().Items;
    }

    protected virtual SubscriptionBinder GetSubscriptionBinder() => new SubscriptionBinder();
  }
}
