// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.RequestContextPopulatingRawRequestToIdentityConverterBootstrapper`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class RequestContextPopulatingRawRequestToIdentityConverterBootstrapper<TReq, TIdentity> : 
    IBootstrapper<IConverter<TReq, TIdentity>>
    where TReq : IRawPackageRequest
    where TIdentity : IPackageIdentity
  {
    private readonly IVssRequestContext requestContext;
    private readonly IConverter<TReq, TIdentity> actualConverter;

    public RequestContextPopulatingRawRequestToIdentityConverterBootstrapper(
      IVssRequestContext requestContext,
      IConverter<TReq, TIdentity> actualConverter)
    {
      this.requestContext = requestContext;
      this.actualConverter = actualConverter;
    }

    public IConverter<TReq, TIdentity> Bootstrap() => (IConverter<TReq, TIdentity>) new PopulateRequestContextItemDelegatingConverter<TReq, TIdentity>(this.requestContext, "Packaging.PackageIdentity", this.actualConverter);
  }
}
