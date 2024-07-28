// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Wiki.WikiAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Wiki, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02A6F20-36DA-4F23-A6E1-EB15CA1D3A8A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Wiki.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Wiki
{
  public class WikiAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Wiki";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => WikiResources.ResourceManager), "TFS");
  }
}
