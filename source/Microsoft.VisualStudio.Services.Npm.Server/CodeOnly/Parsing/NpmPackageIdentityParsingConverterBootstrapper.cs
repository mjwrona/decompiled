// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing.NpmPackageIdentityParsingConverterBootstrapper
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
  public class NpmPackageIdentityParsingConverterBootstrapper : 
    IBootstrapper<IConverter<IRawPackageRequest, NpmPackageIdentity>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmPackageIdentityParsingConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<IRawPackageRequest, NpmPackageIdentity> Bootstrap() => (IConverter<IRawPackageRequest, NpmPackageIdentity>) new PopulateRequestContextItemDelegatingConverter<IRawPackageRequest, NpmPackageIdentity>(this.requestContext, "Packaging.PackageIdentity", (IConverter<IRawPackageRequest, NpmPackageIdentity>) new NpmPackageIdentityParsingConverter());
  }
}
