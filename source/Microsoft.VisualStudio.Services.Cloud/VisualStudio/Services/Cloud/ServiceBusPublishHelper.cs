// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusPublishHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ServiceBusPublishHelper
  {
    private const string s_Area = "ServiceBus";
    private const string s_Layer = "ServiceBusPublishHelper";

    internal static void DisposeMessages(List<BrokeredMessage> messages)
    {
      foreach (BrokeredMessage message in messages)
        message.Dispose();
      messages.Clear();
    }

    internal static BrokeredMessage GetBrokeredMessage(
      MessageBusMessage input,
      string messageBusIdentifier)
    {
      object body = input.GetBody<object>();
      Type type = body.GetType();
      BrokeredMessage brokeredMessage;
      if (typeof (Stream).IsAssignableFrom(type))
      {
        brokeredMessage = new BrokeredMessage((Stream) body);
        brokeredMessage.ContentType = typeof (Stream).FullName;
      }
      else
      {
        brokeredMessage = !(type == typeof (ServiceEvent)) ? (!(type == typeof (BrokeredMessage)) ? new BrokeredMessage(body) : body as BrokeredMessage) : new BrokeredMessage(body, (XmlObjectSerializer) ServiceBusDataContractResolver.GetDataContractSerializer(type));
        brokeredMessage.ContentType = type.FullName;
      }
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) input.Properties)
        brokeredMessage.Properties.Add(property.Key, property.Value);
      if (!string.IsNullOrEmpty(input.PartitionKey))
        brokeredMessage.PartitionKey = input.PartitionKey;
      if (!string.IsNullOrEmpty(input.SessionId))
        brokeredMessage.SessionId = input.SessionId;
      return brokeredMessage;
    }

    internal static long GetHeaderSize(BrokeredMessage message) => (long) (15 + 2 * ((string.IsNullOrEmpty(message.MessageId) ? 0 : message.MessageId.Length) + (string.IsNullOrEmpty(message.SessionId) ? 0 : message.SessionId.Length) + (string.IsNullOrEmpty(message.PartitionKey) ? 0 : message.PartitionKey.Length)));

    internal static void HandlePublishException(
      IVssRequestContext requestContext,
      Exception exception,
      string namespaceName,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish,
      List<BrokeredMessage> messages,
      long size)
    {
      requestContext.TraceException(1005608, "ServiceBus", messageBusIdentifier, exception);
      string str = string.Empty;
      if (messagesToPublish != null && messagesToPublish.Length != 0)
        str = string.Join(",", ((IEnumerable<MessageBusMessage>) messagesToPublish).Select<MessageBusMessage, string>((Func<MessageBusMessage, string>) (x => x.ContentType)));
      requestContext.Trace(1005607, TraceLevel.Error, "ServiceBus", nameof (ServiceBusPublishHelper), "Failed to send {0} messages of size:{1} for message bus:{2} on Namespace {3}. ContentTypes:{4}", (object) messagesToPublish.Length, (object) size, (object) messageBusIdentifier, (object) namespaceName, (object) str);
      ServiceBusPublishHelper.TraceSocketExceptionData(requestContext, exception);
      if (exception is MessageSizeExceededException exception1)
        throw ServiceBusHelper.HandleSizeExceededException(requestContext, exception1, messageBusIdentifier, messages);
      throw exception;
    }

    internal static void TracePartitionIds(
      IVssRequestContext requestContext,
      string messageBusIdentifier,
      MessageBusMessage[] messagesToPublish)
    {
      if (!requestContext.IsTracing(1005610, TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusPublishHelper)))
        return;
      List<string> list = ((IEnumerable<MessageBusMessage>) messagesToPublish).Select<MessageBusMessage, string>((Func<MessageBusMessage, string>) (x =>
      {
        if (x.PartitionKey == null)
          return "null";
        return !(x.PartitionKey == "") ? x.PartitionKey : "empty";
      })).Distinct<string>().ToList<string>();
      requestContext.Trace(1005610, list.Count > 1 ? TraceLevel.Error : TraceLevel.Verbose, "ServiceBus", nameof (ServiceBusPublishHelper), "Messages for {0} have the following distinct partitionIds: {1}", (object) messageBusIdentifier, (object) string.Join(",", (IEnumerable<string>) list));
    }

    internal static void TraceSocketExceptionData(IVssRequestContext requestContext, Exception ex)
    {
      if (!(ex is MessagingCommunicationException communicationException) || communicationException.InnerException == null || !(communicationException.InnerException is SocketException innerException))
        return;
      requestContext.Trace(1005156, TraceLevel.Error, "ServiceBus", nameof (ServiceBusPublishHelper), string.Format("Socket Error:{0}.", (object) innerException.SocketErrorCode));
    }
  }
}
