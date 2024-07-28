// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenRawPackageRequestToRequestConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenRawPackageRequestToRequestConverterBootstrapper : 
    IBootstrapper<IConverter<IRawPackageRequest, PackageRequest<MavenPackageIdentity>>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenRawPackageRequestToRequestConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<IRawPackageRequest, PackageRequest<MavenPackageIdentity>> Bootstrap() => (IConverter<IRawPackageRequest, PackageRequest<MavenPackageIdentity>>) new RawPackageRequestConverter<MavenPackageIdentity>(new MavenRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap());
  }
}
