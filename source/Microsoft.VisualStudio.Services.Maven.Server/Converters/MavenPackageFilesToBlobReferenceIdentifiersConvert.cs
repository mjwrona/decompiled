// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Converters.MavenPackageFilesToBlobReferenceIdentifiersConverter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Converters
{
  public class MavenPackageFilesToBlobReferenceIdentifiersConverter : 
    IConverter<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>, IEnumerable<BlobReferenceIdentifier>>,
    IHaveInputType<IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>>>,
    IHaveOutputType<IEnumerable<BlobReferenceIdentifier>>
  {
    public IEnumerable<BlobReferenceIdentifier> Convert(
      IPackageNameRequest<MavenPackageName, IEnumerable<MavenPackageFileNew>> input)
    {
      foreach (MavenPackageFileNew mavenPackageFileNew in input.AdditionalData)
      {
        if (mavenPackageFileNew.StorageId is BlobStorageId storageId)
        {
          IdBlobReference blobReferenceId = MavenPackageFileUtility.GetBlobReferenceId(input.Feed.Id, input.PackageName, mavenPackageFileNew.Path);
          yield return new BlobReferenceIdentifier(storageId.BlobId, blobReferenceId.Name, blobReferenceId.Scope);
        }
      }
    }
  }
}
