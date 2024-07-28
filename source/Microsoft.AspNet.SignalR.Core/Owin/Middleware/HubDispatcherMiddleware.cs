// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Owin.Middleware.HubDispatcherMiddleware
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Owin.Middleware
{
  public class HubDispatcherMiddleware : OwinMiddleware
  {
    private readonly HubConfiguration _configuration;

    public HubDispatcherMiddleware(OwinMiddleware next, HubConfiguration configuration)
      : base(next)
    {
      this._configuration = configuration;
    }

    public override Task Invoke(IOwinContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (JsonUtility.TryRejectJSONPRequest((ConnectionConfiguration) this._configuration, context))
        return TaskAsyncHelper.Empty;
      HubDispatcher hubDispatcher = new HubDispatcher(this._configuration);
      hubDispatcher.Initialize(this._configuration.Resolver);
      return hubDispatcher.ProcessRequest(context.Environment);
    }
  }
}
