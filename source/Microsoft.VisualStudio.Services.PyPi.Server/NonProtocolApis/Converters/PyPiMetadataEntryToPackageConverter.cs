// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.Converters.PyPiMetadataEntryToPackageConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.Converters
{
  public class PyPiMetadataEntryToPackageConverter : 
    IConverter<IPyPiMetadataEntry, Package>,
    IHaveInputType<IPyPiMetadataEntry>,
    IHaveOutputType<Package>
  {
    public Package Convert(IPyPiMetadataEntry input) => new Package()
    {
      DeletedDate = input.DeletedDate,
      Version = input.PackageIdentity.Version.NormalizedVersion,
      Name = input.PackageIdentity.Name.DisplayName,
      Id = input.PackageIdentity.Name.NormalizedName,
      PermanentlyDeletedDate = input.PermanentDeletedDate,
      SourceChain = input.SourceChain
    };
  }
}
