// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkCustomization.CustomizationAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkCustomization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 078B6855-7C4F-46A0-9EC7-2D1DE7AA93DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkCustomization.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkCustomization
{
  public class CustomizationAreaRegistration : AreaRegistration
  {
    public override string AreaName { get; } = "WorkCustomization";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => WorkCustomizationResources.ResourceManager), "TFS");
  }
}
