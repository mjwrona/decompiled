// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Search.SearchAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Search, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CC5F1FD-9493-4B23-B40F-49E474A0E625
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Search.dll

using Microsoft.TeamFoundation.Server.WebAccess.Search.Scenarios;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Search
{
  public class SearchAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Search";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => SearchResources.ResourceManager), "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => SearchResources.ResourceManager));
      ScriptRegistration.RegisterBundledArea(this.AreaName + "/Scenarios", (Func<ResourceManager>) (() => SearchScenarioResources.ResourceManager), "TFS").RegisterResource("Search.Scenarios", (Func<ResourceManager>) (() => SearchScenarioResources.ResourceManager));
    }
  }
}
