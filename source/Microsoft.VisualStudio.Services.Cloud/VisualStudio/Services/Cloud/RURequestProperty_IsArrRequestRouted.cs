// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_IsArrRequestRouted
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Web;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_IsArrRequestRouted : RURequestProperty_Bool
  {
    public override RUStage PropertyKnownAt { get; protected set; } = RUStage.BeginRequest;

    public override bool ShouldOutputEntityToTelemetry { get; protected set; }

    public override object GetRequestValue(IVssRequestContext requestContext)
    {
      if (!(requestContext is IWebRequestContextInternal requestContextInternal))
        return (object) false;
      HttpContextBase httpContext = requestContextInternal.HttpContext;
      return (object) (bool) (httpContext != null ? (httpContext.Items.Contains((object) HttpContextConstants.ArrRequestRouted) ? 1 : 0) : 0);
    }

    public override object GetXEventValue(XEventObjectBase xeventObject) => throw new VssServiceException("Not supported - Are you trying to bucket by " + this.GetName() + " using an XEvent resource?");
  }
}
