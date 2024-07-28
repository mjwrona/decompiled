// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public abstract class TfsAreaController : TfsController
  {
    public abstract string AreaName { get; }

    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      if (requestContext.RouteData.Values.ContainsKey("area"))
        requestContext.RouteData.Values["area"] = (object) this.AreaName;
      else
        requestContext.RouteData.DataTokens["area"] = (object) this.AreaName;
    }
  }
}
