// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing.NpmRawPackageRequestToRequestConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing
{
  public class NpmRawPackageRequestToRequestConverterBootstrapper : 
    IBootstrapper<IConverter<IRawPackageRequest, PackageRequest<NpmPackageIdentity>>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRawPackageRequestToRequestConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<IRawPackageRequest, PackageRequest<NpmPackageIdentity>> Bootstrap() => (IConverter<IRawPackageRequest, PackageRequest<NpmPackageIdentity>>) new RawPackageRequestConverter<NpmPackageIdentity>(new NpmPackageIdentityParsingConverterBootstrapper(this.requestContext).Bootstrap());
  }
}
