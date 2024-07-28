// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Readme.NpmReadmeUtils
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog.AdditionalObjects;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Readme
{
  public class NpmReadmeUtils
  {
    public async Task<PackageFileMetadata> StoreReadme(
      IVssRequestContext requestContext,
      Guid feedId,
      NpmPackageName packageName,
      SemanticVersion packageVersion,
      string readmeMdPath,
      byte[] readmeMdBytes)
    {
      ArgumentUtility.CheckForNull<byte[]>(readmeMdBytes, nameof (readmeMdBytes));
      IElevatedBlobStore service = requestContext.GetService<IElevatedBlobStore>();
      IdBlobReference packageBlobReference = NpmBlobUtils.GetFileInPackageBlobReference(feedId, packageName, packageVersion, readmeMdPath);
      BlobIdentifier readmeBlobId = readmeMdBytes.CalculateBlobIdentifier((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance);
      using (MemoryStream ms = new MemoryStream(readmeMdBytes))
        await service.PutBlobAndReferenceAsync(requestContext, WellKnownDomainIds.OriginalDomainId, readmeBlobId, (Stream) ms, new BlobReference(packageBlobReference));
      PackageFileMetadata packageFileMetadata = new PackageFileMetadata()
      {
        BlobIdentifier = readmeBlobId
      };
      readmeBlobId = (BlobIdentifier) null;
      return packageFileMetadata;
    }

    internal static bool IsReadmeFilePath(string filePath) => filePath.EndsWith("/readme.md", StringComparison.OrdinalIgnoreCase) && filePath.IndexOf('/', 0, filePath.Length - "/readme.md".Length) < 0;
  }
}
