// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.WebContextExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  public static class WebContextExtensions
  {
    private const string c_TeamFoundationRequestContext = "IVssRequestContext";

    public static Microsoft.TeamFoundation.Server.WebAccess.WebContext WebContext(
      this RequestContext requestContext)
    {
      return WebContextFactory.GetWebContext(requestContext);
    }

    public static Microsoft.TeamFoundation.Server.WebAccess.WebContext WebContext(
      this ViewContext viewContext)
    {
      return viewContext.RequestContext.WebContext();
    }
  }
}
