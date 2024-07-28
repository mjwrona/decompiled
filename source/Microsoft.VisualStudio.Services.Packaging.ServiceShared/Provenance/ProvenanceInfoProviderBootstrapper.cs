// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.ProvenanceInfoProviderBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class ProvenanceInfoProviderBootstrapper
  {
    private readonly IVssRequestContext requestContext;

    public ProvenanceInfoProviderBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public ProvenanceInfoProvider Bootstrap() => new ProvenanceInfoProvider(this.requestContext.UserAgent, (IReadOnlyList<ISessionRequestProvider>) new List<ISessionRequestProvider>()
    {
      (ISessionRequestProvider) new BuildClaimSessionRequestProvider(this.requestContext),
      (ISessionRequestProvider) new SessionIdSessionRequestProvider(this.requestContext)
    });
  }
}
