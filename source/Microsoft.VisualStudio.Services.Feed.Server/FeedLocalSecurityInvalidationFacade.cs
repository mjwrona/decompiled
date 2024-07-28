// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedLocalSecurityInvalidationFacade
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class FeedLocalSecurityInvalidationFacade
  {
    public virtual void InvalidateSystemStore(IVssRequestContext requestContext, string reason) => requestContext.GetService<LocalSecurityInvalidationService>().InvalidateSystemStore(requestContext, reason);
  }
}
