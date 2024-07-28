// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents.FeaturesWidgetComponentsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 01430B34-4C00-4D23-8456-39ADD60E691C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.WidgetComponents
{
  public class FeaturesWidgetComponentsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "WidgetComponents";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterContributedPath("WidgetComponents");
      ScriptRegistration.RegisterContributedPath("WidgetComponents/Resources", ContributionPathType.Resource);
      ScriptRegistration.RegisterBundledArea("WidgetComponents", "WidgetComponents/Resources", "VSS").RegisterResource("WidgetComponents", (Func<ResourceManager>) (() => WidgetComponentsResources.Manager));
    }
  }
}
