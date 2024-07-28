// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Charting.ChartingAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Charting, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E26871-FBED-4ABC-AC4C-33E090B65836
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Charting.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Charting
{
  public class ChartingAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Charting";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("Charting", (Func<ResourceManager>) (() => ChartingResources.Manager), "TFS");
  }
}
