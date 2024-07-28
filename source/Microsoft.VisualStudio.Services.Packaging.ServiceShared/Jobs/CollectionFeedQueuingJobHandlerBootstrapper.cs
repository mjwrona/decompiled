// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.CollectionFeedQueuingJobHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class CollectionFeedQueuingJobHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<TeamFoundationJobDefinition, JobResult>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFeedJobQueuer jobQueuer;
    private readonly IProtocol protocol;
    private readonly IRandomProvider rng;
    private readonly IRegistryService registryService;
    private readonly Guid jobId;

    public CollectionFeedQueuingJobHandlerBootstrapper(
      IVssRequestContext requestContext,
      IFeedJobQueuer jobQueuer,
      IProtocol protocol,
      IRandomProvider rng,
      IRegistryService registryService,
      Guid jobId)
    {
      this.requestContext = requestContext;
      this.jobQueuer = jobQueuer;
      this.protocol = protocol;
      this.rng = rng;
      this.registryService = registryService;
      this.jobId = jobId;
    }

    public IAsyncHandler<TeamFoundationJobDefinition, JobResult> Bootstrap() => new JobErrorHandlerBootstrapper<TeamFoundationJobDefinition>(this.requestContext, (IAsyncHandler<TeamFoundationJobDefinition, JobResult>) new CollectionFeedQueuingJobHandler(this.jobQueuer, this.protocol, (IFeedService) new FeedServiceFacade(this.requestContext), this.requestContext.GetTracerFacade(), this.registryService, this.rng), (JobId) this.jobId).Bootstrap();
  }
}
