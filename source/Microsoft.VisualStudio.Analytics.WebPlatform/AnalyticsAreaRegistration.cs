// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Analytics.WebPlatform.AnalyticsAreaRegistration
// Assembly: Microsoft.VisualStudio.Analytics.WebPlatform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DA6B9D87-1232-44CA-8EC9-599418A96267
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Analytics.WebPlatform.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Analytics.WebPlatform
{
  public class AnalyticsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Analytics";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, this.AreaName + "/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => AnalyticsResources.Manager));
  }
}
