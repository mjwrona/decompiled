// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.MessagingException
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [Serializable]
  public class MessagingException : Exception
  {
    public MessagingException(string message)
      : base(message)
    {
      this.Initialize(MessagingExceptionDetail.UnknownDetail(message), (TrackingContext) null, DateTime.UtcNow);
    }

    public MessagingException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.Initialize(MessagingExceptionDetail.UnknownDetail(message), (TrackingContext) null, DateTime.UtcNow);
    }

    public MessagingException(string message, bool isTransientError, Exception innerException)
      : base(message, innerException)
    {
      this.Initialize(MessagingExceptionDetail.UnknownDetail(message), (TrackingContext) null, DateTime.UtcNow);
      this.IsTransient = isTransientError;
    }

    internal MessagingException(MessagingExceptionDetail detail, TrackingContext trackingContext)
      : base(detail.Message)
    {
      this.Initialize(detail, trackingContext, DateTime.UtcNow);
    }

    internal MessagingException(
      MessagingExceptionDetail detail,
      TrackingContext trackingContext,
      Exception innerException)
      : base(detail.Message, innerException)
    {
      this.Initialize(detail, trackingContext, DateTime.UtcNow);
    }

    protected MessagingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Initialize((MessagingExceptionDetail) info.GetValue(nameof (Detail), typeof (MessagingExceptionDetail)), TrackingContext.GetInstance((string) info.GetValue("TrackingId", typeof (string)), (string) info.GetValue("SubsystemId", typeof (string)), false), (DateTime) info.GetValue(nameof (Timestamp), typeof (DateTime)));
    }

    public MessagingExceptionDetail Detail { get; private set; }

    internal TrackingContext Tracker { get; private set; }

    public DateTime Timestamp { get; private set; }

    public new bool IsTransient { get; protected set; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Detail", (object) this.Detail);
      info.AddValue("TrackingId", (object) this.Tracker.TrackingId);
      info.AddValue("SubsystemId", (object) this.Tracker.SystemTracker);
      info.AddValue("Timestamp", (object) this.Timestamp.ToString());
    }

    public override sealed IDictionary Data => base.Data;

    private void Initialize(
      MessagingExceptionDetail detail,
      TrackingContext currentTracker,
      DateTime timestamp)
    {
      this.IsTransient = true;
      this.Detail = detail;
      this.Tracker = currentTracker ?? TrackingContext.GetInstance(Guid.NewGuid());
      this.Timestamp = timestamp;
      if (!(this.GetType() != typeof (MessagingException)))
        return;
      this.DisablePrepareForRethrow();
    }
  }
}
