// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.InternalServerErrorException
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [Serializable]
  internal sealed class InternalServerErrorException : MessagingException
  {
    public InternalServerErrorException()
      : this((Exception) null)
    {
    }

    public InternalServerErrorException(string message)
      : base(message)
    {
      this.Initialize();
    }

    public InternalServerErrorException(Exception innerException)
      : base(SRClient.InternalServerError, true, innerException)
    {
      this.Initialize();
    }

    public InternalServerErrorException(MessagingExceptionDetail detail, TrackingContext context)
      : base(detail, context)
    {
      this.Initialize();
    }

    public InternalServerErrorException(TrackingContext context)
      : base(MessagingExceptionDetail.UnknownDetail(SRClient.InternalServerError), context)
    {
      this.Initialize();
    }

    private void Initialize() => this.IsTransient = true;
  }
}
