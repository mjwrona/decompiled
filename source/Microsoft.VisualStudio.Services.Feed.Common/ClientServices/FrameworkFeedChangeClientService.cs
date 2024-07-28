// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.FrameworkFeedChangeClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  public class FrameworkFeedChangeClientService : IFeedChangeClientService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Task<FeedChangesResponse> GetFeedChangesAsync(
      IVssRequestContext requestContext,
      long continuationToken = 0,
      int batchSize = 1000,
      bool includeDeleted = false)
    {
      return requestContext.GetClient<FeedHttpClient>().GetFeedChangesAsync(new bool?(includeDeleted), new long?(continuationToken), new int?(batchSize));
    }
  }
}
