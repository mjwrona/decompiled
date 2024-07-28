// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.AlertsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  public class AlertsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Alerts";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea("Alerts", (Func<ResourceManager>) (() => AlertsResources.ResourceManager), "TFS");
      BuiltinPluginManager.RegisterPlugin("Alerts/Scripts/TFS.Alerts.Registration.HostPlugins", "TFS.Host.UI");
      BuiltinPluginManager.RegisterPluginBase("TFS.Alerts", "Alerts/Scripts/");
    }
  }
}
