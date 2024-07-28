// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.UserManagementAreaRegistration
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Resources;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public class UserManagementAreaRegistration : AreaRegistration
  {
    public override string AreaName => "UserManagement";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      this.RegisterRoutes(context.Routes);
      ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => UserManagementResources.ResourceManager), "SPS");
    }

    private void RegisterRoutes(RouteCollection routes) => routes.MapViewRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, "Account_users", "_users", (object) new
    {
      controller = "Users",
      action = "Index",
      id = UrlParameter.Optional
    }, (object) null, new string[1]
    {
      "Microsoft.VisualStudio.Services.Web.UserManagement"
    });
  }
}
