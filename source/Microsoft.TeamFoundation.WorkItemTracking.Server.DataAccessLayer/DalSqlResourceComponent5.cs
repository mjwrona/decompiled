// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent5 : DalSqlResourceComponent4
  {
    public DalSqlResourceComponent5() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override void UpdateQueryTexts(IEnumerable<KeyValuePair<Guid, string>> queries)
    {
      string sqlStatement = "\r\nUPDATE  QI\r\nSET     QI.Text = UpdatedQueryItem.[Value]\r\nFROM    dbo.QueryItems AS QI \r\nJOIN    @queries AS UpdatedQueryItem\r\nON      QI.ID = UpdatedQueryItem.[Key] AND QI.PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      this.BindKeyValuePairGuidStringTable("@queries", queries);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<KeyValuePair<Guid, string>> GetAllQueryTexts()
    {
      string sqlStatement = "\r\nSELECT ID, Text \r\nFROM dbo.QueryItems\r\nWHERE fFolder = 0 AND fDeleted = 0 AND PartitionId = @partitionId\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n";
      this.PrepareSqlBatch(sqlStatement.Length, true);
      this.AddStatement(sqlStatement);
      return this.ReadQueryTexts(this.ExecuteReader());
    }
  }
}
