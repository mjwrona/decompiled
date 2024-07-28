// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.HttpHandlers.NuGetV2PushRouteHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion;
using System.Web;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.NuGet.Server.HttpHandlers
{
  public class NuGetV2PushRouteHandler : IRouteHandler
  {
    public IHttpHandler GetHttpHandler(RequestContext requestContext) => (IHttpHandler) new NuGetV2PushHttpHandler((IFileStreamFactory) new FileStreamFactory(), ((VisualStudioServicesApplication) requestContext.HttpContext.ApplicationInstance).VssRequestContext.GetService<IPackageIngestionService>());
  }
}
