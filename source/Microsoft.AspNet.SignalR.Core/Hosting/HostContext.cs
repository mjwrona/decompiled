// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Hosting.HostContext
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Owin;
using System.Collections.Generic;

namespace Microsoft.AspNet.SignalR.Hosting
{
  public class HostContext
  {
    public IRequest Request { get; private set; }

    public IResponse Response { get; private set; }

    public IDictionary<string, object> Environment { get; private set; }

    public HostContext(IRequest request, IResponse response)
    {
      this.Request = request;
      this.Response = response;
      this.Environment = (IDictionary<string, object>) new Dictionary<string, object>();
    }

    public HostContext(IDictionary<string, object> environment)
    {
      this.Request = (IRequest) new ServerRequest(environment);
      this.Response = (IResponse) new ServerResponse(environment);
      this.Environment = environment;
    }
  }
}
