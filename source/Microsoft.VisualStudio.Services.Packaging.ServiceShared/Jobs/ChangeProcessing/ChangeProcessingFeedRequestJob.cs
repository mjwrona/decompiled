// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.ChangeProcessingFeedRequestJob
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public abstract class ChangeProcessingFeedRequestJob : FeedRequestJob
  {
    protected override IFactory<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>> DecorateHandler(
      IVssRequestContext requestContext,
      IAsyncHandler<IFeedRequest, JobResult> handler)
    {
      return (IFactory<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>>) new ReturnSameInstanceInputFactory<IFeedRequest, IAsyncHandler<IFeedRequest, JobResult>>(UntilNonNullHandler.Create<IFeedRequest, JobResult>((IAsyncHandler<IFeedRequest, JobResult>) new ReadOnlyAndBypassFeatureFlagCheckingJobHandler(requestContext.GetFeatureFlagFacade()), handler));
    }
  }
}
