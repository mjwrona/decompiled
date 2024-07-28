// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityLogEntryColumns5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ActivityLogEntryColumns5 : ActivityLogEntryColumns4
  {
    private SqlColumnBinder timeToFirstPageColumn = new SqlColumnBinder("TimeToFirstPage");

    public ActivityLogEntryColumns5(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override ActivityLogEntry Bind()
    {
      ActivityLogEntry activityLogEntry = base.Bind();
      activityLogEntry.TimeToFirstPage = this.timeToFirstPageColumn.GetInt64((IDataReader) this.Reader, 0L);
      return activityLogEntry;
    }
  }
}
