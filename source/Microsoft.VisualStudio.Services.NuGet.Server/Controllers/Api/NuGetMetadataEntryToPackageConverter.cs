// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetMetadataEntryToPackageConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  public class NuGetMetadataEntryToPackageConverter : 
    IConverter<INuGetMetadataEntry, Package>,
    IHaveInputType<INuGetMetadataEntry>,
    IHaveOutputType<Package>
  {
    public Package Convert(INuGetMetadataEntry input) => new Package()
    {
      DeletedDate = input.DeletedDate,
      Version = input.PackageIdentity.Version.NormalizedVersion,
      Name = input.PackageIdentity.Name.DisplayName,
      Id = input.PackageIdentity.Name.NormalizedName,
      PermanentlyDeletedDate = input.PermanentDeletedDate,
      SourceChain = input.SourceChain,
      Listed = new bool?(input.Listed)
    };
  }
}
