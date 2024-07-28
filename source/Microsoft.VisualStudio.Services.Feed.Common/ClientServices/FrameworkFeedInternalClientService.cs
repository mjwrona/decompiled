// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.FrameworkFeedInternalClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  public class FrameworkFeedInternalClientService : IFeedInternalClientService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedByIdForAnyScopeAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      bool includeSoftDeletedFeeds = false)
    {
      FeedInternalHttpClient client = requestContext.GetClient<FeedInternalHttpClient>();
      client.ExcludeUrlsHeader = true;
      return client.GetFeedByIdForAnyScopeAsync(feedId, new bool?(includeSoftDeletedFeeds));
    }
  }
}
