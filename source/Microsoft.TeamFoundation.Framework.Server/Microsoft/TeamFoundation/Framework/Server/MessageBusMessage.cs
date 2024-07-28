// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageBusMessage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MessageBusMessage : IMessage
  {
    private Dictionary<string, object> m_properties = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private object m_object;

    public MessageBusMessage(object o) => this.m_object = o;

    public string ContentType => this.m_object.GetType().FullName;

    public string PartitionKey { get; set; }

    public IDictionary<string, object> Properties => (IDictionary<string, object>) this.m_properties;

    public long SequenceNumber => throw new InvalidOperationException();

    public T GetBody<T>() => (T) this.m_object;

    public DateTime EnqueuedTimeUtc { get; set; }

    public string SessionId { get; set; }
  }
}
