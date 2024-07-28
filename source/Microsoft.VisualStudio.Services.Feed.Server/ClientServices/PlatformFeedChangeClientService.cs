// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ClientServices.PlatformFeedChangeClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.ClientServices
{
  public class PlatformFeedChangeClientService : IFeedChangeClientService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<FeedChangesResponse> GetFeedChangesAsync(
      IVssRequestContext requestContext,
      long continuationToken = 0,
      int batchSize = 1000,
      bool includeDeleted = false)
    {
      return await requestContext.GetService<IFeedChangeService>().GetFeedChanges(requestContext, (ProjectReference) null, includeDeleted, continuationToken, batchSize).ToResponseAsync(requestContext, batchSize);
    }
  }
}
