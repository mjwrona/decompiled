// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Requirements.RequirementsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Requirements, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6C113FD4-8DA1-49E9-A859-47B7ED9A5698
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Requirements.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Requirements
{
  public class RequirementsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Requirements";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterArea("Requirements/Scripts", "Requirements/Scripts/Resources").RegisterResource("RequirementsFeedback", (Func<ResourceManager>) (() => FeedbackResources.ResourceManager));
      BuiltinPluginManager.RegisterPlugin("Requirements/Scripts/TFS.Requirements.Setup", "TFS.OM.Common");
      BuiltinPluginManager.RegisterPlugin("Requirements/Scripts/TFS.Requirements.Registration.HostPlugins", "TFS.Host.UI");
      BuiltinPluginManager.RegisterPluginBase("TFS.Requirements", "Requirements/Scripts/");
    }
  }
}
