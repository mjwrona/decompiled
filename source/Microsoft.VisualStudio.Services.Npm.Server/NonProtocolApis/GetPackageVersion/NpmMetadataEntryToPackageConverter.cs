// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.GetPackageVersion.NpmMetadataEntryToPackageConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.GetPackageVersion
{
  public class NpmMetadataEntryToPackageConverter : 
    IConverter<INpmMetadataEntry, Package>,
    IHaveInputType<INpmMetadataEntry>,
    IHaveOutputType<Package>
  {
    public Package Convert(INpmMetadataEntry input)
    {
      NpmPackageIdentity packageIdentity = input.PackageIdentity;
      return new Package()
      {
        DeprecateMessage = input.Deprecated,
        UnpublishedDate = input.DeletedDate,
        Version = packageIdentity.Version.NormalizedVersion,
        Name = packageIdentity.Name.FullName,
        Id = packageIdentity.Name.FullName,
        PermanentlyDeletedDate = input.PermanentDeletedDate,
        SourceChain = input.SourceChain
      };
    }
  }
}
