// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AfdEndpointResolverProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AfdEndpointResolverProvider : IAfdEndpointResolverProvider, IVssFrameworkService
  {
    private Dictionary<int, IAfdEndpointResolver> m_resolvers = new Dictionary<int, IAfdEndpointResolver>();

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.RegisterResolver(0, (IAfdEndpointResolver) new AfdEndpointResolverV0());
      this.RegisterResolver(2, (IAfdEndpointResolver) new AfdEndpointResolverV2());
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool TryGetResolver(int version, out IAfdEndpointResolver resolver) => this.m_resolvers.TryGetValue(version, out resolver);

    protected void RegisterResolver(int version, IAfdEndpointResolver resolver) => this.m_resolvers[version] = resolver;
  }
}
