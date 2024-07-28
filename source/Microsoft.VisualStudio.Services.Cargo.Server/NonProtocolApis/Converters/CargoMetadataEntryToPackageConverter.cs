// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.Converters.CargoMetadataEntryToPackageConverter
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.Converters
{
  public class CargoMetadataEntryToPackageConverter : 
    IConverter<ICargoMetadataEntry, Package>,
    IHaveInputType<ICargoMetadataEntry>,
    IHaveOutputType<Package>
  {
    public Package Convert(ICargoMetadataEntry input) => new Package()
    {
      Id = input.PackageIdentity.Name.NormalizedName,
      DeletedDate = input.DeletedDate,
      Version = input.PackageIdentity.Version.NormalizedVersion,
      Name = input.PackageIdentity.Name.DisplayName,
      PermanentlyDeletedDate = input.PermanentDeletedDate,
      SourceChain = input.SourceChain
    };
  }
}
