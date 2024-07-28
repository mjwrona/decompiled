// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity.PyPiRawPackageRequestToIdentityConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Converters.Identity
{
  public class PyPiRawPackageRequestToIdentityConverterBootstrapper : 
    IBootstrapper<IConverter<IRawPackageRequest, PyPiPackageIdentity>>
  {
    private readonly IVssRequestContext requestContext;

    public PyPiRawPackageRequestToIdentityConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<IRawPackageRequest, PyPiPackageIdentity> Bootstrap() => new RequestContextPopulatingRawRequestToIdentityConverterBootstrapper<IRawPackageRequest, PyPiPackageIdentity>(this.requestContext, (IConverter<IRawPackageRequest, PyPiPackageIdentity>) new PyPiRawPackageRequestToIdentityConverter()).Bootstrap();
  }
}
