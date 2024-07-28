// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent2
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent2 : ScaledAgileViewComponent
  {
    private static readonly SqlMetaData[] typ_ScaledAgileViewPropertyTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("TeamId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("WorkItemTypeName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Order", SqlDbType.Int)
    };

    protected override SqlParameter BindScaledAgileViewPropertyTable(
      string parameterName,
      IEnumerable<ScaledAgileViewProperty> rows)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ScaledAgileViewProperty>>(rows, "IEnumerable");
      return this.BindTable(parameterName, "typ_ScaledAgileViewPropertyTable2", rows.Select<ScaledAgileViewProperty, SqlDataRecord>((Func<ScaledAgileViewProperty, int, SqlDataRecord>) ((row, index) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ScaledAgileViewComponent2.typ_ScaledAgileViewPropertyTable2);
        sqlDataRecord.SetGuid(0, row.TeamId);
        sqlDataRecord.SetString(1, row.CategoryReferenceName);
        sqlDataRecord.SetInt32(2, index);
        return sqlDataRecord;
      })));
    }
  }
}
