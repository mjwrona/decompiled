// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmManualUpstreamIngestionBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmManualUpstreamIngestionBootstrapper : 
    IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly BlockedIdentityContext blockedIdentityContext;

    public NpmManualUpstreamIngestionBootstrapper(
      IVssRequestContext requestContext,
      BlockedIdentityContext blockedIdentityContext)
    {
      this.requestContext = requestContext;
      this.blockedIdentityContext = blockedIdentityContext;
    }

    public IAsyncHandler<IRawPackageRequest, NullResult> Bootstrap() => (IAsyncHandler<IRawPackageRequest, NullResult>) IngestRawPackageIfNotAlreadyIngestedBootstrapper.Create(this.requestContext, this.blockedIdentityContext).Bootstrap().ThenReturnNullResult<IRawPackageRequest, NullResult>();
  }
}
