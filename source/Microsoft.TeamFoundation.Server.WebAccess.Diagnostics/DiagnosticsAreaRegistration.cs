// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.DiagnosticsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics
{
  public class DiagnosticsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Diagnostics";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterArea("Diagnostics", (Func<ResourceManager>) (() => DiagnosticsResources.ResourceManager));
      BuiltinPluginManager.RegisterPluginBase("TFS.Diag", "Diagnostic/Scripts/");
    }
  }
}
