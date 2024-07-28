// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenRawPackageRequestWithDataConverterBootstrapper`1
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
  internal class MavenRawPackageRequestWithDataConverterBootstrapper<TAdditionalData> : 
    IBootstrapper<IConverter<RawPackageRequest<TAdditionalData>, PackageRequest<MavenPackageIdentity, TAdditionalData>>>
    where TAdditionalData : class
  {
    private readonly IVssRequestContext requestContext;

    public MavenRawPackageRequestWithDataConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<RawPackageRequest<TAdditionalData>, PackageRequest<MavenPackageIdentity, TAdditionalData>> Bootstrap() => (IConverter<RawPackageRequest<TAdditionalData>, PackageRequest<MavenPackageIdentity, TAdditionalData>>) new RawPackageRequestWithDataConverter<MavenPackageIdentity, TAdditionalData>(new MavenRawPackageRequestToIdentityConverterBootstrapper(this.requestContext).Bootstrap());
  }
}
