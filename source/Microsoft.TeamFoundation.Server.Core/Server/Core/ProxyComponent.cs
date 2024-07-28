// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProxyComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ProxyComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ProxyComponent2>(2),
      (IComponentCreator) new ComponentCreator<ProxyComponent3>(3)
    }, "FrameworkProxy");
    private static readonly SqlMetaData[] typ_ProxyUrl = new SqlMetaData[2]
    {
      new SqlMetaData("OrderId", SqlDbType.Int),
      new SqlMetaData("ProxyUrl", SqlDbType.NVarChar, 256L)
    };

    static ProxyComponent()
    {
      ProxyComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      ProxyComponent.s_sqlExceptionFactories.Add(800105, new SqlExceptionFactory(typeof (ProxyAlreadyAddedToSiteException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ProxyAlreadyAddedToSiteException())));
    }

    public ProxyComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ProxyComponent.s_sqlExceptionFactories;

    public virtual Guid AddProxy(Microsoft.TeamFoundation.Core.WebApi.Proxy proxyData)
    {
      this.PrepareStoredProcedure("prc_AddProxy");
      this.BindString("@url", proxyData.Url, 256, false, SqlDbType.NVarChar);
      this.BindString("@friendlyName", proxyData.FriendlyName, -1, true, SqlDbType.NVarChar);
      this.BindString("@site", proxyData.Site, 128, true, SqlDbType.NVarChar);
      this.BindString("@description", proxyData.Description, -1, true, SqlDbType.NVarChar);
      int parameterValue = 0;
      if (proxyData.SiteDefault.GetValueOrDefault())
        ++parameterValue;
      if (proxyData.GlobalDefault.GetValueOrDefault())
        parameterValue += 2;
      this.BindInt("@flags", parameterValue);
      this.ExecuteNonQuery();
      return Guid.Empty;
    }

    public virtual void DeleteProxy(string proxyUrl, string site = null)
    {
      this.PrepareStoredProcedure("prc_DeleteProxy");
      this.BindString("@proxyUrl", proxyUrl, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual List<Microsoft.TeamFoundation.Core.WebApi.Proxy> QueryProxies(
      IList<string> proxyUrls)
    {
      this.PrepareStoredProcedure("prc_QueryProxies");
      this.BindProxyUrlTable("@proxyUrls", (IEnumerable<string>) proxyUrls);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy>((ObjectBinder<Microsoft.TeamFoundation.Core.WebApi.Proxy>) new ProxyBinderOld());
        return resultCollection.GetCurrent<Microsoft.TeamFoundation.Core.WebApi.Proxy>().Items;
      }
    }

    protected SqlParameter BindProxyUrlTable(string parameterName, IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      int index = 0;
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (proxyUrl =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProxyComponent.typ_ProxyUrl);
        sqlDataRecord.SetInt32(0, index);
        sqlDataRecord.SetString(1, proxyUrl);
        ++index;
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ProxyUrl", rows.Select<string, SqlDataRecord>(selector));
    }
  }
}
