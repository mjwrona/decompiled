// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupInfoRestoringRetriever
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DedupInfoRestoringRetriever : IDedupInfoRetriever
  {
    private IDedupInfoRetriever retriever;
    private IDedupRestorer restorer;

    public DedupInfoRestoringRetriever(IDedupInfoRetriever retriever, IDedupRestorer restorer)
    {
      this.retriever = retriever;
      this.restorer = restorer;
    }

    public async Task<IDedupInfo> GetDedupInfoAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      IDedupInfo dedupInfoAsync = await this.retriever.GetDedupInfoAsync(processor, dedupId).ConfigureAwait(false);
      if (dedupInfoAsync.Status == HealthStatus.Absent)
      {
        if (!await this.restorer.RestoreIfNotExists(processor, dedupId).ConfigureAwait(false))
          throw new InvalidOperationException(string.Format("Unable to restore dedupId {0}", (object) dedupId));
        dedupInfoAsync = await this.retriever.GetDedupInfoAsync(processor, dedupId).ConfigureAwait(false);
      }
      return dedupInfoAsync;
    }

    public async Task<DedupNode?> GetDedupNodeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      NodeDedupIdentifier nodeId)
    {
      DedupNode? dedupNodeAsync = await this.retriever.GetDedupNodeAsync(processor, nodeId).ConfigureAwait(false);
      if (!dedupNodeAsync.HasValue)
      {
        int num = await this.restorer.RestoreIfNotExists(processor, (DedupIdentifier) nodeId).ConfigureAwait(false) ? 1 : 0;
        dedupNodeAsync = await this.retriever.GetDedupNodeAsync(processor, nodeId).ConfigureAwait(false);
      }
      return dedupNodeAsync;
    }
  }
}
