// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Web.AnalyticsAreaRegistration
// Assembly: Microsoft.VisualStudio.Services.Analytics.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 455612C1-A616-4BB6-B9F5-E94C097DFD14
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Analytics.Web
{
  public class AnalyticsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Views";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("Views", "Views/Scripts/Resources", "VSS").RegisterResource("Views", (Func<ResourceManager>) (() => AnalyticsResources.Manager));
  }
}
