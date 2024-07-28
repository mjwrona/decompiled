// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.FrameworkCachedPackagesInternalClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  public class FrameworkCachedPackagesInternalClientService : 
    ICachedPackagesInternalClientService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Task ClearCachedPackagesAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string upstreamId)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().ClearPackageCacheAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, upstreamId);
    }

    public IEnumerable<string> GetCachedPackages(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => throw new NotSupportedException();
  }
}
