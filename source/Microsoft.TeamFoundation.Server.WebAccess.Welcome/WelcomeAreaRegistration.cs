// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Welcome.WelcomeAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Welcome, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B024A61-082C-4505-8523-CF030F6A8A5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Welcome.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Welcome
{
  public class WelcomeAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Welcome";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterArea(this.AreaName, (Func<ResourceManager>) (() => WelcomeResources.ResourceManager));
  }
}
