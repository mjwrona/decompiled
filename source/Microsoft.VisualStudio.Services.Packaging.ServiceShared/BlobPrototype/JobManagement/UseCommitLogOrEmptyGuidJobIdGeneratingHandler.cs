// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.UseCommitLogOrEmptyGuidJobIdGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class UseCommitLogOrEmptyGuidJobIdGeneratingHandler : 
    IAsyncHandler<FeedRequest<JobType>, Guid>,
    IHaveInputType<FeedRequest<JobType>>,
    IHaveOutputType<Guid>
  {
    private readonly ICommitLogEndpointReader commitLogReader;

    public UseCommitLogOrEmptyGuidJobIdGeneratingHandler(ICommitLogEndpointReader commitLogReader) => this.commitLogReader = commitLogReader;

    public async Task<Guid> Handle(FeedRequest<JobType> request)
    {
      if (request.AdditionalData != ChangeProcessingJobConstants.ChangeProcessingJobType)
        throw new Exception("unexpected: job id generating handler only supports change processing jobs.");
      CommitLogBookmark commitBookmarkAsync = await this.commitLogReader.GetOldestCommitBookmarkAsync(request.Feed);
      return !(commitBookmarkAsync != CommitLogBookmark.Empty) ? Guid.Empty : commitBookmarkAsync.CommitId.ToGuid();
    }
  }
}
