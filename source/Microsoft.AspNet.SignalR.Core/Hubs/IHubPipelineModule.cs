// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.IHubPipelineModule
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public interface IHubPipelineModule
  {
    Func<IHubIncomingInvokerContext, Task<object>> BuildIncoming(
      Func<IHubIncomingInvokerContext, Task<object>> invoke);

    Func<IHubOutgoingInvokerContext, Task> BuildOutgoing(Func<IHubOutgoingInvokerContext, Task> send);

    Func<IHub, Task> BuildConnect(Func<IHub, Task> connect);

    Func<IHub, Task> BuildReconnect(Func<IHub, Task> reconnect);

    Func<IHub, bool, Task> BuildDisconnect(Func<IHub, bool, Task> disconnect);

    Func<HubDescriptor, IRequest, bool> BuildAuthorizeConnect(
      Func<HubDescriptor, IRequest, bool> authorizeConnect);

    Func<HubDescriptor, IRequest, IList<string>, IList<string>> BuildRejoiningGroups(
      Func<HubDescriptor, IRequest, IList<string>, IList<string>> rejoiningGroups);
  }
}
