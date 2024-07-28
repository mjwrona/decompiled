// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildRequestTable2
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
  internal sealed class BuildRequestTable2 : TeamFoundationTableValueParameter<BuildRequest>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[14]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("Priority", SqlDbType.TinyInt),
      new SqlMetaData("Postponed", SqlDbType.Bit),
      new SqlMetaData("BatchId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("GetOption", SqlDbType.Int),
      new SqlMetaData("GetVersion", SqlDbType.NVarChar, 326L),
      new SqlMetaData("MaxPosition", SqlDbType.Int),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RequestedFor", SqlDbType.NVarChar, 38L),
      new SqlMetaData("RequestedBy", SqlDbType.NVarChar, 38L),
      new SqlMetaData("ShelvesetName", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Reason", SqlDbType.Int),
      new SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L)
    };

    internal BuildRequestTable2(ICollection<BuildRequest> rows)
      : base((IEnumerable<BuildRequest>) rows, "typ_BuildRequestTableV2", BuildRequestTable2.s_metadata)
    {
    }

    public override void SetRecord(BuildRequest row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.BuildDefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetByte(1, (byte) row.Priority);
      record.SetBoolean(2, row.Postponed);
      record.SetGuid(3, row.BatchId);
      record.SetInt32(4, (int) row.GetOption);
      record.SetString(5, row.CustomGetVersion);
      record.SetInt32(6, row.MaxQueuePosition);
      if (!string.IsNullOrEmpty(row.BuildControllerUri))
        record.SetInt32(7, int.Parse(DBHelper.ExtractDbId(row.BuildControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      else
        record.SetDBNull(7);
      if (!string.IsNullOrEmpty(row.DropLocation))
        record.SetString(8, row.DropLocation);
      else
        record.SetDBNull(8);
      record.SetString(9, row.RequestedForIdentity.TeamFoundationId.ToString("B"));
      record.SetString(10, row.RequestedByIdentity.TeamFoundationId.ToString("B"));
      if (!string.IsNullOrEmpty(row.ShelvesetName))
        record.SetString(11, row.ShelvesetName);
      else
        record.SetDBNull(11);
      record.SetInt32(12, (int) row.Reason);
      if (!string.IsNullOrEmpty(row.ProcessParameters))
        record.SetString(13, row.ProcessParameters);
      else
        record.SetDBNull(13);
    }
  }
}
