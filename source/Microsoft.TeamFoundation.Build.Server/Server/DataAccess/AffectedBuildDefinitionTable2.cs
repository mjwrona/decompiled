// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.AffectedBuildDefinitionTable2
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
  internal sealed class AffectedBuildDefinitionTable2 : 
    TeamFoundationTableValueParameter<BuildDefinition>
  {
    private static SqlMetaData[] s_metadata = new SqlMetaData[4]
    {
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("TriggerType", SqlDbType.TinyInt)
    };

    public AffectedBuildDefinitionTable2(IEnumerable<BuildDefinition> rows)
      : base(rows, "typ_AffectedBuildDefinitionTableV2", AffectedBuildDefinitionTable2.s_metadata)
    {
    }

    public override void SetRecord(BuildDefinition row, SqlDataRecord record)
    {
      record.SetInt32(0, row.Id);
      record.SetInt32(1, int.Parse(DBHelper.ExtractDbId(row.BuildControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      record.SetString(2, row.DefaultDropLocation);
      record.SetByte(3, (byte) row.TriggerType);
    }
  }
}
