// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionRetentionPolicyTable
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
  internal sealed class BuildDefinitionRetentionPolicyTable : 
    TeamFoundationTableValueParameter<RetentionPolicy>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[5]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("BuildStatus", SqlDbType.Int),
      new SqlMetaData("BuildReason", SqlDbType.Int),
      new SqlMetaData("NumberToKeep", SqlDbType.Int),
      new SqlMetaData("DeleteOptions", SqlDbType.Int)
    };

    public BuildDefinitionRetentionPolicyTable(ICollection<RetentionPolicy> rows)
      : base((IEnumerable<RetentionPolicy>) rows, "typ_BuildDefinitionRetentionPolicyTable", BuildDefinitionRetentionPolicyTable.s_metadata)
    {
    }

    public override void SetRecord(RetentionPolicy row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetInt32(1, (int) row.BuildStatus);
      record.SetInt32(2, (int) row.BuildReason);
      record.SetInt32(3, row.NumberToKeep);
      record.SetInt32(4, (int) row.DeleteOptions);
    }
  }
}
