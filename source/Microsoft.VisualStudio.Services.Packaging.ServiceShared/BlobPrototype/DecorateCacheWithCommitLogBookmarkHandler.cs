// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.DecorateCacheWithCommitLogBookmarkHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class DecorateCacheWithCommitLogBookmarkHandler : 
    IAsyncHandler<ICommitLogEntry>,
    IAsyncHandler<ICommitLogEntry, NullResult>,
    IHaveInputType<ICommitLogEntry>,
    IHaveOutputType<NullResult>
  {
    private readonly IConverter<CommitLogBookmark, string> commitBookmarkSerializingConverter;
    private readonly ICache<string, object> cache;

    public DecorateCacheWithCommitLogBookmarkHandler(
      IConverter<CommitLogBookmark, string> commitBookmarkSerializingConverter,
      ICache<string, object> cache)
    {
      this.commitBookmarkSerializingConverter = commitBookmarkSerializingConverter;
      this.cache = cache;
    }

    public Task<NullResult> Handle(ICommitLogEntry commitLogEntry)
    {
      if (commitLogEntry == null)
        return Task.FromResult<NullResult>((NullResult) null);
      this.cache.Set("Packaging.OperationId", (object) this.commitBookmarkSerializingConverter.Convert(commitLogEntry.GetCommitLogBookmark()));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
