// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Owin.Middleware.PersistentConnectionMiddleware
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Owin.Middleware
{
  public class PersistentConnectionMiddleware : OwinMiddleware
  {
    private readonly Type _connectionType;
    private readonly ConnectionConfiguration _configuration;

    public PersistentConnectionMiddleware(
      OwinMiddleware next,
      Type connectionType,
      ConnectionConfiguration configuration)
      : base(next)
    {
      this._connectionType = connectionType;
      this._configuration = configuration;
    }

    public override Task Invoke(IOwinContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (JsonUtility.TryRejectJSONPRequest(this._configuration, context))
        return TaskAsyncHelper.Empty;
      PersistentConnection instance = new PersistentConnectionFactory(this._configuration.Resolver).CreateInstance(this._connectionType);
      instance.Initialize(this._configuration.Resolver);
      return instance.ProcessRequest(context.Environment);
    }
  }
}
