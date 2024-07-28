// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Readme.GetReadmeBlobIdentifierHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Readme
{
  public class GetReadmeBlobIdentifierHandler : 
    IAsyncHandler<
    #nullable disable
    IPackageRequest<NpmPackageIdentity>, BlobIdentifier>,
    IHaveInputType<IPackageRequest<NpmPackageIdentity>>,
    IHaveOutputType<BlobIdentifier>
  {
    private readonly INpmMetadataService metadataService;

    public GetReadmeBlobIdentifierHandler(INpmMetadataService metadataService) => this.metadataService = metadataService;

    public async Task<BlobIdentifier> Handle(IPackageRequest<NpmPackageIdentity> request)
    {
      KeyValuePair<string, PackageFileMetadata>[] array = ((await this.metadataService.GetPackageVersionStateAsync(request))?.PackageManifest?.FilesMetadata ?? new Dictionary<string, PackageFileMetadata>()).Where<KeyValuePair<string, PackageFileMetadata>>((Func<KeyValuePair<string, PackageFileMetadata>, bool>) (x => GetReadmeBlobIdentifierHandler.IsReadmeFilePath(x.Key))).ToArray<KeyValuePair<string, PackageFileMetadata>>();
      if (array.Length != 1)
        throw new FileNotFoundInPackageException(Resources.Error_ReadmeNotFoundPackageVersion((object) request.PackageId.Name, (object) request.PackageId.Version));
      return array[0].Value.BlobIdentifier;
    }

    internal static bool IsReadmeFilePath(string filePath) => filePath.EndsWith("/readme.md", StringComparison.OrdinalIgnoreCase) && filePath.IndexOf('/', 0, filePath.Length - "/readme.md".Length) < 0;
  }
}
