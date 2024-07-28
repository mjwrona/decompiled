// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CommandComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CommandComponent5 : CommandComponent4
  {
    protected override ActivityLogEntryColumns GetActivityLogEntryColumns() => (ActivityLogEntryColumns) new ActivityLogEntryColumns4(this.RequestContext);

    protected override void BindSortText(StringBuilder sqlStmt, ActivityLogColumns sortColumn)
    {
      base.BindSortText(sqlStmt, sortColumn);
      if (sortColumn != ActivityLogColumns.ResponseCode)
        return;
      sqlStmt.Append(" ResponseCode ");
    }
  }
}
