// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.IBlobStoreHttpClientExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class IBlobStoreHttpClientExtensions
  {
    private static readonly Task CompletedTask = (Task) Task.FromResult<int>(0);

    public static async Task<bool> TryReferenceWithBlocksAsync(
      this IBlobStoreHttpClient blobStore,
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdAndBlocks,
      BlobReference reference,
      CancellationToken cancellationToken)
    {
      return !(await blobStore.TryReferenceWithBlocksAsync((IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>) new Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>()
      {
        {
          blobIdAndBlocks,
          (IEnumerable<BlobReference>) new BlobReference[1]
          {
            reference
          }
        }
      }, cancellationToken).ConfigureAwait(false)).Any<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>>();
    }

    public static async Task<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> UploadFileAsync(
      this IBlobStoreHttpClient client,
      BlobIdentifier blobIdentifier,
      string filename,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(filename, nameof (filename));
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks identifierWithBlocks;
      using (FileStream stream = FileStreamUtils.OpenFileStreamForAsync(filename, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
        identifierWithBlocks = await client.UploadBlocksForBlobAsync(blobIdentifier, (Stream) stream, cancellationToken).ConfigureAwait(false);
      return identifierWithBlocks;
    }
  }
}
