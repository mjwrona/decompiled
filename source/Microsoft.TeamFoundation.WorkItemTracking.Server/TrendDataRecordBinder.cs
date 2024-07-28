// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.TrendDataRecordBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class TrendDataRecordBinder : ObjectBinder<TrendDataRecord>
  {
    private SqlColumnBinder m_authorizedDateColumn = new SqlColumnBinder("AuthorizedDate");
    private SqlColumnBinder m_revisedDateColumn = new SqlColumnBinder("RevisedDate");
    private SqlColumnBinder m_valueColumn = new SqlColumnBinder("Value");
    private SqlColumnBinder m_countColumn = new SqlColumnBinder("Count");

    protected override TrendDataRecord Bind()
    {
      TrendDataRecord trendDataRecord = new TrendDataRecord();
      trendDataRecord.AuthorizedDate = this.m_authorizedDateColumn.GetDateTime(this.BaseReader);
      trendDataRecord.RevisedDate = this.m_revisedDateColumn.GetDateTime(this.BaseReader);
      trendDataRecord.Value = this.m_valueColumn.GetString(this.BaseReader, true);
      trendDataRecord.Count = this.m_countColumn.GetInt32(this.BaseReader);
      return trendDataRecord;
    }
  }
}
