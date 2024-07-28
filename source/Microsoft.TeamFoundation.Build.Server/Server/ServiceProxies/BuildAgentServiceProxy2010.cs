// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.BuildAgentServiceProxy2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Machine.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal sealed class BuildAgentServiceProxy2010 : 
    ServiceProxy<IBuildAgentService>,
    IBuildAgentService
  {
    public BuildAgentServiceProxy2010(
      IVssRequestContext requestContext,
      string url,
      bool requireClientCertificates)
      : base(requestContext, url, requireClientCertificates)
    {
    }

    public void TestConnection() => this.Do((Action<IBuildAgentService>) (channel => channel.TestConnection()));
  }
}
