// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.SharedResourceNotifyServiceProxy2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Machine.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal sealed class SharedResourceNotifyServiceProxy2010 : 
    ServiceProxy<ISharedResourceNotifyService>,
    ISharedResourceNotifyService
  {
    public SharedResourceNotifyServiceProxy2010(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates)
      : base(requestContext, url, requireClientCertificates)
    {
    }

    public void NotifySharedResourceAcquired(
      string resourceName,
      string instanceId,
      string buildUri)
    {
      this.Channel.NotifySharedResourceAcquired(resourceName, instanceId, buildUri);
    }

    public IAsyncResult BeginNotifySharedResourceAcquired(
      string resourceName,
      string instanceId,
      string buildUri,
      AsyncCallback callback,
      object @object)
    {
      return this.Channel.BeginNotifySharedResourceAcquired(resourceName, instanceId, buildUri, callback, @object);
    }

    public void EndNotifySharedResourceAcquired(IAsyncResult result) => this.Channel.EndNotifySharedResourceAcquired(result);
  }
}
