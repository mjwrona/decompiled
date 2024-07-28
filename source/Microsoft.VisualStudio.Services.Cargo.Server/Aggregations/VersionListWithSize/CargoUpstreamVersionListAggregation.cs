// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.VersionListWithSize.CargoUpstreamVersionListAggregation
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.VersionListWithSize
{
  public class CargoUpstreamVersionListAggregation : 
    UpstreamVersionListAggregation<CargoPackageName, CargoPackageVersion>
  {
    public static readonly AggregationDefinition StaticDefinition = UpstreamVersionListAggregation<CargoPackageName, CargoPackageVersion>.MakeDefinition((IProtocol) Protocol.Cargo);
    public static readonly CargoUpstreamVersionListAggregation V1 = new CargoUpstreamVersionListAggregation();

    public override AggregationDefinition Definition => CargoUpstreamVersionListAggregation.StaticDefinition;

    public override IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => this.BootstrapCore(requestContext, CargoIdentityResolver.Instance.VersionResolver);
  }
}
