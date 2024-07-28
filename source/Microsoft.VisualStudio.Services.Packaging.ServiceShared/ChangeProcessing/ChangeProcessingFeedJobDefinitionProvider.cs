// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.ChangeProcessingFeedJobDefinitionProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing
{
  public class ChangeProcessingFeedJobDefinitionProvider : IFeedJobDefinitionProvider
  {
    private readonly ICommitLog commitLog;

    public ChangeProcessingFeedJobDefinitionProvider(ICommitLog commitLog, string extension)
    {
      this.commitLog = commitLog;
      this.Extension = extension;
    }

    public string Extension { get; }

    public string JobPrefix => "CommitLogProcessor";

    public Guid GetJobId(IVssRequestContext requestContext, IFeedRequest feedRequest)
    {
      IFeedJobIdMap feedJobMap = new ChangeProcessingFeedJobMapBootstrapper(requestContext, (ICommitLogEndpointReader) this.commitLog).Bootstrap();
      return AsyncPump.Run<FeedJobMapEntry>((Func<Task<FeedJobMapEntry>>) (() => feedJobMap.GetJobId(feedRequest))).JobId;
    }
  }
}
