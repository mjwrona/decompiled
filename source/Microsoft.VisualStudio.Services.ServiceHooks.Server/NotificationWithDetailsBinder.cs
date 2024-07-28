// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.NotificationWithDetailsBinder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  internal class NotificationWithDetailsBinder : ObjectBinder<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>
  {
    private SqlColumnBinder NotificationId = new SqlColumnBinder(nameof (NotificationId));
    private SqlColumnBinder SubscriptionId = new SqlColumnBinder(nameof (SubscriptionId));
    private SqlColumnBinder EventId = new SqlColumnBinder(nameof (EventId));
    private SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
    private SqlColumnBinder Result = new SqlColumnBinder(nameof (Result));
    private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));
    private SqlColumnBinder ModifiedDate = new SqlColumnBinder(nameof (ModifiedDate));
    private SqlColumnBinder EventType = new SqlColumnBinder(nameof (EventType));
    private SqlColumnBinder EventContent = new SqlColumnBinder(nameof (EventContent));
    private SqlColumnBinder PublisherId = new SqlColumnBinder(nameof (PublisherId));
    private SqlColumnBinder ConsumerId = new SqlColumnBinder(nameof (ConsumerId));
    private SqlColumnBinder ConsumerActionId = new SqlColumnBinder(nameof (ConsumerActionId));
    private SqlColumnBinder ConsumerInputs = new SqlColumnBinder(nameof (ConsumerInputs));
    private SqlColumnBinder PublisherInputs = new SqlColumnBinder(nameof (PublisherInputs));
    private SqlColumnBinder Request = new SqlColumnBinder(nameof (Request));
    private SqlColumnBinder Response = new SqlColumnBinder(nameof (Response));
    private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
    private SqlColumnBinder ErrorDetail = new SqlColumnBinder(nameof (ErrorDetail));
    private SqlColumnBinder QueuedDate = new SqlColumnBinder(nameof (QueuedDate));
    private SqlColumnBinder DequeuedDate = new SqlColumnBinder(nameof (DequeuedDate));
    private SqlColumnBinder ProcessedDate = new SqlColumnBinder(nameof (ProcessedDate));
    private SqlColumnBinder CompletedDate = new SqlColumnBinder(nameof (CompletedDate));
    private SqlColumnBinder RequestDuration = new SqlColumnBinder(nameof (RequestDuration));
    private SqlColumnBinder RequestAttempts = new SqlColumnBinder(nameof (RequestAttempts));
    private static readonly JsonSerializerSettings s_serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;

    protected override Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification Bind()
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification = new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification();
      notification.Id = this.NotificationId.GetInt32((IDataReader) this.Reader);
      notification.SubscriptionId = this.SubscriptionId.GetGuid((IDataReader) this.Reader);
      notification.EventId = this.EventId.GetGuid((IDataReader) this.Reader);
      notification.Status = (NotificationStatus) this.Status.GetInt16((IDataReader) this.Reader);
      notification.Result = (NotificationResult) this.Result.GetInt16((IDataReader) this.Reader);
      notification.CreatedDate = this.CreatedDate.GetDateTime((IDataReader) this.Reader);
      notification.ModifiedDate = this.ModifiedDate.GetDateTime((IDataReader) this.Reader);
      NotificationDetails notificationDetails = new NotificationDetails()
      {
        EventType = this.EventType.GetString((IDataReader) this.Reader, false),
        PublisherId = this.PublisherId.GetString((IDataReader) this.Reader, false),
        ConsumerId = this.ConsumerId.GetString((IDataReader) this.Reader, false),
        ConsumerActionId = this.ConsumerActionId.GetString((IDataReader) this.Reader, false),
        Request = this.Request.GetString((IDataReader) this.Reader, true),
        Response = this.Response.GetString((IDataReader) this.Reader, true),
        ErrorMessage = this.ErrorMessage.GetString((IDataReader) this.Reader, true),
        ErrorDetail = this.ErrorDetail.GetString((IDataReader) this.Reader, true)
      };
      if (!this.QueuedDate.IsNull((IDataReader) this.Reader))
        notificationDetails.QueuedDate = new DateTime?(this.QueuedDate.GetDateTime((IDataReader) this.Reader));
      if (!this.DequeuedDate.IsNull((IDataReader) this.Reader))
        notificationDetails.DequeuedDate = new DateTime?(this.DequeuedDate.GetDateTime((IDataReader) this.Reader));
      if (!this.ProcessedDate.IsNull((IDataReader) this.Reader))
        notificationDetails.ProcessedDate = new DateTime?(this.ProcessedDate.GetDateTime((IDataReader) this.Reader));
      if (!this.CompletedDate.IsNull((IDataReader) this.Reader))
        notificationDetails.CompletedDate = new DateTime?(this.CompletedDate.GetDateTime((IDataReader) this.Reader));
      if (!this.RequestDuration.IsNull((IDataReader) this.Reader))
        notificationDetails.RequestDuration = new double?(this.RequestDuration.GetDouble((IDataReader) this.Reader));
      if (!this.RequestAttempts.IsNull((IDataReader) this.Reader))
        notificationDetails.RequestAttempts = (int) this.RequestAttempts.GetInt16((IDataReader) this.Reader);
      string str1 = this.EventContent.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(str1))
        notificationDetails.Event = JsonConvert.DeserializeObject<Event>(str1, NotificationWithDetailsBinder.s_serializerSettings);
      string str2 = this.ConsumerInputs.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(str2))
        notificationDetails.ConsumerInputs = (IDictionary<string, string>) JsonConvert.DeserializeObject<Dictionary<string, string>>(str2, NotificationWithDetailsBinder.s_serializerSettings);
      string str3 = this.PublisherInputs.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(str3))
        notificationDetails.PublisherInputs = (IDictionary<string, string>) JsonConvert.DeserializeObject<Dictionary<string, string>>(str3, NotificationWithDetailsBinder.s_serializerSettings);
      notification.Details = notificationDetails;
      return notification;
    }
  }
}
