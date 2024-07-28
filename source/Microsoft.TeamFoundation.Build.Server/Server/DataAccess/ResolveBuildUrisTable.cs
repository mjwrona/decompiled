// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.ResolveBuildUrisTable
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
  internal sealed class ResolveBuildUrisTable : TeamFoundationTableValueParameter<string>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[3]
    {
      new SqlMetaData("BuildUri", SqlDbType.NVarChar, 256L),
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("ResolveLatest", SqlDbType.Bit)
    };

    public ResolveBuildUrisTable(ICollection<string> buildUris)
      : base((IEnumerable<string>) buildUris, "typ_ResolveBuildUrisTable", ResolveBuildUrisTable.s_metadata)
    {
    }

    public override void SetRecord(string row, SqlDataRecord record)
    {
      string dbId = DBHelper.ExtractDbId(row);
      int length = dbId.IndexOf("?queueId=", StringComparison.OrdinalIgnoreCase);
      int num = dbId.IndexOf("&latest", StringComparison.OrdinalIgnoreCase);
      if (length > 0)
      {
        record.SetString(0, dbId.Substring(0, length));
        if (num > 0)
        {
          record.SetInt32(1, int.Parse(dbId.Substring(length + 9, num - length - 9), (IFormatProvider) CultureInfo.InvariantCulture));
          record.SetBoolean(2, true);
        }
        else
        {
          record.SetInt32(1, int.Parse(dbId.Substring(length + 9), (IFormatProvider) CultureInfo.InvariantCulture));
          record.SetBoolean(2, false);
        }
      }
      else
      {
        record.SetString(0, dbId);
        record.SetDBNull(1);
        record.SetBoolean(2, true);
      }
    }
  }
}
