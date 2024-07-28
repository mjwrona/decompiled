// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionTable2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  [Obsolete]
  internal sealed class BuildDefinitionTable2 : TeamFoundationTableValueParameter<BuildDefinition>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[12]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("TeamProject", SqlDbType.VarChar, 256L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("TriggerType", SqlDbType.TinyInt),
      new SqlMetaData("QuietPeriod", SqlDbType.Int),
      new SqlMetaData("BatchSize", SqlDbType.Int),
      new SqlMetaData("QueueStatus", SqlDbType.TinyInt),
      new SqlMetaData("ProcessTemplateId", SqlDbType.Int),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L)
    };

    public BuildDefinitionTable2(IEnumerable<BuildDefinition> rows)
      : base(rows, "typ_BuildDefinitionTableV2", BuildDefinitionTable2.s_metadata)
    {
    }

    public override void SetRecord(BuildDefinition row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.Uri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetString(1, row.TeamProject.Uri);
      record.SetString(2, DBHelper.ServerPathToDBPath(BuildPath.RemoveTeamProject(row.FullPath)));
      record.SetInt32(3, int.Parse(DBHelper.ExtractDbId(row.BuildControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetString(4, row.DefaultDropLocation);
      record.SetByte(5, (byte) row.TriggerType);
      record.SetInt32(6, row.ContinuousIntegrationQuietPeriod);
      record.SetInt32(7, row.BatchSize);
      record.SetByte(8, (byte) row.QueueStatus);
      if (row.Process != null)
        record.SetInt32(9, row.Process.Id);
      else
        record.SetDBNull(9);
      if (!string.IsNullOrEmpty(row.Description))
        record.SetString(10, row.Description);
      else
        record.SetDBNull(10);
      if (!string.IsNullOrEmpty(row.ProcessParameters))
        record.SetString(11, row.ProcessParameters);
      else
        record.SetDBNull(11);
    }
  }
}
