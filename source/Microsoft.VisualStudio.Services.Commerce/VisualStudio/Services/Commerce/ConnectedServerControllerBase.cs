// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ConnectedServerControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(3.0)]
  public abstract class ConnectedServerControllerBase : CommerceControllerBase
  {
    internal override string Layer => nameof (ConnectedServerControllerBase);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap) => base.InitializeExceptionMap(exceptionMap);

    [HttpPost]
    public ConnectedServer CreateConnectedServer(ConnectedServer connectedServer) => this.TfsRequestContext.GetService<IConnectedServerService>().Create(this.TfsRequestContext, connectedServer);
  }
}
