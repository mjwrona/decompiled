// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.ITrackingConnection
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  public interface ITrackingConnection : IDisposable
  {
    string ConnectionId { get; }

    CancellationToken CancellationToken { get; }

    Task ConnectTask { get; }

    bool IsAlive { get; }

    bool IsTimedOut { get; }

    bool SupportsKeepAlive { get; }

    bool RequiresTimeout { get; }

    TimeSpan DisconnectThreshold { get; }

    Uri Url { get; }

    void ApplyState(TransportConnectionStates states);

    Task Disconnect();

    void Timeout();

    Task KeepAlive();

    void IncrementConnectionsCount();

    void DecrementConnectionsCount();

    void End();
  }
}
