// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ClientServices.PlatformFeedRecycleBinClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.ClientServices
{
  public class PlatformFeedRecycleBinClientService : 
    IFeedRecycleBinClientService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsFromRecycleBinAsync(
      IVssRequestContext requestContext,
      ProjectReference project = null)
    {
      return await requestContext.GetService<IFeedRecycleBinService>().GetFeedsFromRecycleBin(requestContext, project).IncludeUrlsAsync(requestContext);
    }
  }
}
