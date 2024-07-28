// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Dashboards.DashboardsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Dashboards, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1A53BFE3-D2EE-4259-A1B0-9683B82268B4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Dashboards.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Dashboards
{
  public class DashboardsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Dashboards";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("Dashboards", (Func<ResourceManager>) (() => DashboardsResources.Manager), "TFS");
  }
}
