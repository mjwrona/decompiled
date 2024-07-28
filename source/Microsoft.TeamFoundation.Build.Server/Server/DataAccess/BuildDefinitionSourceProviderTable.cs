// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildDefinitionSourceProviderTable
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
  internal sealed class BuildDefinitionSourceProviderTable : 
    TeamFoundationTableValueParameter<BuildDefinitionSourceProvider>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[4]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("SourceProviderId", SqlDbType.Int),
      new SqlMetaData("SourceProvider", SqlDbType.NVarChar, 64L),
      new SqlMetaData("Fields", SqlDbType.NVarChar, -1L)
    };

    public BuildDefinitionSourceProviderTable(ICollection<BuildDefinitionSourceProvider> rows)
      : base((IEnumerable<BuildDefinitionSourceProvider>) rows, "typ_BuildDefinitionSourceProviderTable", BuildDefinitionSourceProviderTable.s_metadata)
    {
    }

    public override void SetRecord(BuildDefinitionSourceProvider row, SqlDataRecord record)
    {
      record.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.DefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetInt32(1, row.Id);
      record.SetString(2, row.Name);
      record.SetString(3, BuildSqlColumnBinderExtensions.ToXml(row.Fields.ToArray()));
    }
  }
}
