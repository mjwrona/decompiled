// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedCapabilitiesService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedCapabilitiesService))]
  public interface IFeedCapabilitiesService : IVssFrameworkService
  {
    FeedCapabilities GetCapabilities(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedId);

    Task<bool> UpdateCapabilityAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedId,
      FeedCapabilities capabilities);

    void EndFeedUpgrade(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed);
  }
}
