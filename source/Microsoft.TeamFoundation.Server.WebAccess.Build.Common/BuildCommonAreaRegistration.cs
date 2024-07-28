// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7070C0C9-4072-4936-9D1D-0101911A5F41
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build.Common
{
  public class BuildCommonAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Build.Common";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => BuildCommonResources.ResourceManager), "TFS");
      BuiltinPluginManager.RegisterPlugin("Build.Common/Scripts/Registration.Artifacts", "TFS.OM.Common");
      BuiltinPluginManager.RegisterPluginBase("TFS.Build.Common", "Build.Common/Scripts/");
    }
  }
}
