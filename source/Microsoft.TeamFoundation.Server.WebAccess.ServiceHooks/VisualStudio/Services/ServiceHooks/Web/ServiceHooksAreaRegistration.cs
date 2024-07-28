// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Web.ServiceHooksAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ServiceHooks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0ADA66F7-C61B-45D2-A394-67E5BF762451
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ServiceHooks.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.ServiceHooks;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Web
{
  public class ServiceHooksAreaRegistration : AreaRegistration
  {
    public override string AreaName => "ServiceHooks";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("ServiceHooks", "ServiceHooks/Resources", "VSS").RegisterResource("ServiceHooks", (Func<ResourceManager>) (() => ServiceHooksResources.ResourceManager));
  }
}
