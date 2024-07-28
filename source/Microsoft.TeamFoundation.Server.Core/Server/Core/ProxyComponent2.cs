// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyComponent2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProxyComponent2 : ProxyComponent
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
      this.ExecuteNonQuery();
      return Guid.Empty;
    }

    public override void DeleteProxy(string proxyUrl, string site = null)
    {
      this.PrepareStoredProcedure("prc_DeleteFrameworkProxy");
      this.BindString("@proxyUrl", proxyUrl, 256, false, SqlDbType.NVarChar);
      this.BindString("@site", site, 128, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override List<Microsoft.TeamFoundation.Core.WebApi.Proxy> QueryProxies(
      IList<string> proxyUrls)
    {
      this.PrepareStoredProcedure("prc_QueryFrameworkProxies");
      if (proxyUrls == null)
        proxyUrls = (IList<string>) new List<string>();
      this.BindStringTable("@proxyUrls", (IEnumerable<string>) proxyUrls);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy>(this.GetProxyBinder());
        return resultCollection.GetCurrent<Microsoft.TeamFoundation.Core.WebApi.Proxy>().Items;
      }
    }

    protected virtual ObjectBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy> GetProxyBinder() => (ObjectBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy>) new ProxyBinder();
  }
}
