// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.Engagement.FeaturesAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.Engagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F839794C-E4E6-4865-B421-7156B186B208
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.Engagement.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.Engagement
{
  public class FeaturesAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Engagement";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("Engagement", "Engagement/Resources", "VSS").RegisterResource("Engagement", (Func<ResourceManager>) (() => EngagementResources.ResourceManager));
  }
}
