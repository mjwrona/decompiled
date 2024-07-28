// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.MessagingEntityAlreadyExistsException
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [Serializable]
  public sealed class MessagingEntityAlreadyExistsException : MessagingException
  {
    public MessagingEntityAlreadyExistsException(string entityName)
      : this(MessagingExceptionDetail.EntityConflict(SRClient.MessagingEntityAlreadyExists((object) entityName)), (TrackingContext) null)
    {
      this.IsTransient = false;
    }

    public MessagingEntityAlreadyExistsException(string entityName, TrackingContext trackingContext)
      : this(MessagingExceptionDetail.EntityConflict(SRClient.MessagingEntityAlreadyExists((object) entityName)), trackingContext, (Exception) null)
    {
      this.IsTransient = false;
    }

    public MessagingEntityAlreadyExistsException(
      string message,
      TrackingContext trackingContext,
      Exception innerException)
      : base(MessagingExceptionDetail.EntityConflict(message), trackingContext, innerException)
    {
      this.IsTransient = false;
    }

    internal MessagingEntityAlreadyExistsException(
      MessagingExceptionDetail detail,
      TrackingContext trackingContext)
      : base(detail, trackingContext)
    {
      this.IsTransient = false;
    }

    internal MessagingEntityAlreadyExistsException(
      MessagingExceptionDetail detail,
      TrackingContext trackingContext,
      Exception innerException)
      : base(detail, trackingContext, innerException)
    {
      this.IsTransient = false;
    }

    private MessagingEntityAlreadyExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.IsTransient = false;
    }

    internal object ExistingEntityMetadata { get; set; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) => base.GetObjectData(info, context);
  }
}
