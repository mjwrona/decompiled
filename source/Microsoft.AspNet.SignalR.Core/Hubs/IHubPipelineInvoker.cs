// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.IHubPipelineInvoker
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public interface IHubPipelineInvoker
  {
    Task<object> Invoke(IHubIncomingInvokerContext context);

    Task Send(IHubOutgoingInvokerContext context);

    Task Connect(IHub hub);

    Task Reconnect(IHub hub);

    Task Disconnect(IHub hub, bool stopCalled);

    bool AuthorizeConnect(HubDescriptor hubDescriptor, IRequest request);

    IList<string> RejoiningGroups(
      HubDescriptor hubDescriptor,
      IRequest request,
      IList<string> groups);
  }
}
