// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildRequestTable3
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
  internal sealed class BuildRequestTable3 : TeamFoundationTableValueParameter<BuildRequest>
  {
    private BuildSqlResourceComponent m_component;
    private static SqlMetaData[] s_metadata = new SqlMetaData[15]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
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

    internal BuildRequestTable3(ICollection<BuildRequest> rows, BuildSqlResourceComponent component)
      : base((IEnumerable<BuildRequest>) rows, "typ_BuildRequestTableV3", BuildRequestTable3.s_metadata)
    {
      this.m_component = component;
    }

    public override void SetRecord(BuildRequest row, SqlDataRecord record)
    {
      record.SetInt32(0, this.m_component.GetDataspaceId(row.ProjectId));
      record.SetInt32(1, int.Parse(DBHelper.ExtractDbId(row.BuildDefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetByte(2, (byte) row.Priority);
      record.SetBoolean(3, row.Postponed);
      record.SetGuid(4, row.BatchId);
      record.SetInt32(5, (int) row.GetOption);
      record.SetString(6, row.CustomGetVersion);
      record.SetInt32(7, row.MaxQueuePosition);
      if (!string.IsNullOrEmpty(row.BuildControllerUri))
        record.SetInt32(8, int.Parse(DBHelper.ExtractDbId(row.BuildControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      else
        record.SetDBNull(8);
      if (!string.IsNullOrEmpty(row.DropLocation))
        record.SetString(9, row.DropLocation);
      else
        record.SetDBNull(9);
      record.SetString(10, row.RequestedForIdentity.TeamFoundationId.ToString("B"));
      record.SetString(11, row.RequestedByIdentity.TeamFoundationId.ToString("B"));
      if (!string.IsNullOrEmpty(row.ShelvesetName))
        record.SetString(12, row.ShelvesetName);
      else
        record.SetDBNull(12);
      record.SetInt32(13, (int) row.Reason);
      if (!string.IsNullOrEmpty(row.ProcessParameters))
        record.SetString(14, row.ProcessParameters);
      else
        record.SetDBNull(14);
    }
  }
}
