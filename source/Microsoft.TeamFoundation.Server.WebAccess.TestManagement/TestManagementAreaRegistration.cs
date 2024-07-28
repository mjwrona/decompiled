// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestManagementAreaRegistration : AreaRegistration
  {
    public override string AreaName => "TestManagement";

    private static bool ShouldRegisterHub(TfsWebContext tfsWebContext) => false;

    private static bool AreTestManagementExtensionAndAdvancedTestExtensionEnabled(
      TfsWebContext tfsWebContext)
    {
      return TestManagementAreaRegistration.IsAdvancedTestExtensionEnabled(tfsWebContext);
    }

    private static bool ShouldRegisterAdvancedHub(TfsWebContext tfsWebContext) => !TestManagementAreaRegistration.AreTestManagementExtensionAndAdvancedTestExtensionEnabled(tfsWebContext) && tfsWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.TestManagementId);

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => TestManagementResources.ResourceManager), "TFS");
      BuiltinPluginManager.RegisterPlugin("TestManagement/Scripts/TFS.TestManagement.Setup", "TFS.OM.Common");
    }

    internal static bool DoesUserHaveTestManagementAdvancedAccess(TfsWebContext tfsWebContext) => TestManagementAreaRegistration.IsAdvancedTestExtensionEnabled(tfsWebContext) || tfsWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.TestManagementId);

    internal static bool IsAdvancedTestExtensionEnabled(TfsWebContext tfsWebContext) => LicenseCheckHelper.IsAdvancedTestExtensionEnabled(tfsWebContext.TfsRequestContext);
  }
}
