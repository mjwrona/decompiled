// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher.IDedupContentProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E75C933D-C085-4E42-931C-50E8D8D54917
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher
{
  public interface IDedupContentProvider
  {
    Task<DedupCompressedBuffer> GetContentAsync(DedupDownloadInfoBase info);

    Task<DedupDownloadInfo> GetDownloadInfoAsync(IDomainId domainId, DedupIdentifier dedupId);
  }
}
