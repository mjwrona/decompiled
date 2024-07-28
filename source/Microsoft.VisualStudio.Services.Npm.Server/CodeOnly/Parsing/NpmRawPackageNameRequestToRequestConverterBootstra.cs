// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing.NpmRawPackageNameRequestToRequestConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing
{
  public class NpmRawPackageNameRequestToRequestConverterBootstrapper : 
    IBootstrapper<IConverter<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>>>
  {
    private readonly IVssRequestContext requestContext;

    public NpmRawPackageNameRequestToRequestConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>> Bootstrap() => (IConverter<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>>) new RawPackageNameRequestConverter<NpmPackageName>(new RequestContextPopulatingRawNameToNameBootstrapper<NpmPackageName>(this.requestContext, (IConverter<string, NpmPackageName>) new NpmRawPackageNameRequestToNameConverter()).Bootstrap());
  }
}
