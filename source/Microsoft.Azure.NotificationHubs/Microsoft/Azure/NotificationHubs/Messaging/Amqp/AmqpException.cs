// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.AmqpException
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  [Serializable]
  internal sealed class AmqpException : Exception
  {
    public AmqpException(Error error)
    {
      Error error1 = error;
      // ISSUE: explicit constructor call
      this.\u002Ector(error1, error1.Description, (Exception) null);
    }

    public AmqpException(Error error, string message)
      : this(error, message, (Exception) null)
    {
    }

    public AmqpException(Error error, Exception innerException)
    {
      Error error1 = error;
      // ISSUE: explicit constructor call
      this.\u002Ector(error1, error1.Description ?? innerException.Message, innerException);
    }

    private AmqpException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Error = (Error) info.GetValue(nameof (Error), typeof (Error));
    }

    private AmqpException(Error error, string message, Exception innerException)
      : base(message ?? SRAmqp.AmqpErrorOccurred((object) error.Condition), innerException)
    {
      this.Error = error;
      if (string.IsNullOrEmpty(message))
        return;
      this.Error.Description = message;
    }

    public Error Error { get; private set; }

    public static AmqpException FromError(Error error)
    {
      if (error == null || error.Condition.Value == null)
        return (AmqpException) null;
      if (error.Description == null)
        return new AmqpException(AmqpError.GetError(error.Condition));
      Error error1 = error;
      return new AmqpException(error1, error1.Description);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Error", (object) this.Error);
    }
  }
}
