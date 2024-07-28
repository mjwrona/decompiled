// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub.WorkItemsHubAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EF10A6C0-53C4-4480-9084-156ADE56D4B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemsHub
{
  public class WorkItemsHubAreaRegistration : AreaRegistration
  {
    public override string AreaName => "WorkItemsHub";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => WorkItemsHubResources.ResourceManager), "TFS");
  }
}
