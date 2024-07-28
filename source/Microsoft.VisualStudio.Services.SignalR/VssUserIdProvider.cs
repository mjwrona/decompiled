// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssUserIdProvider
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Framework.Server;
using System.Web;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public sealed class VssUserIdProvider : IUserIdProvider
  {
    public string GetUserId(IRequest request)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      HttpContextBase httpContext = request.GetHttpContext();
      if (httpContext.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext)
        httpContext.Items[(object) "VssSignalRUserContext"] = (object) requestContext.GetUserIdentity();
      else
        identity = httpContext.Items[(object) "VssSignalRUserContext"] as Microsoft.VisualStudio.Services.Identity.Identity;
      if (identity != null)
        return identity.Id.ToString();
      return request.User == null || request.User.Identity == null ? (string) null : request.User.Identity.Name;
    }
  }
}
