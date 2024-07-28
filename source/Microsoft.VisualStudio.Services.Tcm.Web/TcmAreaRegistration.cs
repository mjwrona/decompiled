// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.Web.TcmAreaRegistration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78439952-D4CA-4096-B285-270E3917D35C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Tcm.Web
{
  public class TcmAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Tcm";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterArea("Tcm", (Func<ResourceManager>) (() => TcmResources.ResourceManager));
  }
}
