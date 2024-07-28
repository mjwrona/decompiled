// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.BlobBasedFeedJobIdMap
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Data.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class BlobBasedFeedJobIdMap : IFeedJobIdMap
  {
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFactory;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IAsyncHandler<FeedRequest<JobType>, Guid> defaultJobIdGeneratingHandler;
    private readonly ISerializer<FeedJobMapEntry> serializer;
    private readonly JobType jobType;

    public BlobBasedFeedJobIdMap(
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      IExecutionEnvironment executionEnvironment,
      IAsyncHandler<FeedRequest<JobType>, Guid> defaultJobIdGeneratingHandler,
      ISerializer<FeedJobMapEntry> serializer,
      JobType jobType)
    {
      this.blobServiceFactory = blobServiceFactory;
      this.executionEnvironment = executionEnvironment;
      this.defaultJobIdGeneratingHandler = defaultJobIdGeneratingHandler;
      this.serializer = serializer;
      this.jobType = jobType;
    }

    public async Task<FeedJobMapEntry> GetJobId(IFeedRequest feedRequest)
    {
      FeedCore feed = feedRequest.Feed;
      IProtocol protocol = feedRequest.Protocol;
      Locator path = this.GetPathTo(protocol, feed, this.jobType);
      IBlobService blobService = this.blobServiceFactory.Get(new ContainerAddress((CollectionId) this.executionEnvironment.HostId, new Locator(new string[1]
      {
        "jobidmappings"
      })));
      string str = (string) null;
      Random r = new Random();
      for (int iterations = 0; iterations < 10 && str == null; ++iterations)
      {
        if (iterations != 0)
          Thread.Sleep(500 + r.Next(1000));
        using (MemoryStream blobStream = new MemoryStream())
        {
          if (await blobService.GetBlobAsync(path, (Stream) blobStream) != null)
            return this.serializer.Deserialize((Stream) blobStream);
        }
        Guid guid = await this.defaultJobIdGeneratingHandler.Handle(new FeedRequest<JobType>(feedRequest, this.jobType));
        FeedJobMapEntry entry = new FeedJobMapEntry()
        {
          JobId = guid
        };
        if (guid == Guid.Empty)
          return entry;
        str = await blobService.PutBlobAsync(path, this.serializer.Serialize(entry).AsArraySegment(), (string) null);
        if (str != null)
          return entry;
        entry = (FeedJobMapEntry) null;
      }
      throw new ChangeConflictException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_RequiredJobIdNotFound((object) feed.Id, (object) protocol, (object) this.jobType.Name, (object) this.jobType.Id));
    }

    private Locator GetPathTo(IProtocol protocol, FeedCore feed, JobType jobType) => new Locator(new string[3]
    {
      jobType.Id.ToString(),
      protocol.ToString(),
      feed.Id.ToString() + ".txt"
    });
  }
}
