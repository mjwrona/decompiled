// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.HttpHandlers.PyPiUploadRouteHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.PyPi.Server.Ingestion;
using System.Web;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.PyPi.Server.HttpHandlers
{
  public class PyPiUploadRouteHandler : IRouteHandler
  {
    public IHttpHandler GetHttpHandler(RequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = ((VisualStudioServicesApplication) requestContext.HttpContext.ApplicationInstance).VssRequestContext;
      return (IHttpHandler) new PyPiUploadHttpHandler(vssRequestContext.GetService<IFeedCacheService>(), new PyPiPackageIngesterBootstrapper(vssRequestContext).Bootstrap());
    }
  }
}
