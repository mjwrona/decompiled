// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.HttpHandlers.MavenPublishRouteHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Ingestion;
using System.Web;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Maven.Server.HttpHandlers
{
  public class MavenPublishRouteHandler : IRouteHandler
  {
    public IHttpHandler GetHttpHandler(RequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = ((VisualStudioServicesApplication) requestContext.HttpContext.ApplicationInstance).VssRequestContext;
      return (IHttpHandler) new MavenPublishHttpHandler(vssRequestContext.GetService<IFeedCacheService>(), new MavenPackageIngesterBootstrapper(vssRequestContext).Bootstrap());
    }
  }
}
