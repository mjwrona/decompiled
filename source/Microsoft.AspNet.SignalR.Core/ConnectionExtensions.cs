// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.ConnectionExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR
{
  public static class ConnectionExtensions
  {
    public static Task Send(this IConnection connection, string connectionId, object value)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      ConnectionMessage message = !string.IsNullOrEmpty(connectionId) ? new ConnectionMessage(PrefixHelper.GetConnectionId(connectionId), value) : throw new ArgumentException(Resources.Error_ArgumentNullOrEmpty, nameof (connectionId));
      return connection.Send(message);
    }

    public static Task Send(this IConnection connection, IList<string> connectionIds, object value)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      if (connectionIds == null)
        throw new ArgumentNullException(nameof (connectionIds));
      ConnectionMessage message = new ConnectionMessage((IList<string>) connectionIds.Select<string, string>((Func<string, string>) (c => PrefixHelper.GetConnectionId(c))).ToList<string>(), value);
      return connection.Send(message);
    }

    public static Task Broadcast(
      this IConnection connection,
      object value,
      params string[] excludeConnectionIds)
    {
      if (connection == null)
        throw new ArgumentNullException(nameof (connection));
      ConnectionMessage message = new ConnectionMessage(connection.DefaultSignal, value, PrefixHelper.GetPrefixedConnectionIds((IList<string>) excludeConnectionIds));
      return connection.Send(message);
    }
  }
}
