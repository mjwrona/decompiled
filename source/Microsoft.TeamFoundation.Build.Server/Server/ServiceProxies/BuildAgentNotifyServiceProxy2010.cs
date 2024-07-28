// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.BuildAgentNotifyServiceProxy2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Machine.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal sealed class BuildAgentNotifyServiceProxy2010 : 
    ServiceProxy<IBuildAgentNotifyService>,
    IBuildAgentNotifyService
  {
    public BuildAgentNotifyServiceProxy2010(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates)
      : base(requestContext, url, requireClientCertificates)
    {
    }

    public IAsyncResult BeginNotifyAgentAvailable(
      string buildUri,
      int reservationId,
      string agentUri,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAsyncInvoke((Func<IBuildAgentNotifyService, AsyncCallback, object, IAsyncResult>) ((x, y, z) => x.BeginNotifyAgentAvailable(buildUri, reservationId, agentUri, y, z)), (Action<IBuildAgentNotifyService, IAsyncResult>) ((x, y) => x.EndNotifyAgentAvailable(y)), callback, state);
    }

    public void EndNotifyAgentAvailable(IAsyncResult result) => this.EndAsyncInvoke(result);
  }
}
