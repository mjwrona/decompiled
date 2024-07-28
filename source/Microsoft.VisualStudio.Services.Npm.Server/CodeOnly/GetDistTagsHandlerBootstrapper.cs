// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.GetDistTagsHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.DistTag;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class GetDistTagsHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawPackageNameRequest, IDictionary<string, string>>>
  {
    private readonly IVssRequestContext requestContext;

    public GetDistTagsHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawPackageNameRequest, IDictionary<string, string>> Bootstrap() => (IAsyncHandler<RawPackageNameRequest, IDictionary<string, string>>) new NpmRawPackageNameRequestToRequestConverterBootstrapper(this.requestContext).Bootstrap().ThenDelegateTo<IRawPackageNameRequest, IPackageNameRequest<NpmPackageName>, IDictionary<string, string>>(NpmAggregationResolver.Bootstrap(this.requestContext).HandlerFor<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>>((IRequireAggBootstrapper<IAsyncHandler<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>>>) new DistTagProviderBootstrapper(this.requestContext)));
  }
}
