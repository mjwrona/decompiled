// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata.INpmMetadataService
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata
{
  public interface INpmMetadataService : 
    IMetadataDocumentService<
    #nullable disable
    NpmPackageIdentity, INpmMetadataEntry>,
    IMetadataService<
    #nullable enable
    NpmPackageIdentity, INpmMetadataEntry>,
    IReadMetadataService<NpmPackageIdentity, INpmMetadataEntry>,
    IReadSingleVersionMetadataService<NpmPackageIdentity, INpmMetadataEntry>,
    IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>,
    IReadMetadataDocumentService
  {
  }
}
