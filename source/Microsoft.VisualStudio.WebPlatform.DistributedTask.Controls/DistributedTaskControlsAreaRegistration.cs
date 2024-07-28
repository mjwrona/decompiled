// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.WebPlatform.DistributedTask.Controls.DistributedTaskControlsAreaRegistration
// Assembly: Microsoft.VisualStudio.WebPlatform.DistributedTask.Controls, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E35CC858-57FC-4735-A3D3-EAA6F50485DC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.WebPlatform.DistributedTask.Controls.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.WebPlatform.DistributedTask.Controls
{
  public class DistributedTaskControlsAreaRegistration : AreaRegistration
  {
    private const string c_baseModulePath = "DistributedTaskControls";

    public override string AreaName => "DistributedTaskControls";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("DistributedTaskControls", "DistributedTaskControls/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => DistributedTaskControlsResources.ResourceManager));
  }
}
