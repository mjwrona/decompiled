// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBusMessage
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class ServiceBusMessage : IMessage
  {
    internal BrokeredMessage m_message;

    internal ServiceBusMessage(BrokeredMessage message) => this.m_message = message;

    public long SequenceNumber => this.m_message.SequenceNumber;

    public T GetBody<T>()
    {
      if (!(this.m_message.ContentType == "Microsoft.VisualStudio.Services.WebApi.ServiceEvent"))
        return this.m_message.GetBody<T>();
      if (typeof (T) != typeof (ServiceEvent))
        throw new InvalidCastException(this.GetType().AssemblyQualifiedName);
      return this.m_message.GetBody<T>((XmlObjectSerializer) ServiceBusDataContractResolver.GetDataContractSerializer(typeof (ServiceEvent)));
    }

    public IDictionary<string, object> Properties => this.m_message.Properties;

    public string ContentType => this.m_message.ContentType;

    public string PartitionKey
    {
      get => this.m_message.PartitionKey;
      set => throw new NotSupportedException();
    }

    public DateTime EnqueuedTimeUtc => this.m_message.EnqueuedTimeUtc;
  }
}
