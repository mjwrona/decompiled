// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ReleasePipeline.ReleasePipelineAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ReleasePipeline, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CA4476C-0395-4FD1-8BC2-591304FD4DB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.ReleasePipeline.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.ReleasePipeline
{
  public class ReleasePipelineAreaRegistration : AreaRegistration
  {
    public override string AreaName => "ReleasePipeline";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("ReleasePipeline/Scripts", "ReleasePipeline/Scripts/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => ReleasePipelineResources.ResourceManager));
  }
}
