// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.Charts.FeaturesChartsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 49538BC1-A38B-4EF6-AE82-2B8AD0FFF17F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.Charts.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.Charts
{
  public class FeaturesChartsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Charts";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterContributedPath("Charts");
      ScriptRegistration.RegisterContributedPath("Charts/Resources", ContributionPathType.Resource);
      ScriptRegistration.RegisterBundledArea("Charts", "Charts/Resources", "VSS").RegisterResource("Charts", (Func<ResourceManager>) (() => ChartsResources.Manager));
    }
  }
}
