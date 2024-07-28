// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.GetSeekableBlobHttpStreamHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class GetSeekableBlobHttpStreamHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<BlobIdentifier, Stream>>
  {
    private readonly IVssRequestContext requestContext;

    public GetSeekableBlobHttpStreamHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<BlobIdentifier, Stream> Bootstrap() => (IAsyncHandler<BlobIdentifier, Stream>) new GetSeekableBlobHttpStreamHandler(new ContentBlobStoreFacadeBootstrapper(this.requestContext).Bootstrap(), (IFactory<DateTimeOffset>) new SasTokenExpiryTimeFactoryFacade(this.requestContext));
  }
}
