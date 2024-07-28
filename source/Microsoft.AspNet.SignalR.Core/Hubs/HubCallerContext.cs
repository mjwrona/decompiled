// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hubs.HubCallerContext
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using System.Collections.Generic;
using System.Security.Principal;

namespace Microsoft.AspNet.SignalR.Hubs
{
  public class HubCallerContext
  {
    public virtual string ConnectionId { get; private set; }

    public virtual IDictionary<string, Cookie> RequestCookies => this.Request.Cookies;

    public virtual INameValueCollection Headers => this.Request.Headers;

    public virtual INameValueCollection QueryString => this.Request.QueryString;

    public virtual IPrincipal User => this.Request.User;

    public virtual IRequest Request { get; private set; }

    protected HubCallerContext()
    {
    }

    public HubCallerContext(IRequest request, string connectionId)
    {
      this.ConnectionId = connectionId;
      this.Request = request;
    }
  }
}
