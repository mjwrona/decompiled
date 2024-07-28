// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenMetadataEntryToPackageConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenMetadataEntryToPackageConverter : 
    IConverter<IMavenMetadataEntry, Package>,
    IHaveInputType<IMavenMetadataEntry>,
    IHaveOutputType<Package>
  {
    public Package Convert(IMavenMetadataEntry input) => new Package()
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
