// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Widgets.WidgetsAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Widgets, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DD4C24BB-2646-4C82-B0E8-494FC53AC01D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Widgets.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Widgets
{
  public class WidgetsAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Widgets";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("Widgets", (Func<ResourceManager>) (() => WidgetsResources.Manager), "TFS");
  }
}
