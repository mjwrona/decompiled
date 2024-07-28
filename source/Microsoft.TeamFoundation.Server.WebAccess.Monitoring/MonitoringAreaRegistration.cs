// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.MonitoringAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring
{
  public class MonitoringAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Monitoring";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterArea(this.AreaName, (Func<ResourceManager>) (() => MonitoringResources.ResourceManager));
      BuiltinPluginManager.RegisterPluginBase("TFS.Monitoring", "Monitoring/Scripts/");
    }
  }
}
