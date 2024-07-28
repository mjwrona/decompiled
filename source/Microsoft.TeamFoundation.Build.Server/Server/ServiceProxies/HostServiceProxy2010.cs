// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.HostServiceProxy2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Machine.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal sealed class HostServiceProxy2010 : ServiceProxy<IHostService>, IHostService
  {
    public HostServiceProxy2010(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates)
      : base(requestContext, url, requireClientCertificates)
    {
    }

    public IAsyncResult BeginAgentUpdated(
      string uri,
      ServiceAction action,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAsyncInvoke((Func<IHostService, AsyncCallback, object, IAsyncResult>) ((x, y, z) => x.BeginAgentUpdated(uri, action, y, z)), (Action<IHostService, IAsyncResult>) ((x, y) => x.EndAgentUpdated(y)), callback, state);
    }

    public void EndAgentUpdated(IAsyncResult result) => this.EndAsyncInvoke(result);

    public IAsyncResult BeginControllerUpdated(
      string uri,
      ServiceAction action,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAsyncInvoke((Func<IHostService, AsyncCallback, object, IAsyncResult>) ((x, y, z) => x.BeginControllerUpdated(uri, action, y, z)), (Action<IHostService, IAsyncResult>) ((x, y) => x.EndControllerUpdated(y)), callback, state);
    }

    public void EndControllerUpdated(IAsyncResult result) => this.EndAsyncInvoke(result);
  }
}
