// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.GetFileAsyncBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class GetFileAsyncBootstrapper : 
    IBootstrapper<IAsyncHandler<IRawPackageInnerFileRequest<NuGetGetFileData>, HttpResponseMessage>>
  {
    private readonly IVssRequestContext requestContext;

    public GetFileAsyncBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<IRawPackageInnerFileRequest<NuGetGetFileData>, HttpResponseMessage> Bootstrap()
    {
      IValidator<IPackageIdentity> validator = new BlockedPackageIdentityToExceptionConverterBootstrapper(this.requestContext).Bootstrap().AsThrowingValidator<IPackageIdentity>();
      return new RawPackageInnerFileRequestConverter<VssNuGetPackageIdentity, NuGetGetFileData>(new NuGetRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap().ValidateResultWith<IRawPackageRequest, VssNuGetPackageIdentity, IPackageIdentity>(validator)).ThenDelegateTo<IRawPackageInnerFileRequest<NuGetGetFileData>, IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>(NuGetAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>((IRequireAggBootstrapper<IAsyncHandler<IPackageInnerFileRequest<VssNuGetPackageIdentity, NuGetGetFileData>, HttpResponseMessage>>) new V3BlobGetFileAsyncHandlerBootstrapper(this.requestContext)));
    }
  }
}
