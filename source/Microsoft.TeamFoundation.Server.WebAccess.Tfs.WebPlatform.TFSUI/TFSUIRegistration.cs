// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI.TFSUIRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDA8DF6C-700E-486B-89AF-56EBC173977C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI
{
  public class TFSUIRegistration : AreaRegistration
  {
    public override string AreaName => "TFSUI";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterContributedPath("TFSUI");
      ScriptRegistration.RegisterContributedPath("TFSUI/Resources", ContributionPathType.Resource);
      ScriptRegistration.RegisterBundledArea("TFSUI", "TFSUI/Resources", "TFS").RegisterResource("TFSUI", (Func<ResourceManager>) (() => TFSUIResources.Manager));
    }
  }
}
