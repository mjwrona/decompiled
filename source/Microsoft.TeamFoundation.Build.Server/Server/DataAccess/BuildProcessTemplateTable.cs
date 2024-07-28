// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildProcessTemplateTable
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  [Obsolete]
  internal sealed class BuildProcessTemplateTable : 
    TeamFoundationTableValueParameter<ProcessTemplate>
  {
    private bool m_isAdd;
    private static SqlMetaData[] s_metadata = new SqlMetaData[7]
    {
      new SqlMetaData("TemplateId", SqlDbType.Int),
      new SqlMetaData("TeamProject", SqlDbType.VarChar, 256L),
      new SqlMetaData("ServerPath", SqlDbType.NVarChar, 260L),
      new SqlMetaData("TemplateType", SqlDbType.Int),
      new SqlMetaData("SupportedReasons", SqlDbType.Int),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Parameters", SqlDbType.NVarChar, -1L)
    };

    public BuildProcessTemplateTable(ICollection<ProcessTemplate> rows, bool isAdd)
      : base((IEnumerable<ProcessTemplate>) rows, "typ_BuildProcessTemplateTable", BuildProcessTemplateTable.s_metadata)
    {
      this.m_isAdd = isAdd;
    }

    public override void SetRecord(ProcessTemplate row, SqlDataRecord record)
    {
      if (this.m_isAdd)
      {
        record.SetDBNull(0);
        record.SetString(1, row.TeamProjectObj.Uri);
        record.SetString(2, DBHelper.VersionControlPathToDBPath(row.ServerPath));
      }
      else
      {
        record.SetInt32(0, row.Id);
        record.SetDBNull(1);
        record.SetDBNull(2);
      }
      record.SetInt32(3, (int) row.TemplateType);
      record.SetInt32(4, (int) row.SupportedReasons);
      if (!string.IsNullOrEmpty(row.Description))
        record.SetString(5, row.Description);
      else
        record.SetDBNull(5);
      if (!string.IsNullOrEmpty(row.Parameters))
        record.SetString(6, row.Parameters);
      else
        record.SetDBNull(6);
    }
  }
}
