// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.TransportConnectionExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Messaging;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  internal static class TransportConnectionExtensions
  {
    internal static Task Initialize(this ITransportConnection connection, string connectionId) => TransportConnectionExtensions.SendCommand(connection, connectionId, CommandType.Initializing);

    internal static Task Abort(this ITransportConnection connection, string connectionId) => TransportConnectionExtensions.SendCommand(connection, connectionId, CommandType.Abort);

    private static Task SendCommand(
      ITransportConnection connection,
      string connectionId,
      CommandType commandType)
    {
      Command command = new Command()
      {
        CommandType = commandType
      };
      ConnectionMessage message = new ConnectionMessage(PrefixHelper.GetConnectionId(connectionId), (object) command);
      return connection.Send(message);
    }
  }
}
