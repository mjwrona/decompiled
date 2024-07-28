// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CommonAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class CommonAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Common";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("VSS", "VSS/Resources", "VSS").RegisterResource("TFSSeedFileResources", (Func<ResourceManager>) (() => TfsSeedFileResources.ResourceManager)).RegisterResource("Platform", (Func<ResourceManager>) (() => PlatformResources.ResourceManager)).RegisterResource("Common", (Func<ResourceManager>) (() => WACommonResources.ResourceManager));
  }
}
