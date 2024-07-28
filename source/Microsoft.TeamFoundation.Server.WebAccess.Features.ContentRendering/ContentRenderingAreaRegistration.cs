// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering.ContentRenderingAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78AC3D55-06D3-4434-8BC6-2E2C9E46022A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.ContentRendering
{
  public class ContentRenderingAreaRegistration : AreaRegistration
  {
    public override string AreaName => "ContentRendering";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterContributedPath("ContentRendering");
      ScriptRegistration.RegisterContributedPath("ContentRendering/Resources", ContributionPathType.Resource);
      ScriptRegistration.RegisterBundledArea("ContentRendering", "ContentRendering/Resources", "VSS").RegisterResource("ContentRendering", (Func<ResourceManager>) (() => ContentRenderingResources.ResourceManager));
    }
  }
}
