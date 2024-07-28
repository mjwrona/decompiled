// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.WebApiVersionAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System.Reflection;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  public class WebApiVersionAttribute : ActionMethodSelectorAttribute
  {
    public int MinVersion { get; set; }

    public int MaxVersion { get; set; }

    public override bool IsValidForRequest(
      ControllerContext controllerContext,
      MethodInfo methodInfo)
    {
      if (!(controllerContext.Controller is ITfsController controller))
        return false;
      int apiVersionClient = controller.TfsWebContext.WebApiVersionClient;
      if (apiVersionClient < this.MinVersion)
        return false;
      return this.MaxVersion <= 0 || apiVersionClient <= this.MaxVersion;
    }
  }
}
