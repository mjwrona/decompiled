// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardCardStyleBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardCardStyleBinder : ObjectBinder<CardRuleAttributeRow>
  {
    protected SqlColumnBinder RuleIdColumn = new SqlColumnBinder("RuleId");
    protected SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    protected SqlColumnBinder ValueColumn = new SqlColumnBinder("Value");

    protected override CardRuleAttributeRow Bind() => new CardRuleAttributeRow()
    {
      RuleId = this.RuleIdColumn.GetInt32((IDataReader) this.Reader),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, true),
      Value = this.ValueColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
