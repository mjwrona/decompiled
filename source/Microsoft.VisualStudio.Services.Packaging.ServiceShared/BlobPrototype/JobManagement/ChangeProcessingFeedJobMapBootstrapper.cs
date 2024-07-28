// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.ChangeProcessingFeedJobMapBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class ChangeProcessingFeedJobMapBootstrapper : IBootstrapper<IFeedJobIdMap>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICommitLogEndpointReader commitLogReader;

    public ChangeProcessingFeedJobMapBootstrapper(
      IVssRequestContext requestContext,
      ICommitLogEndpointReader commitLogReader)
    {
      this.requestContext = requestContext;
      this.commitLogReader = commitLogReader;
      requestContext.CheckProjectCollectionRequestContext();
    }

    public IFeedJobIdMap Bootstrap() => (IFeedJobIdMap) new BlobBasedFeedJobIdMap(BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap(), this.requestContext.GetExecutionEnvironmentFacade(), (IAsyncHandler<FeedRequest<JobType>, Guid>) new UseCommitLogOrEmptyGuidJobIdGeneratingHandler(this.commitLogReader), (ISerializer<FeedJobMapEntry>) new JobMapEntrySerializer((ISerializer<FeedJobMapEntry>) new JsonSerializer<FeedJobMapEntry>(new JsonSerializerSettings()
    {
      MissingMemberHandling = MissingMemberHandling.Ignore
    })), ChangeProcessingJobConstants.ChangeProcessingJobType);
  }
}
