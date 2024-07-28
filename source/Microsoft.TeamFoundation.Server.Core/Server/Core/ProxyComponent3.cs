// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyComponent3
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProxyComponent3 : ProxyComponent2
  {
    public override Guid AddProxy(Microsoft.TeamFoundation.Core.WebApi.Proxy proxyData)
    {
      this.PrepareStoredProcedure("prc_AddFrameworkProxy");
      this.BindString("@url", proxyData.Url, 256, false, SqlDbType.NVarChar);
      this.BindString("@site", proxyData.Site, 128, true, SqlDbType.NVarChar);
      this.BindString("@friendlyName", proxyData.FriendlyName, -1, true, SqlDbType.NVarChar);
      this.BindString("@description", proxyData.Description, -1, true, SqlDbType.NVarChar);
      this.BindNullableBoolean("@siteDefault", proxyData.SiteDefault);
      this.BindNullableBoolean("@globalDefault", proxyData.GlobalDefault);
      if (proxyData.Authorization != null)
        this.BindBoolean("@authorizationRequested", true);
      else
        this.BindNullValue("@authorizationRequested", SqlDbType.Bit);
      return (Guid) this.ExecuteScalar();
    }

    protected override ObjectBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy> GetProxyBinder() => (ObjectBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy>) new ProxyBinder2();
  }
}
