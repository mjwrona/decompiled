// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.RegisterGitRoutes
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class RegisterGitRoutes
  {
    private const int WebAccessExceptionEaten = 599999;

    public static void RegisterGitRoutesInit()
    {
      RouteValueDictionary defaults = new RouteValueDictionary();
      defaults["controller"] = (object) "Git";
      defaults["action"] = (object) "index";
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      GitRouteRegistration.RegisterWebGitRoutes(GitRouteType.WebAccess, "Action_Parameters", string.Format("/{{{0}}}/{{*{1}}}", (object) "action", (object) "parameters"), defaults, (RouteValueDictionary) null, (IRouteHandler) new MvcRouteHandler(), RegisterGitRoutes.\u003C\u003EO.\u003C0\u003E__CreateWebAccessGitRoute ?? (RegisterGitRoutes.\u003C\u003EO.\u003C0\u003E__CreateWebAccessGitRoute = new GitRouteRegistration.InstantiateRouteMethod(RegisterGitRoutes.CreateWebAccessGitRoute)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      GitRouteRegistration.RegisterWebGitRoutes(GitRouteType.WebAccess, "Action", string.Format("/{{{0}}}", (object) "action"), defaults, (RouteValueDictionary) null, (IRouteHandler) new MvcRouteHandler(), RegisterGitRoutes.\u003C\u003EO.\u003C0\u003E__CreateWebAccessGitRoute ?? (RegisterGitRoutes.\u003C\u003EO.\u003C0\u003E__CreateWebAccessGitRoute = new GitRouteRegistration.InstantiateRouteMethod(RegisterGitRoutes.CreateWebAccessGitRoute)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      GitRouteRegistration.RegisterWebGitRoutes(GitRouteType.WebAccess, "DefaultAction", "", defaults, (RouteValueDictionary) null, (IRouteHandler) new MvcRouteHandler(), RegisterGitRoutes.\u003C\u003EO.\u003C0\u003E__CreateWebAccessGitRoute ?? (RegisterGitRoutes.\u003C\u003EO.\u003C0\u003E__CreateWebAccessGitRoute = new GitRouteRegistration.InstantiateRouteMethod(RegisterGitRoutes.CreateWebAccessGitRoute)));
    }

    private static Route CreateWebAccessGitRoute(
      string address,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      RouteValueDictionary tokens,
      IRouteHandler handler)
    {
      return (Route) new TfsRoute(TeamFoundationHostType.ProjectCollection, address, defaults, constraints, tokens, handler);
    }
  }
}
