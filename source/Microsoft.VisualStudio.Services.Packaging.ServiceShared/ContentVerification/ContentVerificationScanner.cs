// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.ContentVerificationScanner
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public class ContentVerificationScanner : IContentVerificationScanner
  {
    private readonly IAsyncHandler<IContentVerificationRequest, NullResult> protocolSpecificContentScanner;

    public ContentVerificationScanner(
      IAsyncHandler<IContentVerificationRequest, NullResult> scanner)
    {
      this.protocolSpecificContentScanner = scanner;
    }

    public async Task ScanCommitAsync(
      CollectionId collectionId,
      FeedCore feed,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      foreach (ICommitLogEntry commitLogEntry in (IEnumerable<ICommitLogEntry>) commitLogEntries)
      {
        if (commitLogEntry.CommitOperationData is IAddOperationData)
        {
          NullResult nullResult = await this.protocolSpecificContentScanner.Handle((IContentVerificationRequest) new ContentVerificationRequest(collectionId, feed, commitLogEntry));
        }
      }
    }
  }
}
