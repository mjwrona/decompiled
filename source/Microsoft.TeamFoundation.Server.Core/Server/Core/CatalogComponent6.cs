// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogComponent6
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
  internal sealed class CatalogComponent6 : CatalogComponent5
  {
    private static readonly SqlMetaData[] typ_CatalogResourceTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("Identifier", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, -1L),
      new SqlMetaData("TypeIdentifier", SqlDbType.UniqueIdentifier)
    };

    protected override sealed SqlParameter BindCatalogResourceTable(
      string parameterName,
      IEnumerable<CatalogResource> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<CatalogResource>();
      System.Func<CatalogResource, SqlDataRecord> selector = (System.Func<CatalogResource, SqlDataRecord>) (resource =>
      {
        SqlDataRecord record = new SqlDataRecord(CatalogComponent6.typ_CatalogResourceTable2);
        record.SetGuid(0, resource.Identifier == Guid.Empty ? resource.TempCorrelationId : resource.Identifier);
        record.SetString(1, resource.DisplayName);
        record.SetNullableString(2, resource.Description);
        record.SetGuid(3, resource.ResourceType.Identifier);
        return record;
      });
      return this.BindTable(parameterName, "typ_CatalogResourceTable2", rows.Select<CatalogResource, SqlDataRecord>(selector));
    }
  }
}
