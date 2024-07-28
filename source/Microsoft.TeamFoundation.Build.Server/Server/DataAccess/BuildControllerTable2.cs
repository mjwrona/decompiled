// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildControllerTable2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  [Obsolete]
  internal sealed class BuildControllerTable2 : TeamFoundationTableValueParameter<BuildController>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[9]
    {
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("ServiceHostId", SqlDbType.Int),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("MaxConcurrentBuilds", SqlDbType.Int),
      new SqlMetaData("CustomAssemblyPath", SqlDbType.NVarChar, 400L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Enabled", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L)
    };

    public BuildControllerTable2(IEnumerable<BuildController> rows)
      : base(rows, "typ_BuildControllerTableV2", BuildControllerTable2.s_metadata)
    {
    }

    public override void SetRecord(BuildController row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.Uri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetInt32(1, int.Parse(DBHelper.ExtractDbId(row.ServiceHostUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetString(2, DBHelper.ServerPathToDBPath(row.Name));
      record.SetInt32(3, row.MaxConcurrentBuilds);
      if (!string.IsNullOrEmpty(row.CustomAssemblyPath))
        record.SetString(4, row.CustomAssemblyPath);
      else
        record.SetDBNull(4);
      record.SetByte(5, (byte) row.Status);
      record.SetString(6, row.StatusMessage ?? string.Empty);
      record.SetBoolean(7, row.Enabled);
      if (!string.IsNullOrEmpty(row.Description))
        record.SetString(8, row.Description);
      else
        record.SetDBNull(8);
    }
  }
}
