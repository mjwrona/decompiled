// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.RootAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  public class RootAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Root";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea("Presentation/Scripts/TFS", "Presentation/Scripts/TFS/Resources", "TFS").RegisterResource("Presentation", (Func<ResourceManager>) (() => PresentationResources.ResourceManager)).RegisterResource("Discussion", (Func<ResourceManager>) (() => DiscussionResources.ResourceManager)).RegisterResource("Configurations", (Func<ResourceManager>) (() => ConfigurationsResources.ResourceManager));
      BuiltinPluginManager.RegisterPluginBase("VSS", "VSS/");
      BuiltinPluginManager.RegisterPluginBase("TFS.Resources", "_api/_ScriptResource/Module/");
      BuiltinPluginManager.RegisterPluginBase("CommonConstants", "VSS/WebApi");
      BuiltinPluginManager.RegisterPluginBase("TFS.WorkItemTracking.Constants", "Presentation/Scripts/TFS/Generated/");
      BuiltinPluginManager.RegisterPluginBase("TFS.TestManagement.Constants", "Presentation/Scripts/TFS/Generated/");
    }
  }
}
