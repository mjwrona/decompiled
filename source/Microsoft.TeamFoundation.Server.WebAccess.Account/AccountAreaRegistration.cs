// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.AccountAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Resources;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class AccountAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Account";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      RouteTable.Routes.MapTfsRoute("Account_home_default", TeamFoundationHostType.Deployment, "", (object) new
      {
        controller = "Account",
        action = "Home",
        id = UrlParameter.Optional
      });
      RouteTable.Routes.MapTfsRoute("Account_go", TeamFoundationHostType.Deployment, "go/account", (object) new
      {
        controller = "Account",
        action = "Go",
        id = UrlParameter.Optional
      });
      ScriptRegistration.RegisterBundledArea("Account", (Func<ResourceManager>) (() => AccountResources.ResourceManager), "TFS");
      BuiltinPluginManager.RegisterPluginBase("TFS.Account", "Account/Scripts/");
      RouteTable.Routes.MapTfsRoute("Account_createProject", TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, "_createproject", (object) new
      {
        controller = "AccountHome",
        action = "CreateProject",
        id = UrlParameter.Optional,
        routeArea = ""
      });
      RouteTable.Routes.MapTfsRoute("Account_createDefaultProject", TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, "_createdefaultproject", (object) new
      {
        controller = "AccountHome",
        action = "CreateDefaultProject",
        id = UrlParameter.Optional,
        routeArea = ""
      });
    }
  }
}
