// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardCardRuleBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardCardRuleBinder : ObjectBinder<BoardCardRuleRow>
  {
    protected SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    protected SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    protected SqlColumnBinder TypeColumn = new SqlColumnBinder("Type");
    protected SqlColumnBinder IsEnabledColumn = new SqlColumnBinder("IsEnabled");
    protected SqlColumnBinder OrderColumn = new SqlColumnBinder("Order");
    protected SqlColumnBinder RevisedDateColumn = new SqlColumnBinder("RevisedDate");
    protected SqlColumnBinder FilterColumn = new SqlColumnBinder("Filter");
    protected SqlColumnBinder FilterExpressionColumn = new SqlColumnBinder("FilterExpression");

    protected override BoardCardRuleRow Bind() => new BoardCardRuleRow()
    {
      Id = this.IdColumn.GetInt32((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, true),
      Type = this.TypeColumn.GetString((IDataReader) this.Reader, true),
      IsEnabled = this.IsEnabledColumn.GetBoolean((IDataReader) this.Reader, true),
      Order = this.OrderColumn.GetInt32((IDataReader) this.Reader),
      RevisedDate = this.RevisedDateColumn.GetDateTime((IDataReader) this.Reader),
      Filter = this.FilterColumn.GetString((IDataReader) this.Reader, true),
      FilterExpression = this.FilterExpressionColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
