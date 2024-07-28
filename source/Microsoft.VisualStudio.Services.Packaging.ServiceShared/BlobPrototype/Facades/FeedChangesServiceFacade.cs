// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.FeedChangesServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class FeedChangesServiceFacade : IFeedChangesService
  {
    private readonly IVssRequestContext requestContext;
    private readonly IFeedChangeClientService feedChangeService;

    public FeedChangesServiceFacade(
      IVssRequestContext requestContext,
      IFeedChangeClientService feedChangeService)
    {
      this.requestContext = requestContext;
      this.feedChangeService = feedChangeService;
    }

    public Task<FeedChangesResponse> GetFeedChangesAsync(
      long continuationToken = 0,
      int batchSize = 1000,
      bool includeDeleted = false)
    {
      return this.feedChangeService.GetFeedChangesAsync(this.requestContext, continuationToken, batchSize, includeDeleted);
    }
  }
}
