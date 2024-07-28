// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.DeletePackageVersion.CargoDeleteOpGeneratorBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.DeletePackageVersion
{
  public class CargoDeleteOpGeneratorBootstrapper : 
    RequireAggHandlerBootstrapper<IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>>
  {
    private readonly IVssRequestContext requestContext;

    public CargoDeleteOpGeneratorBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    protected override IAsyncHandler<IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData> Bootstrap(
      IReadMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry> metadataService)
    {
      return DeleteOpGeneratorBootstrapper.Create<CargoPackageIdentity, ICargoMetadataEntry>(this.requestContext, (IReadMetadataService<CargoPackageIdentity, ICargoMetadataEntry>) metadataService, (IComparer<IPackageVersion>) new ReverseVersionComparer<CargoPackageVersion>()).Bootstrap();
    }
  }
}
