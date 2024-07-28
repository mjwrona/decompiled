// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.NoMatchingSubscriptionException
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [Serializable]
  public sealed class NoMatchingSubscriptionException : MessagingException
  {
    public NoMatchingSubscriptionException(string message)
      : base(message)
    {
      this.IsTransient = false;
    }

    public NoMatchingSubscriptionException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.IsTransient = false;
    }

    public override string ToString() => this.Message;
  }
}
