// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NuGetRawPackageRequestToIdentityConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NuGetRawPackageRequestToIdentityConverterBootstrapper : 
    IBootstrapper<IConverter<IRawPackageRequest, VssNuGetPackageIdentity>>
  {
    private readonly IVssRequestContext requestContext;

    public NuGetRawPackageRequestToIdentityConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<IRawPackageRequest, VssNuGetPackageIdentity> Bootstrap() => (IConverter<IRawPackageRequest, VssNuGetPackageIdentity>) new PopulateRequestContextItemDelegatingConverter<IRawPackageRequest, VssNuGetPackageIdentity>(this.requestContext, "Packaging.PackageIdentity", (IConverter<IRawPackageRequest, VssNuGetPackageIdentity>) new NuGetRawPackageRequestToIdentityConverter());
  }
}
