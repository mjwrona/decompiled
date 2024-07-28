// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Upstreams.NugetManualUpstreamIngestionBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Upstreams
{
  public class NugetManualUpstreamIngestionBootstrapper : 
    IBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>
  {
    private readonly IVssRequestContext requestContext;

    public NugetManualUpstreamIngestionBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IRawPackageRequest, NullResult> Bootstrap() => (IAsyncHandler<IRawPackageRequest, NullResult>) IngestRawPackageIfNotAlreadyIngestedBootstrapper.Create(this.requestContext, BlockedIdentityContext.Download).Bootstrap().ThenReturnNullResult<IRawPackageRequest, ContentResult>();
  }
}
