// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class AdminAreaRegistration : AreaRegistration
  {
    private const string UserManagementflagName = "WebAccess.UserManagement";

    public override string AreaName => "Admin";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      BuiltinPluginManager.RegisterPlugin("Admin/Scripts/TFS.Admin.Registration.HostPlugins", "TFS.Host.UI");
      BuiltinPluginManager.RegisterPlugin("Admin/Scripts/TFS.Admin.Registration.HostPlugins", "TFS.Host.UI.AccountHomeView");
      BuiltinPluginManager.RegisterPluginBase("TFS.Admin", "Admin/Scripts/");
      ScriptRegistration.RegisterBundledArea("Admin/Scripts", "Admin/Scripts/Resources", "TFS").RegisterResource("Admin", (Func<ResourceManager>) (() => AdminResources.ResourceManager)).RegisterResource("Admin.Navigation", (Func<ResourceManager>) (() => AdminNavigationResources.ResourceManager));
    }

    private static bool HasTeamContext(TfsWebContext tfsWebContext)
    {
      ArgumentUtility.CheckForNull<TfsWebContext>(tfsWebContext, nameof (tfsWebContext));
      return tfsWebContext.NavigationContext.Team != null;
    }
  }
}
