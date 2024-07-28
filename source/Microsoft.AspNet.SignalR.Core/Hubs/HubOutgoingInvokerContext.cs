// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubOutgoingInvokerContext
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hubs
{
  internal class HubOutgoingInvokerContext : IHubOutgoingInvokerContext
  {
    public HubOutgoingInvokerContext(
      IConnection connection,
      string signal,
      ClientHubInvocation invocation)
    {
      this.Connection = connection;
      this.Signal = signal;
      this.Invocation = invocation;
    }

    public HubOutgoingInvokerContext(
      IConnection connection,
      IList<string> signals,
      ClientHubInvocation invocation)
    {
      this.Connection = connection;
      this.Signals = signals;
      this.Invocation = invocation;
    }

    public IConnection Connection { get; private set; }

    public ClientHubInvocation Invocation { get; private set; }

    public string Signal { get; private set; }

    public IList<string> Signals { get; private set; }

    public IList<string> ExcludedSignals { get; set; }
  }
}
