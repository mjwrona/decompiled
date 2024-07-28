// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyBinder2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProxyBinder2 : ProxyBinder
  {
    private SqlColumnBinder proxyId = new SqlColumnBinder("ProxyId");

    protected override Microsoft.TeamFoundation.Core.WebApi.Proxy Bind()
    {
      Microsoft.TeamFoundation.Core.WebApi.Proxy proxy = base.Bind();
      proxy.ProxyId = this.proxyId.GetGuid((IDataReader) this.Reader);
      return proxy;
    }
  }
}
